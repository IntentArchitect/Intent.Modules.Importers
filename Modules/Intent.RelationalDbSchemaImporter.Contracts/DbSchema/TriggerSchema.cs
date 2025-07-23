namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a trigger in the database.
/// </summary>
public class TriggerSchema
{
    /// <summary>
    /// The name of the trigger.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the parent object.
    /// </summary>
    public string ParentSchema { get; set; } = string.Empty;
    /// <summary>
    /// The name of the parent object.
    /// </summary>
    public string ParentName { get; set; } = string.Empty;
    /// <summary>
    /// The type of the parent object.
    /// </summary>
    /// <remarks>
    /// "Table" or "View"
    /// </remarks>
    public string ParentType { get; set; } = string.Empty;
    /// <summary>
    /// Additional metadata about the trigger.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
