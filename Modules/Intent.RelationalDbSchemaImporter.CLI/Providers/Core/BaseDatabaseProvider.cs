using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSchemaReader;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using DatabaseSchema = Intent.RelationalDbSchemaImporter.Contracts.DbSchema.DatabaseSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

/// <summary>
/// Base implementation for database providers in the Intent Architect RPC backend.
/// This class provides database schema extraction capabilities that are isolated from Intent Architect
/// due to native library dependencies (Microsoft.Data.SqlClient, Npgsql, etc.).
/// 
/// The provider pattern allows database-specific implementations while maintaining a consistent
/// interface for Intent Architect importer modules to consume via the RPC layer.
/// 
/// All extraction logic is delegated to specialized services to ensure separation of concerns
/// and enable database-specific customization where needed.
/// </summary>
internal abstract class BaseDatabaseProvider
{
    // Service dependencies - can be overridden by derived classes for database-specific implementations
    protected virtual DataTypeMapperBase DataTypeMapper => new DefaultDataTypeMapper();
    protected virtual SystemObjectFilterBase SystemObjectFilter => new DefaultSystemObjectFilter();
    protected virtual ColumnExtractorBase ColumnExtractor => new DefaultColumnExtractor();
    protected virtual IndexExtractorBase IndexExtractor => new DefaultIndexExtractor();
    protected virtual ForeignKeyExtractorBase ForeignKeyExtractor => new DefaultForeignKeyExtractor();
    protected virtual TriggerExtractorBase TriggerExtractor => new DefaultTriggerExtractor();
    protected virtual TableExtractorBase TableExtractor => new DefaultTableExtractor();
    protected virtual ViewExtractorBase ViewExtractor => new DefaultViewExtractor();
    protected virtual StoredProcedureExtractorBase StoredProcedureExtractor => new DefaultStoredProcedureExtractor();

    public abstract DatabaseType SupportedType { get; }
    
    /// <summary>
    /// Creates a database-specific connection. Implementations must provide database-specific connection types
    /// (SqlConnection, NpgsqlConnection, etc.) that include the native dependencies requiring this separation.
    /// </summary>
    protected abstract DbConnection CreateConnection(string connectionString);
    
    /// <summary>
    /// Creates a database-specific dependency resolver for handling table relationships and dependencies.
    /// </summary>
    protected abstract IDependencyResolver CreateDependencyResolver(DbConnection connection);
    
    /// <summary>
    /// Creates a database-specific stored procedure analyzer for extracting result set information.
    /// Different databases have varying approaches to stored procedure/function analysis.
    /// </summary>
    protected abstract IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection);

    /// <summary>
    /// Extracts the complete database schema for Intent Architect domain package creation.
    /// This method orchestrates all extraction through specialized services and is the primary
    /// entry point called by Intent Architect importer modules via the RPC interface.
    /// </summary>
    public async Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportFilterService importFilterService, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll(cancellationToken);

        var schema = new DatabaseSchema
        {
            DatabaseName = connection.Database,
            Tables = [],
            Views = [],
            StoredProcedures = []
        };

        if (importFilterService.ExportTables())
        {
            schema.Tables = await TableExtractor.ExtractTablesAsync(
                databaseSchema, importFilterService, connection,
                SystemObjectFilter, ColumnExtractor, IndexExtractor, 
                ForeignKeyExtractor, TriggerExtractor, DataTypeMapper,
                CreateDependencyResolver(connection));
        }

        if (importFilterService.ExportViews())
        {
            schema.Views = await ViewExtractor.ExtractViewsAsync(
                databaseSchema, importFilterService,
                SystemObjectFilter, ColumnExtractor, TriggerExtractor, DataTypeMapper);
        }

        if (importFilterService.ExportStoredProcedures())
        {
            schema.StoredProcedures = await StoredProcedureExtractor.ExtractStoredProceduresAsync(
                databaseSchema, importFilterService, connection,
                SystemObjectFilter, DataTypeMapper, CreateStoredProcedureAnalyzer(connection));
        }

        return schema;
    }

    /// <summary>
    /// Tests database connectivity for Intent Architect import validation.
    /// This method cannot be overridden to ensure consistent connection handling across all database types.
    /// </summary>
    public async Task TestConnectionAsync(string connectionString, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        
        // Allow database-specific test commands
        await ExecuteConnectionTestAsync(connection, cancellationToken);
    }

    /// <summary>
    /// Gets list of table names for Intent Architect selection interfaces.
    /// This method cannot be overridden to ensure consistent system object filtering across database types.
    /// </summary>
    public async Task<List<string>> GetTableNamesAsync(string connectionString)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        return databaseSchema.Tables
            .Where(table => !SystemObjectFilter.IsSystemObject(table.SchemaOwner, table.Name))
            .Select(t => $"{t.SchemaOwner}.{t.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    /// <summary>
    /// Gets list of view names in the database. This method cannot be overridden to ensure consistent system object filtering.
    /// </summary>
    public async Task<List<string>> GetViewNamesAsync(string connectionString)
    {
        await using var connection = CreateConnection(connectionString);
        await connection.OpenAsync();
        
        var reader = new DatabaseReader(connection);
        var databaseSchema = reader.ReadAll();
        
        return databaseSchema.Views
            .Where(view => !SystemObjectFilter.IsSystemObject(view.SchemaOwner, view.Name))
            .Select(v => $"{v.SchemaOwner}.{v.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    /// <summary>
    /// Gets list of database routine names. This method cannot be overridden to ensure consistent system object filtering.
    /// </summary>
    public async Task<List<string>> GetStoredProcedureNamesAsync(string connectionString)
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
                .Where(sp => !SystemObjectFilter.IsSystemObject(sp.SchemaOwner, sp.Name))
                .Select(sp => $"{sp.SchemaOwner}.{sp.Name}"));
        }
        
        // Add functions (if supported by DatabaseSchemaReader)
        if (databaseSchema.Functions != null)
        {
            routines.AddRange(databaseSchema.Functions
                .Where(func => !SystemObjectFilter.IsSystemObject(func.SchemaOwner, func.Name))
                .Select(func => $"{func.SchemaOwner}.{func.Name}"));
        }
        
        return routines.OrderBy(name => name).ToList();
    }

    /// <summary>
    /// Executes database-specific connection test. Override for custom test commands.
    /// </summary>
    protected virtual async Task ExecuteConnectionTestAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
} 