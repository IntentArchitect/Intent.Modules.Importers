using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

public class IntentModelMapper
{
    private readonly TypeReferenceMapper _typeReferenceMapper;
    private readonly RdbmsStereotypeApplicator _stereotypeApplicator;

    public IntentModelMapper()
    {
        _typeReferenceMapper = new TypeReferenceMapper();
        _stereotypeApplicator = new RdbmsStereotypeApplicator();
    }

    public ElementPersistable MapTableToClass(TableSchema table, ImportConfiguration config)
    {
        var className = GetEntityName(table.Name, config.EntityNameConvention);
        
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = className,
            SpecializationType = "Class",
            SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d94", // Class specialization type ID
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };

        // Map columns to attributes
        foreach (var column in table.Columns)
        {
            var attribute = MapColumnToAttribute(column);
            classElement.ChildElements.Add(attribute);
            
            // Apply stereotypes
            _stereotypeApplicator.ApplyPrimaryKey(column, attribute);
            _stereotypeApplicator.ApplyColumnDetails(column, attribute);
            _stereotypeApplicator.ApplyTextConstraint(column, attribute);
            _stereotypeApplicator.ApplyDecimalConstraint(column, attribute);
            _stereotypeApplicator.ApplyDefaultConstraint(column, attribute);
            _stereotypeApplicator.ApplyComputedValue(column, attribute);
        }

        // Apply table stereotypes
        _stereotypeApplicator.ApplyTableDetails(config, table, classElement);

        return classElement;
    }

    public ElementPersistable MapViewToClass(ViewSchema view, ImportConfiguration config)
    {
        var className = GetEntityName(view.Name, config.EntityNameConvention);
        
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = className,
            SpecializationType = "Class",
            SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d94", // Class specialization type ID
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };

        // Map columns to attributes
        foreach (var column in view.Columns)
        {
            var attribute = MapColumnToAttribute(column);
            classElement.ChildElements.Add(attribute);
            
            // Apply stereotypes (views don't have primary keys, defaults, or computed values)
            _stereotypeApplicator.ApplyColumnDetails(column, attribute);
            _stereotypeApplicator.ApplyTextConstraint(column, attribute);
            _stereotypeApplicator.ApplyDecimalConstraint(column, attribute);
        }

        // Apply view stereotypes
        _stereotypeApplicator.ApplyViewDetails(view, classElement);

        return classElement;
    }

    public ElementPersistable MapStoredProcedureToElement(StoredProcedureSchema storedProc, ImportConfiguration config)
    {
        var procElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = GetStoredProcedureName(storedProc.Name),
            SpecializationType = "Operation", // or appropriate stored procedure type
            SpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb", // Operation specialization type ID
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };

        // Map parameters
        foreach (var parameter in storedProc.Parameters)
        {
            var paramElement = MapParameterToElement(parameter);
            procElement.ChildElements.Add(paramElement);
        }

        // Apply stored procedure stereotypes
        _stereotypeApplicator.ApplyStoredProcedureSettings(storedProc, procElement);

        return procElement;
    }

    private ElementPersistable MapColumnToAttribute(ColumnSchema column)
    {
        var attributeName = GetAttributeName(column.Name);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = attributeName,
            SpecializationType = "Attribute",
            SpecializationTypeId = "0090fb93-483e-4e74-8cce-1e9b96e5fb9d", // Attribute specialization type ID
            TypeReference = _typeReferenceMapper.MapColumnTypeToTypeReference(column),
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };
    }

    private ElementPersistable MapParameterToElement(StoredProcedureParameterSchema parameter)
    {
        var paramName = GetParameterName(parameter.Name);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = paramName,
            SpecializationType = "Parameter",
            SpecializationTypeId = "f1b9c8c8-e8e8-4e8e-8e8e-8e8e8e8e8e8e", // Parameter specialization type ID (placeholder)
            TypeReference = _typeReferenceMapper.MapStoredProcedureParameterTypeToTypeReference(parameter),
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };
    }

    private string GetEntityName(string tableName, EntityNameConvention convention)
    {
        return convention switch
        {
            EntityNameConvention.MatchTable => tableName,
            EntityNameConvention.SingularEntity => tableName.Singularize(),
            _ => tableName
        };
    }

    private string GetAttributeName(string columnName)
    {
        // Convert to PascalCase and remove underscores
        return columnName.ToPascalCase();
    }

    private string GetStoredProcedureName(string procName)
    {
        // Remove schema prefix if present and convert to PascalCase
        var name = procName.Contains('.') ? procName.Split('.').Last() : procName;
        return name.ToPascalCase();
    }

    private string GetParameterName(string paramName)
    {
        // Remove @ prefix if present and convert to camelCase
        var name = paramName.StartsWith("@") ? paramName.Substring(1) : paramName;
        return name.ToCamelCase();
    }
}
