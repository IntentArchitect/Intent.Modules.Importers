using System.Collections.Generic;
using System.Linq;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class TriggerExtractorBase
{
    public abstract List<TriggerSchema> ExtractTableTriggers(DatabaseTable table);
    public abstract List<TriggerSchema> ExtractViewTriggers(DatabaseView view);
}

/// <summary>
/// Base implementation for trigger extraction from database objects
/// </summary>
internal class DefaultTriggerExtractor : TriggerExtractorBase
{
    /// <summary>
    /// Extract trigger schema information from a database table
    /// </summary>
    public override List<TriggerSchema> ExtractTableTriggers(DatabaseTable table)
    {
        var triggers = new HashSet<TriggerSchema>(EqualityComparer<TriggerSchema>.Create(
            (a, b) => string.Equals(a?.Name, b?.Name),
            x => x?.Name?.GetHashCode() ?? 0));

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

        return triggers.ToList();
    }

    /// <summary>
    /// Extract trigger schema information from a database view
    /// </summary>
    public override List<TriggerSchema> ExtractViewTriggers(DatabaseView view)
    {
        var triggers = new HashSet<TriggerSchema>(EqualityComparer<TriggerSchema>.Create(
            (a, b) => string.Equals(a?.Name, b?.Name),
            x => x?.Name?.GetHashCode() ?? 0));

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

        return triggers.ToList();
    }
} 