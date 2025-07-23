namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a computed column in the database.
/// </summary>
public class ComputedColumnSchema
{
    /// <summary>
    /// The SQL expression used to compute the column value.
    /// </summary>
    public string Expression { get; set; } = string.Empty;
    /// <summary>
    /// Whether the computed column is persisted.
    /// </summary>
    public bool IsPersisted { get; set; }
}
