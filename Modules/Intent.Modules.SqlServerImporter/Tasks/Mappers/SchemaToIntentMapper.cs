using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

public class SchemaToIntentMapper
{
    private readonly IntentModelMapper _intentModelMapper;
    private readonly ImportConfiguration _config;
    
    // Track schema folders to avoid duplicates
    private readonly Dictionary<string, ElementPersistable> _schemaFolders = new();

    public SchemaToIntentMapper(ImportConfiguration config)
    {
        _config = config;
        _intentModelMapper = new IntentModelMapper();
    }

    public PackageUpdateResult MapSchemaToPackage(DatabaseSchema databaseSchema, PackageModelPersistable package)
    {
        var result = new PackageUpdateResult();

        try
        {
            // Clear existing elements to rebuild from schema
            var existingElements = package.Classes.ToList();
            
            // Map tables to classes
            if (_config.ExportTables())
            {
                foreach (var table in databaseSchema.Tables)
                {
                    // Check if class already exists first (before creating folder structure)
                    // Create a temporary mapping to get the expected name
                    var tempClassElement = _intentModelMapper.MapTableToClass(table, _config, null);
                    var existingClass = existingElements.FirstOrDefault(c => 
                        c.Name == tempClassElement.Name || 
                        IsTableMappedToClass(c, table));
                    
                    if (existingClass != null)
                    {
                        // Update existing class without moving it (keep existing ParentFolderId)
                        var updatedClassElement = _intentModelMapper.MapTableToClass(table, _config, existingClass.ParentFolderId);
                        UpdateExistingClass(existingClass, updatedClassElement);
                        result.UpdatedElements.Add(existingClass);
                        
                        // Process indexes for existing class
                        ProcessTableIndexes(table, existingClass, package);
                    }
                    else
                    {
                        // Only create schema folder for new elements
                        var schemaFolder = GetOrCreateSchemaFolder(table.Schema, package);
                        var classElement = _intentModelMapper.MapTableToClass(table, _config, schemaFolder.Id);
                        
                        package.Classes.Add(classElement);
                        result.AddedElements.Add(classElement);
                        
                        // Process indexes for new class
                        ProcessTableIndexes(table, classElement, package);
                    }
                }
            }

            // Map views to classes
            if (_config.ExportViews())
            {
                foreach (var view in databaseSchema.Views)
                {
                    // Check if class already exists first (before creating folder structure)
                    var tempClassElement = _intentModelMapper.MapViewToClass(view, _config, null);
                    var existingClass = existingElements.FirstOrDefault(c => 
                        c.Name == tempClassElement.Name || 
                        IsViewMappedToClass(c, view));
                    
                    if (existingClass != null)
                    {
                        // Update existing class without moving it (keep existing ParentFolderId)
                        var updatedClassElement = _intentModelMapper.MapViewToClass(view, _config, existingClass.ParentFolderId);
                        UpdateExistingClass(existingClass, updatedClassElement);
                        result.UpdatedElements.Add(existingClass);
                    }
                    else
                    {
                        // Only create schema folder for new elements
                        var schemaFolder = GetOrCreateSchemaFolder(view.Schema, package);
                        var classElement = _intentModelMapper.MapViewToClass(view, _config, schemaFolder.Id);
                        
                        package.Classes.Add(classElement);
                        result.AddedElements.Add(classElement);
                    }
                }
            }

            // Map stored procedures
            if (_config.ExportStoredProcedures())
            {
                foreach (var storedProc in databaseSchema.StoredProcedures)
                {
                    ElementPersistable procElement;
                    
                    // Create stored procedure element based on configuration
                    if (_config.StoredProcedureType == StoredProcedureType.StoredProcedureElement)
                    {
                        // Create as stored procedure element (no parent folder for standalone elements)
                        procElement = _intentModelMapper.MapStoredProcedureToElement(storedProc, null, _config);
                    }
                    else
                    {
                        // Create repository first if it doesn't exist
                        var repositoryElement = GetOrCreateRepository(storedProc.Schema, package);
                        
                        // Create as operation within repository
                        procElement = _intentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, _config);
                        repositoryElement.ChildElements.Add(procElement);
                        
                        // Don't add to package.Classes since it's a child of repository
                        result.AddedElements.Add(procElement);
                        continue;
                    }
                    
                    // Check if stored procedure element already exists
                    var existingProc = existingElements.FirstOrDefault(c => 
                        c.Name == procElement.Name && 
                        c.SpecializationType == procElement.SpecializationType);
                    
                    if (existingProc != null)
                    {
                        UpdateExistingClass(existingProc, procElement);
                        result.UpdatedElements.Add(existingProc);
                    }
                    else
                    {
                        package.Classes.Add(procElement);
                        result.AddedElements.Add(procElement);
                    }
                }
            }

            result.IsSuccessful = true;
            result.Message = $"Successfully mapped {result.AddedElements.Count} new elements and updated {result.UpdatedElements.Count} existing elements.";
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Error mapping schema to package: {ex.Message}";
            result.Exception = ex;
        }

        return result;
    }

    /// <summary>
    /// Processes table indexes and creates them in the package
    /// </summary>
    private void ProcessTableIndexes(TableSchema table, ElementPersistable classElement, PackageModelPersistable package)
    {
        foreach (var index in table.Indexes)
        {
            var indexElement = _intentModelMapper.CreateIndex(index, classElement.Id, package);
            package.Classes.Add(indexElement);
            
            // Create index columns
            foreach (var indexColumn in index.Columns)
            {
                // Find the corresponding attribute in the class
                var attribute = classElement.ChildElements.FirstOrDefault(attr => 
                    attr.Name.Equals(indexColumn.Name, StringComparison.OrdinalIgnoreCase));
                
                var indexColumnElement = _intentModelMapper.CreateIndexColumn(indexColumn, indexElement.Id, attribute?.Id, package);
                indexElement.ChildElements.Add(indexColumnElement);
            }
        }
    }

    private bool IsTableMappedToClass(ElementPersistable classElement, TableSchema table)
    {
        // Check if class has table stereotype with matching table name
        return classElement.Stereotypes?.Any(s => 
            s.DefinitionId == Constants.Stereotypes.Rdbms.Table.DefinitionId &&
            s.Properties?.Any(p => p.Value == table.Name) == true) == true;
    }

    private bool IsViewMappedToClass(ElementPersistable classElement, ViewSchema view)
    {
        // Check if class has view stereotype with matching view name
        return classElement.Stereotypes?.Any(s => 
            s.DefinitionId == Constants.Stereotypes.Rdbms.View.DefinitionId &&
            s.Properties?.Any(p => p.Value == view.Name) == true) == true;
    }

    private void UpdateExistingClass(ElementPersistable existingClass, ElementPersistable newClass)
    {
        // Update stereotypes
        existingClass.Stereotypes = newClass.Stereotypes;
        
        // Update child elements (attributes)
        existingClass.ChildElements = newClass.ChildElements;
        
        // Update type reference if needed
        if (newClass.TypeReference != null)
        {
            existingClass.TypeReference = newClass.TypeReference;
        }
    }

    public static ImportConfiguration CreateImportConfiguration(DatabaseImportModel importModel)
    {
        return new ImportConfiguration
        {
            ApplicationId = importModel.ApplicationId,
            EntityNameConvention = Enum.Parse<EntityNameConvention>(importModel.EntityNameConvention),
            TableStereotype = Enum.Parse<TableStereotype>(importModel.TableStereotype),
            TypesToExport = importModel.TypesToExport.Select(t => Enum.Parse<ExportType>(t)).ToHashSet(),
            ImportFilterFilePath = importModel.ImportFilterFilePath,
            StoredProcedureType = string.IsNullOrWhiteSpace(importModel.StoredProcedureType) 
                ? StoredProcedureType.Default 
                : Enum.Parse<StoredProcedureType>(importModel.StoredProcedureType),
            ConnectionString = importModel.ConnectionString,
            PackageFileName = importModel.PackageFileName
        };
    }

    /// <summary>
    /// Gets or creates a schema folder for organizing elements by database schema
    /// </summary>
    private ElementPersistable GetOrCreateSchemaFolder(string schemaName, PackageModelPersistable package)
    {
        if (_schemaFolders.TryGetValue(schemaName, out var existingFolder))
            return existingFolder;

        // Check if folder already exists in package
        var folder = package.Classes.FirstOrDefault(c => 
            c.Name == GetNormalizedSchemaName(schemaName) && 
            c.SpecializationType == "Folder");

        if (folder == null)
        {
            folder = _intentModelMapper.CreateSchemaFolder(schemaName, package.Id);
            package.Classes.Add(folder);
        }

        _schemaFolders[schemaName] = folder;
        return folder;
    }

    private static string GetNormalizedSchemaName(string schemaName)
    {
        return schemaName.Substring(0, 1).ToUpper() + schemaName.Substring(1);
    }

    /// <summary>
    /// Gets or creates a repository element for stored procedure operations
    /// </summary>
    private ElementPersistable GetOrCreateRepository(string schemaName, PackageModelPersistable package)
    {
        var repositoryName = $"{GetNormalizedSchemaName(schemaName)}Repository";
        
        // Check if repository already exists
        var repository = package.Classes.FirstOrDefault(c => 
            c.Name == repositoryName && 
            c.SpecializationType == "Repository");

        if (repository == null)
        {
            // Get or create schema folder for repository organization
            var schemaFolder = GetOrCreateSchemaFolder(schemaName, package);
            repository = _intentModelMapper.CreateRepository(repositoryName, schemaFolder.Id);
            package.Classes.Add(repository);
        }

        return repository;
    }
}

public class PackageUpdateResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public List<ElementPersistable> AddedElements { get; set; } = new();
    public List<ElementPersistable> UpdatedElements { get; set; } = new();
    public List<ElementPersistable> RemovedElements { get; set; } = new();
}
