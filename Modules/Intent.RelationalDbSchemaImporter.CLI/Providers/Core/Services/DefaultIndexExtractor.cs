using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class IndexExtractorBase
{
    public abstract Task<List<IndexSchema>> ExtractIndexesAsync(DatabaseTable table, ImportFilterService importFilterService, DbConnection connection);
    public abstract List<IndexColumnSchema> ExtractIndexColumns(DatabaseIndex index);
}

/// <summary>
/// Base implementation for index extraction from database tables
/// </summary>
internal class DefaultIndexExtractor : IndexExtractorBase
{
    /// <summary>
    /// Extract table indexes. Can be overridden for database-specific implementations
    /// </summary>
    public override async Task<List<IndexSchema>> ExtractIndexesAsync(DatabaseTable table, ImportFilterService importFilterService, DbConnection connection)
    {
        var indexes = new List<IndexSchema>();

        if (!importFilterService.ExportIndexes())
        {
            return indexes;
        }
        
        // DatabaseSchemaReader provides indexes through the table.Indexes collection
        foreach (var index in table.Indexes ?? [])
        {
            var indexSchema = new IndexSchema
            {
                Name = index.Name ?? "",
                IsUnique = index.IsUnique,
                IsClustered = false, // DatabaseSchemaReader doesn't expose this directly for all databases
                HasFilter = false, // DatabaseSchemaReader doesn't expose filter information directly
                FilterDefinition = null, 
                Columns = ExtractIndexColumns(index)
            };

            indexes.Add(indexSchema);
        }

        return indexes;
    }

    /// <summary>
    /// Extract index column information from a database index
    /// </summary>
    public override List<IndexColumnSchema> ExtractIndexColumns(DatabaseIndex index)
    {
        var columns = new List<IndexColumnSchema>();

        // DatabaseSchemaReader provides index columns
        foreach (var indexColumn in index.Columns ?? [])
        {
            var columnSchema = new IndexColumnSchema
            {
                Name = indexColumn.Name ?? "",
                IsDescending = false, // DatabaseSchemaReader doesn't expose sort order directly
                IsIncluded = false // DatabaseSchemaReader doesn't expose this directly
            };

            columns.Add(columnSchema);
        }

        return columns;
    }
} 