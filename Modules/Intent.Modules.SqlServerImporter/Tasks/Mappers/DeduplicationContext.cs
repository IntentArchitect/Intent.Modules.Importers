using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

/// <summary>
/// Context for tracking deduplicated names during import operations to avoid naming conflicts
/// </summary>
public class DeduplicationContext
{
    private readonly List<string> _addedTableNames = new();
    private readonly List<string> _addedColumnNames = new();
    private readonly List<string> _addedViewNames = new();
    private readonly List<string> _addedProcedureNames = new();

    /// <summary>
    /// Deduplicates a table/class name within the given schema
    /// </summary>
    public string DeduplicateTable(string className, string schema)
    {
        var counter = 0;
        var addedReference = $"[{schema}].[{className}]";
        var originalClassName = className;

        while (_addedTableNames.Any(x => x == addedReference))
        {
            counter++;
            className = $"{originalClassName}{counter}";
            addedReference = $"[{schema}].[{className}]";
        }

        _addedTableNames.Add(addedReference);
        return className;
    }

    /// <summary>
    /// Deduplicates a view/class name within the given schema
    /// </summary>
    public string DeduplicateView(string className, string schema)
    {
        var counter = 0;
        var addedReference = $"[{schema}].[{className}]";
        var originalClassName = className;

        while (_addedViewNames.Any(x => x == addedReference))
        {
            counter++;
            className = $"{originalClassName}{counter}";
            addedReference = $"[{schema}].[{className}]";
        }

        _addedViewNames.Add(addedReference);
        return className;
    }

    /// <summary>
    /// Deduplicates a column/attribute name within the given class and schema
    /// </summary>
    public string DeduplicateColumn(string propertyName, string className, string schema)
    {
        if (propertyName == className)
        {
            propertyName = propertyName + "Property";
        }

        var counter = 0;
        var addedReference = $"[{schema}].[{className}].[{propertyName}]";
        var originalPropertyName = propertyName;

        while (_addedColumnNames.Any(x => x == addedReference))
        {
            counter++;
            propertyName = $"{originalPropertyName}{counter}";
            addedReference = $"[{schema}].[{className}].[{propertyName}]";
        }

        _addedColumnNames.Add(addedReference);
        return propertyName;
    }

    /// <summary>
    /// Deduplicates a stored procedure name within the given schema
    /// </summary>
    public string DeduplicateStoredProcedure(string procedureName, string schema)
    {
        var counter = 0;
        var addedReference = $"[{schema}].[{procedureName}]";
        var originalProcedureName = procedureName;

        while (_addedProcedureNames.Any(x => x == addedReference))
        {
            counter++;
            procedureName = $"{originalProcedureName}{counter}";
            addedReference = $"[{schema}].[{procedureName}]";
        }

        _addedProcedureNames.Add(addedReference);
        return procedureName;
    }
} 