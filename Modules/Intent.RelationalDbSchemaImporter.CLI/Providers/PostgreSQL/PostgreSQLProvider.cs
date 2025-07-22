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
/// PostgreSQL database provider implementation
/// </summary>
internal class PostgreSQLProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.PostgreSQL;

    // Override to use PostgreSQL-specific services
    protected override DataTypeMapperBase DataTypeMapper => new PostgreSqlDataTypeMapper();
    protected override SystemObjectFilterBase SystemObjectFilter => new PostgreSQLSystemObjectFilter();
    protected override IndexExtractorBase IndexExtractor => new PostgreSQLIndexExtractor();
    protected override ForeignKeyExtractorBase ForeignKeyExtractor => new PostgreSQLForeignKeyExtractor();
    protected override StoredProcedureExtractorBase StoredProcedureExtractor => new PostgreSQLStoredProcedureExtractor();

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

    // All extraction logic moved to PostgreSQL-specific services
}