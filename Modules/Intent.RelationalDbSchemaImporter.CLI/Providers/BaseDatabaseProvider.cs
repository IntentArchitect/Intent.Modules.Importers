using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using DatabaseSchema = Intent.RelationalDbSchemaImporter.Contracts.DbSchema.DatabaseSchema;

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

    /// <summary>
    /// Gets the base SQL data type name without length/precision information
    /// </summary>
    /// <remarks>
    /// PostgresSQL: SERIAL / BIGSERIAL aren't real datatypes - syntactic sugar for autoincrement columns - is actually INT4 / INT8. 
    /// </remarks>
    protected virtual string GetDataTypeString(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        // Strip length/precision information (e.g., "nvarchar(255)" -> "nvarchar", "decimal(18,2)" -> "decimal")
        var baseTypeName = dataTypeName;
        var parenIndex = baseTypeName.IndexOf('(');
        if (parenIndex > 0)
        {
            baseTypeName = baseTypeName.Substring(0, parenIndex);
        }

        return baseTypeName.Trim().ToLowerInvariant();
    }

    public virtual async Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportFilterService importFilterService)
    {
        await using var connection = CreateConnection(connectionString);
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

    public virtual async Task TestConnectionAsync(string connectionString, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        var result = await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public virtual async Task<List<string>> GetTableNamesAsync(string connectionString)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        return databaseSchema.Tables
            .Where(table => !IsSystemObject(table.SchemaOwner, table.Name))
            .Select(t => $"{t.SchemaOwner}.{t.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    public virtual async Task<List<string>> GetViewNamesAsync(string connectionString)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        return databaseSchema.Views
            .Where(view => !IsSystemObject(view.SchemaOwner, view.Name))
            .Select(v => $"{v.SchemaOwner}.{v.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    public virtual async Task<List<string>> GetRoutineNamesAsync(string connectionString)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        var routines = new List<string>();
        
        // Add stored procedures
        if (databaseSchema.StoredProcedures != null)
        {
            routines.AddRange(databaseSchema.StoredProcedures
                .Where(sp => !IsSystemObject(sp.SchemaOwner, sp.Name))
                .Select(sp => $"{sp.SchemaOwner}.{sp.Name}"));
        }
        
        // Add functions (if supported by DatabaseSchemaReader)
        if (databaseSchema.Functions != null)
        {
            routines.AddRange(databaseSchema.Functions
                .Where(func => !IsSystemObject(func.SchemaOwner, func.Name))
                .Select(func => $"{func.SchemaOwner}.{func.Name}"));
        }
        
        return routines.OrderBy(name => name).ToList();
    }

    protected virtual bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;
        
        var systemSchemas = new[] { "sys", "INFORMATION_SCHEMA", "information_schema", "pg_catalog", "pg_toast" };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };
        
        return systemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    protected virtual async Task<List<TableSchema>> ExtractTablesAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var tables = new List<TableSchema>();
        var filteredTables = databaseSchema.Tables
            .Where(table => !IsSystemObject(table.SchemaOwner, table.Name) && importFilterService.ExportTable(table.SchemaOwner, table.Name))
            .ToArray();

        // Handle dependent tables if required
        if (importFilterService.IncludeDependantTables())
        {
            var dependencyResolver = CreateDependencyResolver(connection);
            var tableNames = filteredTables.Select(t => $"{t.SchemaOwner}.{t.Name}");
            var dependentTableNames = await dependencyResolver.GetDependentTablesAsync(tableNames);
            
            var dependentTables = databaseSchema.Tables
                .Where(t => dependentTableNames.Contains($"{t.SchemaOwner}.{t.Name}"))
                .Where(t => !filteredTables.Contains(t))
                .Where(t => importFilterService.ExportDependantTable(t.SchemaOwner, t.Name));
            
            filteredTables = filteredTables.Concat(dependentTables).ToArray();
        }

        foreach (var table in filteredTables)
        {
            var tableSchema = new TableSchema
            {
                Name = table.Name,
                Schema = table.SchemaOwner,
                Columns = ExtractTableColumns(table, importFilterService),
                Indexes = ExtractTableIndexes(table, importFilterService),
                ForeignKeys = ExtractTableForeignKeys(table),
                Triggers = ExtractTableTriggers(table)
            };

            tables.Add(tableSchema);
        }

        return tables;
    }
    
    protected virtual List<ColumnSchema> ExtractTableColumns(DatabaseTable table, ImportFilterService importFilterService)
    {
        var columns = new List<ColumnSchema>();

        foreach (var col in table.Columns)
        {
            if (!importFilterService.ExportTableColumn(table.SchemaOwner, table.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = GetDataTypeString(col.DbDataType),
                NormalizedDataType = GetNormalizedDataTypeString(col.DataType, col.DbDataType),
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

    protected virtual List<IndexSchema> ExtractTableIndexes(DatabaseTable table, ImportFilterService importFilterService)
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

    protected virtual List<IndexColumnSchema> ExtractIndexColumns(DatabaseIndex index)
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

    protected virtual List<ForeignKeySchema> ExtractTableForeignKeys(DatabaseTable table)
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
                Columns = ExtractForeignKeyColumns(foreignKey)
            };

            foreignKeys.Add(fkSchema);
        }

        return foreignKeys;
    }

    protected virtual List<ForeignKeyColumnSchema> ExtractForeignKeyColumns(DatabaseConstraint foreignKey)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // DatabaseSchemaReader provides column mappings in foreign keys
        foreach (var column in foreignKey.Columns ?? [])
        {
            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = column ?? "",
                ReferencedColumnName = "" // DatabaseSchemaReader doesn't expose referenced column mapping directly
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    protected virtual List<TriggerSchema> ExtractTableTriggers(DatabaseTable table)
    {
        var triggers = new List<TriggerSchema>();

        // DatabaseSchemaReader provides trigger information
        foreach (var trigger in table.Triggers ?? [])
        {
            var triggerSchema = new TriggerSchema
            {
                Name = trigger.Name ?? "",
                ParentSchema = table.SchemaOwner ?? "",
                ParentName = table.Name ?? "",
                ParentType = "Table"
            };

            triggers.Add(triggerSchema);
        }

        return triggers;
    }

    protected virtual async Task<List<ViewSchema>> ExtractViewsAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService)
    {
        var views = new List<ViewSchema>();
        var filteredViews = databaseSchema.Views
            .Where(view => !IsSystemObject(view.SchemaOwner, view.Name) && importFilterService.ExportView(view.SchemaOwner, view.Name))
            .ToArray();

        foreach (var view in filteredViews)
        {
            var viewSchema = new ViewSchema
            {
                Name = view.Name,
                Schema = view.SchemaOwner,
                Columns = ExtractViewColumns(view, importFilterService),
                Triggers = ExtractViewTriggers(view)
            };

            views.Add(viewSchema);
        }

        return views;
    }

    protected virtual List<ColumnSchema> ExtractViewColumns(DatabaseView view, ImportFilterService importFilterService)
    {
        var columns = new List<ColumnSchema>();

        foreach (var col in view.Columns ?? [])
        {
            if (!importFilterService.ExportViewColumn(view.SchemaOwner, view.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = GetDataTypeString(col.DbDataType),
                NormalizedDataType = GetNormalizedDataTypeString(col.DataType, col.DbDataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = false, // Views don't have primary keys
                IsIdentity = false, // Views don't have identity columns
                MaxLength = GetMaxLength(col),
                NumericPrecision = GetNumericPrecision(col),
                NumericScale = GetNumericScale(col),
                DefaultConstraint = null, // Views don't have default constraints
                ComputedColumn = null // Views don't have computed columns in this context
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    protected virtual List<TriggerSchema> ExtractViewTriggers(DatabaseView view)
    {
        var triggers = new List<TriggerSchema>();

        // DatabaseSchemaReader provides trigger information for views
        foreach (var trigger in view.Triggers ?? [])
        {
            var triggerSchema = new TriggerSchema
            {
                Name = trigger.Name ?? "",
                ParentSchema = view.SchemaOwner ?? "",
                ParentName = view.Name ?? "",
                ParentType = "View"
            };

            triggers.Add(triggerSchema);
        }

        return triggers;
    }

    protected virtual async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var storedProcedures = new List<StoredProcedureSchema>();
        
        // Combine stored procedures and functions (PostgreSQL uses functions instead of stored procedures)
        var routines = new List<DatabaseStoredProcedure>();
        
        // Add stored procedures
        if (databaseSchema.StoredProcedures != null)
        {
            routines.AddRange(databaseSchema.StoredProcedures
                .Where(sp => !IsSystemObject(sp.SchemaOwner, sp.Name) && importFilterService.ExportStoredProcedure(sp.SchemaOwner, sp.Name)));
        }
        
        // Add functions (important for PostgreSQL)
        if (databaseSchema.Functions != null)
        {
            // Convert functions to stored procedure format for consistent handling
            var functionRoutines = databaseSchema.Functions
                .Where(func => !IsSystemObject(func.SchemaOwner, func.Name) && importFilterService.ExportStoredProcedure(func.SchemaOwner, func.Name))
                .Cast<DatabaseStoredProcedure>(); // Functions inherit from base routine type
            
            routines.AddRange(functionRoutines);
        }

        // Apply additional filtering if specific stored procedure names are specified
        if (importFilterService.GetStoredProcedureNames().Count > 0)
        {
            var storedProcLookup = new HashSet<string>(importFilterService.GetStoredProcedureNames(), StringComparer.OrdinalIgnoreCase);
            routines = routines.Where(routine => 
                storedProcLookup.Contains(routine.Name) || 
                storedProcLookup.Contains($"{routine.SchemaOwner}.{routine.Name}"))
                .ToList();
        }

        foreach (var routine in routines)
        {
            var storedProcSchema = new StoredProcedureSchema
            {
                Name = routine.Name,
                Schema = routine.SchemaOwner,
                Parameters = ExtractStoredProcedureParameters(routine),
                ResultSetColumns = await ExtractStoredProcedureResultSetAsync(routine, connection)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    protected virtual List<StoredProcedureParameterSchema> ExtractStoredProcedureParameters(DatabaseStoredProcedure routine)
    {
        var parameters = new List<StoredProcedureParameterSchema>();

        // DatabaseSchemaReader provides parameter information through Arguments
        foreach (var argument in routine.Arguments ?? [])
        {
            var parameterSchema = new StoredProcedureParameterSchema
            {
                Name = argument.Name ?? "",
                DataType = GetDataTypeString(argument.DatabaseDataType),
                NormalizedDataType = GetNormalizedDataTypeString(argument.DataType, argument.DatabaseDataType),
                IsOutputParameter = argument.Out, // DatabaseSchemaReader exposes input/output information
                MaxLength = GetMaxLength(argument),
                NumericPrecision = GetNumericPrecision(argument),
                NumericScale = GetNumericScale(argument)
            };

            parameters.Add(parameterSchema);
        }

        return parameters;
    }

    protected virtual async Task<List<ResultSetColumnSchema>> ExtractStoredProcedureResultSetAsync(DatabaseStoredProcedure routine, DbConnection connection)
    {
        var resultColumns = new List<ResultSetColumnSchema>();

        // Use the stored procedure analyzer for database-specific result set analysis
        var analyzer = CreateStoredProcedureAnalyzer(connection);
        var parameters = ExtractStoredProcedureParameters(routine);
        
        try
        {
            resultColumns = await analyzer.AnalyzeResultSetAsync(routine.Name, routine.SchemaOwner, parameters);
        }
        catch (Exception)
        {
            // If analysis fails, return empty result set
            // This is common for procedures that don't return result sets or require specific parameters
        }

        return resultColumns;
    }

    protected virtual DefaultConstraintSchema? ExtractDefaultConstraint(DatabaseColumn column)
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

    /// <remarks>
    /// If you get an exception thrown here, you will need to perform an override on this method in the respective database provider.
    /// Then you will need to perform the "dbDataType" checks first to determine the appropriate C# type.
    /// After that call this base method.
    /// </remarks>
    protected virtual string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        return dataType?.NetDataTypeCSharpName ?? throw new InvalidOperationException($"Unable to extract normalized data type for database data type '{dbDataType}'");;
    }

    protected virtual ComputedColumnSchema? ExtractComputedColumn(DatabaseColumn column)
    {
        if (column.ComputedDefinition is null)
        {
            return null;
        }
        
        return new ComputedColumnSchema
        {
            Expression = column.ComputedDefinition
        };
    }

    // Helper methods for extracting length/precision/scale from DatabaseSchemaReader objects
    protected virtual int? GetMaxLength(DatabaseArgument argument)
    {
        return argument.Length > 0 ? argument.Length : null;
    }

    protected virtual int? GetNumericPrecision(DatabaseArgument argument)
    {
        return argument.Precision > 0 ? argument.Precision : null;
    }

    protected virtual int? GetNumericScale(DatabaseArgument argument)
    {
        return argument.Scale > 0 ? argument.Scale : null;
    }

    // Helper methods for DatabaseColumn (separate from DatabaseArgument)
    protected virtual int? GetMaxLength(DatabaseColumn column)
    {
        return column.Length > 0 ? column.Length : null;
    }

    protected virtual int? GetNumericPrecision(DatabaseColumn column)
    {
        return column.Precision > 0 ? column.Precision : null;
    }

    protected virtual int? GetNumericScale(DatabaseColumn column)
    {
        return column.Scale > 0 ? column.Scale : null;
    }
} 