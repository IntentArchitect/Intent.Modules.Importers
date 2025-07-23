namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a view in the database.
/// </summary>
public class ViewSchema
{
    /// <summary>
    /// The name of the view.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the view.
    /// </summary>
    public string Schema { get; set; } = string.Empty;
    /// <summary>
    /// The columns in the view.
    /// </summary>
    public List<ColumnSchema> Columns { get; set; } = [];

    /// <summary>
    /// A collection of triggers associated with the database view.
    /// </summary>
    public List<TriggerSchema> Triggers { get; set; } = [];
    /// <summary>
    /// Additional metadata about the view.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
