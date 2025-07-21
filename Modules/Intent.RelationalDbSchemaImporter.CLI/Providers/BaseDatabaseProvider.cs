using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Base implementation for database providers using DatabaseSchemaReader
/// </summary>
internal abstract class BaseDatabaseProvider : IDatabaseProvider
{
    public abstract DatabaseType SupportedType { get; }
    
    protected abstract DbConnection CreateConnection(string connectionString);
    protected abstract IDependencyResolver CreateDependencyResolver(DbConnection connection);
    protected abstract IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection);

    public virtual async Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportFilterService importFilterService)
    {
        using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        var schema = new DatabaseSchema
        {
            DatabaseName = connection.Database, // Use connection database name as fallback
            Tables = [],
            Views = [],
            StoredProcedures = [],
            UserDefinedTableTypes = []
        };

        if (importFilterService.ExportTables())
        {
            schema.Tables = await ExtractTablesAsync(databaseSchema, importFilterService, connection);
        }

        if (importFilterService.ExportViews())
        {
            schema.Views = await ExtractViewsAsync(databaseSchema, importFilterService);
        }

        if (importFilterService.ExportStoredProcedures())
        {
            schema.StoredProcedures = await ExtractStoredProceduresAsync(databaseSchema, importFilterService, connection);
        }

        return schema;
    }

    public virtual async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = CreateConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected virtual async Task<List<TableSchema>> ExtractTablesAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var tables = new List<TableSchema>();
        var filteredTables = databaseSchema.Tables
            .Where(table => importFilterService.ExportTable(table.SchemaOwner ?? "dbo", table.Name))
            .ToArray();

        // Handle dependent tables if required
        if (importFilterService.IncludeDependantTables())
        {
            var dependencyResolver = CreateDependencyResolver(connection);
            var tableNames = filteredTables.Select(t => $"{t.SchemaOwner ?? "dbo"}.{t.Name}");
            var dependentTableNames = await dependencyResolver.GetDependentTablesAsync(tableNames);
            
            var dependentTables = databaseSchema.Tables
                .Where(t => dependentTableNames.Contains($"{t.SchemaOwner ?? "dbo"}.{t.Name}"))
                .Where(t => !filteredTables.Contains(t))
                .Where(t => importFilterService.ExportDependantTable(t.SchemaOwner ?? "dbo", t.Name));
            
            filteredTables = filteredTables.Concat(dependentTables).ToArray();
        }

        foreach (var table in filteredTables)
        {
            var tableSchema = new TableSchema
            {
                Name = table.Name,
                Schema = table.SchemaOwner ?? "dbo",
                Columns = ExtractTableColumns(table, importFilterService),
                Indexes = ExtractTableIndexes(table, importFilterService),
                ForeignKeys = ExtractTableForeignKeys(table),
                Triggers = ExtractTableTriggers(table)
            };

            tables.Add(tableSchema);
        }

        return tables;
    }
    
    protected virtual List<ColumnSchema> ExtractTableColumns(
        DatabaseSchemaReader.DataSchema.DatabaseTable table, 
        ImportFilterService importFilterService)
    {
        var columns = new List<ColumnSchema>();

        foreach (var col in table.Columns)
        {
            if (!importFilterService.ExportTableColumn(table.SchemaOwner ?? "dbo", table.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = GetDataTypeString(col.DataType?.TypeName),
                IsNullable = col.Nullable,
                IsPrimaryKey = col.IsPrimaryKey,
                IsIdentity = col.IsAutoNumber,
                MaxLength = GetMaxLength(col),
                NumericPrecision = GetNumericPrecision(col),
                NumericScale = GetNumericScale(col),
                DefaultConstraint = ExtractDefaultConstraint(col),
                ComputedColumn = ExtractComputedColumn(col)
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    protected virtual List<IndexSchema> ExtractTableIndexes(
        DatabaseSchemaReader.DataSchema.DatabaseTable table, 
        ImportFilterService importFilterService)
    {
        // Basic implementation - would be enhanced by derived classes
        return [];
    }

    protected virtual List<ForeignKeySchema> ExtractTableForeignKeys(
        DatabaseSchemaReader.DataSchema.DatabaseTable table)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        // Simplified implementation - using basic properties
        foreach (var foreignKey in table.ForeignKeys ?? [])
        {
            var fkSchema = new ForeignKeySchema
            {
                Name = foreignKey.Name ?? $"FK_{table.Name}",
                ReferencedTableSchema = "dbo", // Default schema
                ReferencedTableName = foreignKey.RefersToTable ?? "",
                Columns = [] // Simplified for now
            };

            foreignKeys.Add(fkSchema);
        }

        return foreignKeys;
    }

    protected virtual List<TriggerSchema> ExtractTableTriggers(
        DatabaseSchemaReader.DataSchema.DatabaseTable table)
    {
        // Basic implementation - would be enhanced by derived classes
        return [];
    }

    protected virtual async Task<List<ViewSchema>> ExtractViewsAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService)
    {
        var views = new List<ViewSchema>();
        // Basic implementation for views
        return views;
    }

    protected virtual async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var storedProcedures = new List<StoredProcedureSchema>();
        // Basic implementation for stored procedures
        return storedProcedures;
    }

    protected virtual DefaultConstraintSchema? ExtractDefaultConstraint(DatabaseSchemaReader.DataSchema.DatabaseColumn column)
    {
        if (string.IsNullOrEmpty(column.DefaultValue))
        {
            return null;
        }

        return new DefaultConstraintSchema
        {
            Text = column.DefaultValue
        };
    }

    protected virtual ComputedColumnSchema? ExtractComputedColumn(DatabaseSchemaReader.DataSchema.DatabaseColumn column)
    {
        // DatabaseSchemaReader doesn't expose computed column information directly
        return null;
    }

    protected virtual string GetDataTypeString(string? dataTypeName)
    {
        return dataTypeName?.ToLowerInvariant() ?? "unknown";
    }

    // Fixed: Access properties directly from DatabaseColumn instead of DataType
    protected virtual int? GetMaxLength(DatabaseSchemaReader.DataSchema.DatabaseColumn column)
    {
        return column.Length > 0 ? column.Length : null;
    }

    protected virtual int? GetNumericPrecision(DatabaseSchemaReader.DataSchema.DatabaseColumn column)
    {
        return column.Precision > 0 ? column.Precision : null;
    }

    protected virtual int? GetNumericScale(DatabaseSchemaReader.DataSchema.DatabaseColumn column)
    {
        return column.Scale > 0 ? column.Scale : null;
    }
} 