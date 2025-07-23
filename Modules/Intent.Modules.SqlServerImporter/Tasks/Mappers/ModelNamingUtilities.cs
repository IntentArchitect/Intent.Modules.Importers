using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

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
        // Normalize column name
        var normalized = NormalizeColumnName(columnName, tableName);
        return deduplicationContext?.DeduplicateColumn(normalized, className, schema) ?? normalized;
    }

    public static string GetStoredProcedureName(string procName, string schema, DeduplicationContext? deduplicationContext)
    {
        // Remove schema prefix if present and convert to PascalCase
        var name = procName.Contains('.') ? procName.Split('.').Last() : procName;
        var normalized = NormalizeStoredProcName(name);
        return deduplicationContext?.DeduplicateStoredProcedure(normalized, schema) ?? normalized;
    }

    public static string GetParameterName(string paramName)
    {
        // Remove @ prefix if present and convert to camelCase
        var name = paramName.StartsWith("@") ? paramName.Substring(1) : paramName;
        return name.ToCamelCase();
    }

    /// <summary>
    /// Converts database identifier to valid C# identifier following C# naming conventions
    /// </summary>
    public static string ToCSharpIdentifier(string identifier, string prefixValue = "Db")
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
        var normalized = tableName.RemovePrefix("tbl");
        normalized = ToCSharpIdentifier(normalized, "Db");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    public static string NormalizeColumnName(string colName, string? tableOrViewName)
    {
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
        var normalized = ToCSharpIdentifier(storeProcName);
        normalized = normalized.RemovePrefix("prc")
            .RemovePrefix("Prc")
            .RemovePrefix("proc");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    public static string NormalizeSchemaName(string schemaName)
    {
        var normalized = schemaName;
        return normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
    }

    /// <summary>
    /// Generates ExternalReference for table following [schema].[tablename] pattern
    /// </summary>
    public static string GetTableExternalReference(string tableName, string schema)
    {
        return $"[{schema}].[{tableName}]";
    }

    /// <summary>
    /// Generates ExternalReference for view following [schema].[viewname] pattern
    /// </summary>
    public static string GetViewExternalReference(string viewName, string schema)
    {
        return $"[{schema}].[{viewName}]";
    }

    /// <summary>
    /// Generates ExternalReference for column following [schema].[tablename].[columnname] pattern
    /// </summary>
    public static string GetColumnExternalReference(string columnName, string tableName, string schema)
    {
        return $"[{schema}].[{tableName}].[{columnName}]";
    }

    /// <summary>
    /// Generates ExternalReference for stored procedure following [schema].[procname] pattern
    /// </summary>
    public static string GetStoredProcedureExternalReference(string procName, string schema)
    {
        return $"stored-procedure:[{schema.ToLower()}].[{procName.ToLower()}]";
    }

    /// <summary>
    /// Generates ExternalReference for data contract following stored procedure external ref + .Response pattern
    /// </summary>
    public static string GetDataContractExternalReference(string procName, string schema)
    {
        return $"{GetStoredProcedureExternalReference(procName, schema)}.Response";
    }

    /// <summary>
    /// Generates ExternalReference for foreign key following [schema].[tablename].[fkname] pattern
    /// </summary>
    public static string GetForeignKeyExternalReference(string fkName, string tableName, string schema)
    {
        return $"[{schema}].[{tableName}].[{fkName}]";
    }

    /// <summary>
    /// Generates ExternalReference for trigger following trigger:[schema].[tablename].[triggername] pattern
    /// </summary>
    public static string GetTriggerExternalReference(string triggerName, string tableName, string schema)
    {
        return $"trigger:[{schema.ToLower()}].[{tableName.ToLower()}].[{triggerName.ToLower()}]";
    }

    /// <summary>
    /// Generates ExternalReference for result set column following data contract external ref + .[columnname] pattern
    /// </summary>
    public static string GetResultSetColumnExternalReference(string columnName, string procName, string schema)
    {
        return $"{GetDataContractExternalReference(procName, schema)}.[{columnName.ToLower()}]";
    }
} 