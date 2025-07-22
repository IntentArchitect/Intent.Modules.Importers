using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class ViewExtractorBase
{
    public abstract Task<List<ViewSchema>> ExtractViewsAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        SystemObjectFilterBase systemObjectFilter,
        ColumnExtractorBase columnExtractor,
        TriggerExtractorBase triggerExtractor,
        DataTypeMapperBase dataTypeMapper);
}

/// <summary>
/// Default implementation for view extraction from databases
/// </summary>
internal class DefaultViewExtractor : ViewExtractorBase
{
    public override Task<List<ViewSchema>> ExtractViewsAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        SystemObjectFilterBase systemObjectFilter,
        ColumnExtractorBase columnExtractor,
        TriggerExtractorBase triggerExtractor,
        DataTypeMapperBase dataTypeMapper)
    {
        var views = new List<ViewSchema>();
        var filteredViews = databaseSchema.Views
            .Where(view => !systemObjectFilter.IsSystemObject(view.SchemaOwner, view.Name) && importFilterService.ExportView(view.SchemaOwner, view.Name))
            .ToArray();

        var progressOutput = ConsoleOutput.CreateSectionProgress("Views", filteredViews.Length);
        
        foreach (var view in filteredViews)
        {
            progressOutput.OutputNext(view.Name);
            
            var viewSchema = new ViewSchema
            {
                Name = view.Name,
                Schema = view.SchemaOwner,
                Columns = columnExtractor.ExtractViewColumns(view, importFilterService, dataTypeMapper),
                Triggers = triggerExtractor.ExtractViewTriggers(view)
            };

            views.Add(viewSchema);
        }

        return Task.FromResult(views);
    }
} 