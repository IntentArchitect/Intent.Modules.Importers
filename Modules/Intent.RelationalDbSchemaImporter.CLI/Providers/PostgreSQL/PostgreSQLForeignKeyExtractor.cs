using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSQLForeignKeyExtractor : DefaultForeignKeyExtractor
{
    public override async Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseConstraint foreignKey, DbConnection connection)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // Use PostgreSQL-specific query to get both source and referenced column names
        const string sql =
            """
            SELECT 
                kcu.column_name AS source_column,
                ccu.column_name AS referenced_column
            FROM information_schema.key_column_usage kcu
            INNER JOIN information_schema.constraint_column_usage ccu 
                ON kcu.constraint_name = ccu.constraint_name 
                AND kcu.constraint_schema = ccu.constraint_schema
            WHERE kcu.constraint_name = @constraintName
              AND kcu.constraint_schema = @schemaName
            ORDER BY kcu.ordinal_position;
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var constraintParam = command.CreateParameter();
        constraintParam.ParameterName = "@constraintName";
        constraintParam.Value = foreignKey.Name ?? "";
        command.Parameters.Add(constraintParam);

        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@schemaName";
        schemaParam.Value = foreignKey.SchemaOwner ?? "public";
        command.Parameters.Add(schemaParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var sourceColumn = reader["source_column"]?.ToString() ?? "";
            var referencedColumn = reader["referenced_column"]?.ToString() ?? "";

            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = sourceColumn,
                ReferencedColumnName = referencedColumn
            };

            columns.Add(columnSchema);
        }

        // If we didn't get any results, fall back to base implementation
        if (columns.Count == 0)
        {
            return await base.ExtractForeignKeyColumnsAsync(foreignKey, connection);
        }

        return columns;
    }
} 