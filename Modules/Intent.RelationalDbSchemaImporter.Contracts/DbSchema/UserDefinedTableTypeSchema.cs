namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a user-defined table type in the database.
/// </summary>
public class UserDefinedTableTypeSchema
{
    /// <summary>
    /// The name of the user-defined table type.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the user-defined table type.
    /// </summary>
    public string Schema { get; set; } = string.Empty;
    /// <summary>
    /// The columns in the user-defined table type.
    /// </summary>
    public List<ColumnSchema> Columns { get; set; } = [];
    /// <summary>
    /// Additional metadata about the user-defined table type.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
