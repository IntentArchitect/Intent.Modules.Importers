using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class TableExtractorBase
{
    public abstract Task<List<TableSchema>> ExtractTablesAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection,
        SystemObjectFilterBase systemObjectFilter,
        ColumnExtractorBase columnExtractor,
        IndexExtractorBase indexExtractor,
        ForeignKeyExtractorBase foreignKeyExtractor,
        TriggerExtractorBase triggerExtractor,
        DataTypeMapperBase dataTypeMapper,
        IDependencyResolver dependencyResolver);
}

internal class DefaultTableExtractor : TableExtractorBase
{
    public override async Task<List<TableSchema>> ExtractTablesAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection,
        SystemObjectFilterBase systemObjectFilter,
        ColumnExtractorBase columnExtractor,
        IndexExtractorBase indexExtractor,
        ForeignKeyExtractorBase foreignKeyExtractor,
        TriggerExtractorBase triggerExtractor,
        DataTypeMapperBase dataTypeMapper,
        IDependencyResolver dependencyResolver)
    {
        var tables = new List<TableSchema>();
        var filteredTables = databaseSchema.Tables
            .Where(table => !systemObjectFilter.IsSystemObject(table.SchemaOwner, table.Name) && importFilterService.ExportTable(table.SchemaOwner, table.Name))
            .ToArray();

        // Handle dependent tables if required
        if (importFilterService.IncludeDependantTables())
        {
            var tableNames = filteredTables.Select(t => $"{t.SchemaOwner}.{t.Name}");
            var dependentTableNames = await dependencyResolver.GetDependentTablesAsync(tableNames);

            var tables1 = filteredTables;
            var dependentTables = databaseSchema.Tables
                .Where(t => dependentTableNames.Contains($"{t.SchemaOwner}.{t.Name}"))
                .Where(t => !tables1.Contains(t))
                .Where(t => importFilterService.ExportDependantTable(t.SchemaOwner, t.Name));
            
            filteredTables = filteredTables.Concat(dependentTables).ToArray();
        }

        var progressOutput = ConsoleOutput.CreateSectionProgress("Tables", filteredTables.Length);
        
        foreach (var table in filteredTables)
        {
            progressOutput.OutputNext(table.Name);
            
            var tableSchema = new TableSchema
            {
                Name = table.Name,
                Schema = table.SchemaOwner,
                Columns = await columnExtractor.ExtractTableColumnsAsync(table, importFilterService, dataTypeMapper, connection),
                Indexes = await indexExtractor.ExtractIndexesAsync(table, importFilterService, connection),
                ForeignKeys = await foreignKeyExtractor.ExtractForeignKeysAsync(table, connection),
                Triggers = triggerExtractor.ExtractTableTriggers(table)
            };

            tables.Add(tableSchema);
        }

        return tables;
    }
} 