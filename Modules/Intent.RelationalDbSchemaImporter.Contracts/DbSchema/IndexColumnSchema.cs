namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a column in an index.
/// </summary>
public class IndexColumnSchema
{
    /// <summary>
    /// The name of the column.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Whether the column is descending ("Ascending" is the default).
    /// </summary>
    public bool IsDescending { get; set; }
    /// <summary>
    /// Whether the column is included in the index.
    /// </summary>
    public bool IsIncluded { get; set; }
}
