using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

/// <summary>
/// Handles naming conventions and external reference generation for database schema elements
/// </summary>
internal static class ModelNamingUtilities
{
    // Reserved C# keywords
    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
        "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", 
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try",
        "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void", "volatile", "while"
    };

    public static string GetEntityName(string tableName, EntityNameConvention convention, string schema, DeduplicationContext? deduplicationContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        var normalized = convention switch
        {
            EntityNameConvention.MatchTable => NormalizeTableName(tableName),
            EntityNameConvention.SingularEntity => NormalizeTableName(tableName.Singularize()),
            _ => NormalizeTableName(tableName)
        };
        return deduplicationContext?.DeduplicateTable(normalized, schema) ?? normalized;
    }

    public static string GetViewName(string viewName, EntityNameConvention convention, string schema, DeduplicationContext? deduplicationContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        var normalized = convention switch
        {
            EntityNameConvention.MatchTable => NormalizeTableName(viewName),
            EntityNameConvention.SingularEntity => NormalizeTableName(viewName.Singularize()),
            _ => NormalizeTableName(viewName)
        };
        return deduplicationContext?.DeduplicateView(normalized, schema) ?? normalized;
    }

    public static string GetAttributeName(string columnName, string? tableName, string className, string schema, DeduplicationContext? deduplicationContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        ArgumentException.ThrowIfNullOrWhiteSpace(className);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        // Normalize column name
        var normalized = NormalizeColumnName(columnName, tableName);
        return deduplicationContext?.DeduplicateColumn(normalized, className, schema) ?? normalized;
    }

    public static string GetStoredProcedureName(string procName, string schema, DeduplicationContext? deduplicationContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(procName);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        // Remove schema prefix if present and convert to PascalCase
        var name = procName.Contains('.') ? procName.Split('.').Last() : procName;
        var normalized = NormalizeStoredProcName(name);
        return deduplicationContext?.DeduplicateStoredProcedure(normalized, schema) ?? normalized;
    }

    public static string GetParameterName(string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paramName);
        // Remove @ prefix if present and convert to camelCase
        var normalized = paramName.StartsWith("@") ? paramName.Substring(1) : paramName;
        normalized=ToCSharpIdentifier(normalized);
        return normalized.ToCamelCase();
    }

    /// <summary>
    /// Converts database identifier to valid C# identifier following C# naming conventions
    /// </summary>
    public static string ToCSharpIdentifier(string? identifier, string? prefixValue = "Db")
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return string.Empty;
        }

        // https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names
        // - Identifiers must start with a letter, or _.
        // - Identifiers may contain Unicode letter characters, decimal digit characters,
        //   Unicode connecting characters, Unicode combining characters, or Unicode formatting
        //   characters. For more information on Unicode categories, see the Unicode Category
        //   Database. You can declare identifiers that match C# keywords by using the @ prefix
        //   on the identifier. The @ is not part of the identifier name. For example, @if
        //   declares an identifier named if. These verbatim identifiers are primarily for
        //   interoperability with identifiers declared in other languages.
        
        identifier = identifier
            .Replace("#", "Sharp")
            .Replace("&", "And");

        var asCharArray = identifier.ToCharArray();
        for (var i = 0; i < asCharArray.Length; i++)
        {
            // Underscore character conversion to space for processing
            if (asCharArray[i] == '_')
            {
                asCharArray[i] = ' ';
                continue;
            }

            switch (char.GetUnicodeCategory(asCharArray[i]))
            {
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.Format:
                    break;
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.Control:
                case UnicodeCategory.CurrencySymbol:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.MathSymbol:
                case UnicodeCategory.ModifierSymbol:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.OpenPunctuation:
                case UnicodeCategory.OtherNotAssigned:
                case UnicodeCategory.OtherNumber:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.PrivateUse:
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.Surrogate:
                    asCharArray[i] = ' ';
                    break;
                default:
                    asCharArray[i] = ' ';
                    break;
            }
        }

        identifier = new string(asCharArray);

        // Replace double spaces
        while (identifier.Contains("  "))
        {
            identifier = identifier.Replace("  ", " ");
        }

        // Convert to PascalCase
        identifier = string.Concat(identifier
            .Split(' ')
            .Where(element => !string.IsNullOrWhiteSpace(element))
            .Select((element, index) => index == 0
                ? element
                : element.ToPascalCase()));

        // Ensure identifier starts with letter
        if (char.IsNumber(identifier[0]))
        {
            identifier = $"{prefixValue}{identifier}";
        }

        // Handle reserved words
        if (ReservedWords.Contains(identifier))
        {
            identifier = $"{prefixValue}{identifier}";
        }

        return identifier;
    }

    public static string NormalizeTableName(string tableName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        var normalized = tableName.RemovePrefix("tbl");
        normalized = ToCSharpIdentifier(normalized, "Db");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    public static string NormalizeColumnName(string colName, string? tableOrViewName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(colName);
        var normalized = colName != tableOrViewName ? colName : colName + "Value";
        normalized = ToCSharpIdentifier(normalized, "db");
        normalized = normalized.RemovePrefix("col").RemovePrefix("pk");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);

        if (normalized.EndsWith("ID"))
        {
            normalized = normalized.RemoveSuffix("ID") + "Id";
        }

        return normalized;
    }

    public static string NormalizeStoredProcName(string storeProcName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storeProcName);
        var normalized = ToCSharpIdentifier(storeProcName);
        normalized = normalized.RemovePrefix("prc")
            .RemovePrefix("Prc");
        // We need to be careful with the "proc" prefix since a name could start with:
        // procedure, procurement, process, etc.
        // So what we can do is check if the letter after "proc" is a capital letter or a non-letter character.
        if (normalized.StartsWith("proc") &&
            (normalized.Length < 5 || !char.IsLetter(normalized[4]) || char.IsUpper(normalized[4])))
        {
            normalized = normalized.RemoveSuffix("proc");
        }

        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    public static string NormalizeSchemaName(string schemaName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);
        var normalized = schemaName;
        return normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
    }
    
    /// <summary>
    /// Generates name for UserDefinedTable DataContract
    /// </summary>
    public static string NormalizeUserDefinedTableName(string udtName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(udtName);
        var normalized = udtName;
        return normalized.Substring(0, 1).ToUpper() + normalized.Substring(1) + "Model";
    }

    /// <summary>
    /// Generates ExternalReference for table following [schema].[tablename] pattern
    /// </summary>
    public static string GetTableExternalReference(string schema, string tableName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        return $"[{schema.ToLowerInvariant()}].[{tableName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for view following [schema].[viewname] pattern
    /// </summary>
    public static string GetViewExternalReference(string schema, string viewName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);
        return $"[{schema.ToLowerInvariant()}].[{viewName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for column following [schema].[tablename].[columnname] pattern
    /// </summary>
    public static string GetColumnExternalReference(string schema, string tableName, string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        return $"[{schema.ToLowerInvariant()}].[{tableName.ToLowerInvariant()}].[{columnName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for stored procedure following [schema].[procname] pattern
    /// </summary>
    public static string GetStoredProcedureExternalReference(string schema, string procName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(procName);
        return $"[{schema.ToLowerInvariant()}].[{procName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for data contract following stored procedure external ref + .Response pattern
    /// </summary>
    public static string GetDataContractExternalReference(string schema, string procName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(procName);
        return $"{GetStoredProcedureExternalReference(schema, procName)}.Response";
    }

    /// <summary>
    /// Generates ExternalReference for foreign key following [schema].[tablename].[fkname] pattern
    /// </summary>
    public static string GetForeignKeyExternalReference(string schema, string tableName, string fkName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fkName);
        return $"[{schema.ToLowerInvariant()}].[{tableName.ToLowerInvariant()}].[{fkName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for trigger following trigger:[schema].[tablename].[triggername] pattern
    /// </summary>
    public static string GetTriggerExternalReference(string schema, string tableName, string triggerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(triggerName);
        return $"trigger:[{schema.ToLowerInvariant()}].[{tableName.ToLowerInvariant()}].[{triggerName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for result set column following data contract external ref + .[columnname] pattern
    /// </summary>
    public static string GetResultSetColumnExternalReference(string schema, string procName, string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(procName);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        return $"[{GetDataContractExternalReference(schema, procName).ToLowerInvariant()}].[{columnName.ToLowerInvariant()}]";
    }

    public static string GetIndexExternalReference(string tableSchema, string tableName, string indexName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);
        return $"index:[{tableSchema.ToLowerInvariant()}].[{tableName.ToLowerInvariant()}].[{indexName.ToLowerInvariant()}]";
    }

    /// <summary>
    /// Generates ExternalReference for UserDefinedTable DataContract following [schema].[udtname].UDT pattern
    /// </summary>
    public static string GetUserDefinedTableDataContractExternalReference(string schema, string udtName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(udtName);
        return $"[{schema.ToLowerInvariant()}].[{udtName.ToLowerInvariant()}].UDT";
    }

    /// <summary>
    /// Generates ExternalReference for UserDefinedTable column following UDT external ref + .[columnname] pattern
    /// </summary>
    public static string GetUserDefinedTableColumnExternalReference(string schema, string udtName, string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(udtName);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        return $"{GetUserDefinedTableDataContractExternalReference(schema, udtName)}.[{columnName.ToLowerInvariant()}]";
    }

    public static string GetSchemaExternalReference(string schemaName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);
        return $"schema:[{schemaName.ToLowerInvariant()}]";
    }

    public static string GetIndexColumnExternalReference(string indexColumnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexColumnName);
        return indexColumnName.ToLowerInvariant();
    }
} 