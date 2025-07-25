using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Data.SqlClient;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server database provider implementation for the Intent Architect RPC backend.
/// This provider uses Microsoft.Data.SqlClient which contains native library dependencies
/// that cannot be included in Intent Architect modules, necessitating this separate executable.
/// 
/// Provides SQL Server-specific implementations for schema extraction, stored procedure analysis,
/// and other database operations required by Intent Architect importer modules.
/// </summary>
internal class SqlServerProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.SqlServer;

    protected override DataTypeMapperBase DataTypeMapper => new SqlServerDataTypeMapper();
    protected override SystemObjectFilterBase SystemObjectFilter => new SqlServerSystemObjectFilter();
    protected override ColumnExtractorBase ColumnExtractor => new SqlServerColumnExtractor();
    protected override IndexExtractorBase IndexExtractor => new SqlServerIndexExtractor();

    protected override DbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    protected override IDependencyResolver CreateDependencyResolver(DbConnection connection)
    {
        return new SqlServerDependencyResolver(connection);
    }

    protected override IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection)
    {
        return new SqlServerStoredProcedureAnalyzer(connection);
    }

    protected override async Task ExecuteConnectionTestAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        // Simple SQL Server connection test
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        await command.ExecuteScalarAsync(cancellationToken);
    }

}