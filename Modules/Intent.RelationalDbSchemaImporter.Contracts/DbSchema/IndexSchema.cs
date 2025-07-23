namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents an index in the database.
/// </summary>
public class IndexSchema
{
    /// <summary>
    /// The name of the index.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Whether the index is unique.
    /// </summary>
    public bool IsUnique { get; set; }
    /// <summary>
    /// Whether the index is clustered.
    /// </summary>
    public bool IsClustered { get; set; }
    /// <summary>
    /// Whether the index has a filter.
    /// </summary>
    public bool HasFilter { get; set; }
    /// <summary>
    /// The filter definition of the index.
    /// </summary>
    public string? FilterDefinition { get; set; }
    /// <summary>
    /// The columns in the index.
    /// </summary>
    public List<IndexColumnSchema> Columns { get; set; } = [];
    /// <summary>
    /// Additional metadata about the index.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
