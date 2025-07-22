using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class ForeignKeyExtractorBase
{
    public abstract Task<List<ForeignKeySchema>> ExtractForeignKeysAsync(DatabaseTable table, DbConnection connection);
    public abstract Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseConstraint foreignKey, DbConnection connection);
}

/// <summary>
/// Base implementation for foreign key extraction from database tables
/// </summary>
internal class DefaultForeignKeyExtractor : ForeignKeyExtractorBase
{
    /// <summary>
    /// Extract table foreign keys. Can be overridden for database-specific implementations to fix missing referenced column names
    /// </summary>
    public override async Task<List<ForeignKeySchema>> ExtractForeignKeysAsync(DatabaseTable table, DbConnection connection)
    {
        var foreignKeys = new List<ForeignKeySchema>();
        
        // DatabaseSchemaReader provides comprehensive foreign key information
        foreach (var foreignKey in table.ForeignKeys ?? [])
        {
            var fkSchema = new ForeignKeySchema
            {
                Name = foreignKey.Name ?? $"FK_{table.Name}",
                ReferencedTableSchema = foreignKey.RefersToSchema ?? "",
                ReferencedTableName = foreignKey.RefersToTable ?? "",
                Columns = await ExtractForeignKeyColumnsAsync(foreignKey, connection)
            };

            foreignKeys.Add(fkSchema);
        }

        return foreignKeys;
    }

    /// <summary>
    /// Extract foreign key columns. Can be overridden for database-specific implementations to populate ReferencedColumnName
    /// </summary>
    public override Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseConstraint foreignKey, DbConnection connection)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // DatabaseSchemaReader provides column mappings in foreign keys
        foreach (var column in foreignKey.Columns ?? [])
        {
            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = column ?? "",
                // ReferencedColumnName is not provided by DatabaseSchemaReader
                // Database-specific implementations should override this method to populate it
                ReferencedColumnName = ""
            };

            columns.Add(columnSchema);
        }

        return Task.FromResult(columns);
    }
} 