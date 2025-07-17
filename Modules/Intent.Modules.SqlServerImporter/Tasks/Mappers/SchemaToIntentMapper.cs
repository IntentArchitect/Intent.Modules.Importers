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
                    var classElement = _intentModelMapper.MapTableToClass(table, _config);
                    
                    // Check if class already exists and update or add
                    var existingClass = existingElements.FirstOrDefault(c => 
                        c.Name == classElement.Name || 
                        IsTableMappedToClass(c, table));
                    
                    if (existingClass != null)
                    {
                        UpdateExistingClass(existingClass, classElement);
                        result.UpdatedElements.Add(existingClass);
                    }
                    else
                    {
                        package.Classes.Add(classElement);
                        result.AddedElements.Add(classElement);
                    }
                }
            }

            // Map views to classes
            if (_config.ExportViews())
            {
                foreach (var view in databaseSchema.Views)
                {
                    var classElement = _intentModelMapper.MapViewToClass(view, _config);
                    
                    var existingClass = existingElements.FirstOrDefault(c => 
                        c.Name == classElement.Name || 
                        IsViewMappedToClass(c, view));
                    
                    if (existingClass != null)
                    {
                        UpdateExistingClass(existingClass, classElement);
                        result.UpdatedElements.Add(existingClass);
                    }
                    else
                    {
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
                    var procElement = _intentModelMapper.MapStoredProcedureToElement(storedProc, _config);
                    
                    // Add to appropriate collection based on stored procedure type
                    if (_config.StoredProcedureType == StoredProcedureType.StoredProcedureElement)
                    {
                        // Add to stored procedures collection if it exists
                        // For now, add to classes collection as operations
                        package.Classes.Add(procElement);
                    }
                    else
                    {
                        // Add as operation to repository or service
                        package.Classes.Add(procElement);
                    }
                    
                    result.AddedElements.Add(procElement);
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
