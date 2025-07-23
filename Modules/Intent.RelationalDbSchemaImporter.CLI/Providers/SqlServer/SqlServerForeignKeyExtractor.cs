using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Microsoft.Data.SqlClient;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerForeignKeyExtractor : DefaultForeignKeyExtractor
{
    public override async Task<List<ForeignKeySchema>> ExtractForeignKeysAsync(DatabaseTable table, DbConnection connection)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        // Always try enhanced SQL Server foreign key extraction first
        var sqlServerForeignKeys = await ExtractSqlServerForeignKeysAsync(table, connection);

        if (sqlServerForeignKeys.Count > 0)
        {
            foreignKeys.AddRange(sqlServerForeignKeys);
        }

        return foreignKeys;
    }

    private static async Task<List<ForeignKeySchema>> ExtractSqlServerForeignKeysAsync(DatabaseTable table, DbConnection connection)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        // T-SQL query to get foreign key metadata with referenced table info
        const string sql =
            """
            SELECT DISTINCT
                fk.name AS ForeignKeyName,
                SCHEMA_NAME(rt.schema_id) AS ReferencedSchema,
                rt.name AS ReferencedTable
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.tables rt ON fk.referenced_object_id = rt.object_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
            ORDER BY fk.name
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Add parameters
        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@SchemaName";
        schemaParam.Value = table.SchemaOwner ?? "dbo";
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@TableName";
        tableParam.Value = table.Name;
        command.Parameters.Add(tableParam);

        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var foreignKeyName = reader["ForeignKeyName"]?.ToString() ?? "";
                var referencedSchema = reader["ReferencedSchema"]?.ToString() ?? "dbo";
                var referencedTable = reader["ReferencedTable"]?.ToString() ?? "";

                var foreignKeySchema = new ForeignKeySchema
                {
                    Name = foreignKeyName,
                    ReferencedTableName = referencedTable,
                    ReferencedTableSchema = referencedSchema,
                    // Don't set Columns here due to the need of another SQL Connection inside an existing one
                };

                foreignKeys.Add(foreignKeySchema);
            }
        }

        foreach (var foreignKey in foreignKeys)
        {
            foreignKey.Columns = await ExtractSqlServerForeignKeyColumnsAsync(table, foreignKey.Name, connection);
        }

        return foreignKeys;
    }

    private static async Task<List<ForeignKeyColumnSchema>> ExtractSqlServerForeignKeyColumnsAsync(DatabaseTable table, string foreignKeyName, DbConnection connection)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // T-SQL query to get detailed foreign key column mapping
        const string sql =
            """
            SELECT 
                pc.name AS SourceColumn,
                rc.name AS ReferencedColumn
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
            INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
              AND fk.name = @ForeignKeyName
            ORDER BY fkc.constraint_column_id
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Add parameters
        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@SchemaName";
        schemaParam.Value = table.SchemaOwner ?? "dbo";
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@TableName";
        tableParam.Value = table.Name;
        command.Parameters.Add(tableParam);

        var foreignKeyParam = command.CreateParameter();
        foreignKeyParam.ParameterName = "@ForeignKeyName";
        foreignKeyParam.Value = foreignKeyName;
        command.Parameters.Add(foreignKeyParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var sourceColumn = reader["SourceColumn"]?.ToString() ?? "";
            var referencedColumn = reader["ReferencedColumn"]?.ToString() ?? "";

            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = sourceColumn,
                ReferencedColumnName = referencedColumn
            };

            columns.Add(columnSchema);
        }


        return columns;
    }
}