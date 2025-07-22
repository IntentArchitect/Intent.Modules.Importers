using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server database provider implementation
/// </summary>
internal class SqlServerProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.SqlServer;

    protected override DbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    protected override IDependencyResolver CreateDependencyResolver(DbConnection connection)
    {
        // TODO: Implement SQL Server dependency resolver similar to PostgreSQL
        throw new NotImplementedException("SQL Server dependency resolver not yet implemented in the new architecture");
    }

    protected override IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection)
    {
        // TODO: Implement SQL Server stored procedure analyzer similar to PostgreSQL
        throw new NotImplementedException("SQL Server stored procedure analyzer not yet implemented in the new architecture");
    }

    protected override async Task ExecuteConnectionTestAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        // SQL Server specific connection test using SMO
        var sqlConnection = (SqlConnection)connection;
        var server = new Server(new ServerConnection(sqlConnection));
        var database = server.Databases[connection.Database];
        database.ExecuteWithResults("SELECT 1");
    }

    // NOTE: The following methods were moved to base class for consistency
    // They can be removed from here since they now use the template method pattern
    
    // Legacy implementation - can be extracted for reference:
    // The old ExtractSchemaAsync method used DatabaseSchemaExtractor directly
    // This should be migrated to override the protected methods in BaseDatabaseProvider
    // when the SQL Server provider is fully migrated to the new architecture

    // TODO: Override protected virtual methods from BaseDatabaseProvider for SQL Server customizations
    // - ExtractTablesAsync (if needed)
    // - ExtractStoredProceduresAsync (if needed)
    // - GetNormalizedDataTypeString (for SQL Server types)
    // - IsSystemObject (for SQL Server system objects)
} 