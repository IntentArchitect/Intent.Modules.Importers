using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using DatabaseSchema = DatabaseSchemaReader.DataSchema.DatabaseSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class ForeignKeyExtractorBase
{
    public abstract Task<List<ForeignKeySchema>> ExtractForeignKeysAsync(DatabaseTable table, DbConnection connection);
    protected abstract Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseSchema databaseSchema, DatabaseConstraint foreignKey, DbConnection connection);
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
                Columns = await ExtractForeignKeyColumnsAsync(table.DatabaseSchema, foreignKey, connection)
            };

            foreignKeys.Add(fkSchema);
        }

        return foreignKeys;
    }

    /// <summary>
    /// Extract foreign key columns. Can be overridden for database-specific implementations to populate ReferencedColumnName
    /// </summary>
    protected override Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseSchema databaseSchema, DatabaseConstraint foreignKey, DbConnection connection)
    {
        var refCols = foreignKey.ReferencedColumns(databaseSchema).ToArray();
        var columns = new List<ForeignKeyColumnSchema>();
        for (var index = 0; index < foreignKey.Columns.Count; index++)
        {
            var curTableColumn = foreignKey.Columns[index];
            var refTableColumn = refCols.Length > index ? refCols[index] : string.Empty;
            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = curTableColumn,
                ReferencedColumnName = refTableColumn
            };
            columns.Add(columnSchema);
        }

        return Task.FromResult(columns);
    }
} 