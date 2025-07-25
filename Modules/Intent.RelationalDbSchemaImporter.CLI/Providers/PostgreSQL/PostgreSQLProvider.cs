using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Npgsql;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL database provider implementation for the Intent Architect RPC backend.
/// This provider uses Npgsql which contains native library dependencies that cannot be
/// included in Intent Architect modules, necessitating this separate executable.
/// 
/// Provides PostgreSQL-specific implementations for schema extraction, function analysis,
/// and other database operations required by Intent Architect importer modules.
/// </summary>
internal class PostgreSQLProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.PostgreSQL;

    protected override DataTypeMapperBase DataTypeMapper => new PostgreSqlDataTypeMapper();
    protected override SystemObjectFilterBase SystemObjectFilter => new PostgreSQLSystemObjectFilter();
    protected override IndexExtractorBase IndexExtractor => new PostgreSQLIndexExtractor();

    protected override DbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }

    protected override IDependencyResolver CreateDependencyResolver(DbConnection connection)
    {
        return new PostgreSQLDependencyResolver(connection);
    }

    protected override IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection)
    {
        return new PostgreSQLStoredProcedureAnalyzer(connection);
    }
}