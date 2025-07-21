using System.Data.Common;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Npgsql;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL database provider implementation
/// </summary>
internal class PostgreSQLProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.PostgreSQL;

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

    protected override string GetDataTypeString(string? dataTypeName)
    {
        return MapPostgreSQLDataType(dataTypeName);
    }

    /// <summary>
    /// Maps PostgreSQL data types to normalized names
    /// </summary>
    private string MapPostgreSQLDataType(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        return dataTypeName.ToLowerInvariant() switch
        {
            "character varying" => "varchar",
            "character" => "char",
            "text" => "text",
            "integer" => "int",
            "bigint" => "bigint",
            "smallint" => "smallint",
            "decimal" => "decimal",
            "numeric" => "numeric",
            "real" => "real",
            "double precision" => "float",
            "boolean" => "boolean",
            "timestamp without time zone" => "timestamp",
            "timestamp with time zone" => "timestamptz",
            "date" => "date",
            "time without time zone" => "time",
            "time with time zone" => "timetz",
            "uuid" => "uuid",
            "json" => "json",
            "jsonb" => "jsonb",
            "bytea" => "bytea",
            _ => dataTypeName.ToLowerInvariant()
        };
    }
} 