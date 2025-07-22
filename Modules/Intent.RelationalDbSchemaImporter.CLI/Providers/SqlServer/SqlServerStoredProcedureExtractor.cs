using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific stored procedure extractor using DatabaseSchemaReader + custom analysis
/// Migrated from DatabaseSchemaExtractor to work with the new service architecture
/// </summary>
internal class SqlServerStoredProcedureExtractor : DefaultStoredProcedureExtractor
{
    public override async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection,
        SystemObjectFilterBase systemObjectFilter,
        DataTypeMapperBase dataTypeMapper,
        IStoredProcedureAnalyzer analyzer)
    {
        var storedProcedures = new List<StoredProcedureSchema>();

        if (!importFilterService.ExportStoredProcedures())
        {
            return storedProcedures;
        }

        var progressOutput = ConsoleOutput.CreateSectionProgress("Stored Procedures", databaseSchema.StoredProcedures.Count);

        foreach (var procedure in databaseSchema.StoredProcedures)
        {
            var schema = procedure.SchemaOwner ?? "dbo";
            var name = procedure.Name;

            progressOutput.OutputNext($"{schema}.{name}");

            // Apply system object filtering (migrated from DatabaseSchemaExtractor logic)
            if (systemObjectFilter.IsSystemObject(schema, name))
                continue;

            // Convert DatabaseSchemaReader parameters to contract parameters with SQL types
            var parameters = procedure.Arguments.Select(arg => new StoredProcedureParameterSchema
            {
                Name = arg.Name,
                DataType = dataTypeMapper.GetDataTypeString(arg.DataType?.TypeName), // Keep SQL type
                NormalizedDataType = dataTypeMapper.GetNormalizedDataTypeString(arg.DataType, arg.DataType?.TypeName ?? ""),
                IsOutputParameter = arg.Out,
                MaxLength = arg.Length,
                NumericPrecision = arg.Precision,
                NumericScale = arg.Scale
            }).ToList();

            // Use SQL Server-specific analyzer to get result set metadata
            var resultSetColumns = await analyzer.AnalyzeResultSetAsync(name, schema, parameters);

            var storedProcSchema = new StoredProcedureSchema
            {
                Name = name,
                Schema = schema,
                Parameters = parameters,
                ResultSetColumns = resultSetColumns
            };

            storedProcedures.Add(storedProcSchema);
        }
        return storedProcedures;
    }
} 