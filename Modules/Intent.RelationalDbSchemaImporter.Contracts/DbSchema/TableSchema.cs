namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a table in the database.
/// </summary>
public class TableSchema
{
    /// <summary>
    /// The name of the table.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the table.
    /// </summary>
    public string Schema { get; set; } = string.Empty;
    /// <summary>
    /// The columns in the table.
    /// </summary>
    public List<ColumnSchema> Columns { get; set; } = [];
    /// <summary>
    /// The indexes in the table.
    /// </summary>
    public List<IndexSchema> Indexes { get; set; } = [];
    /// <summary>
    /// The foreign keys in the table.
    /// </summary>
    public List<ForeignKeySchema> ForeignKeys { get; set; } = [];
    /// <summary>
    /// The triggers in the table.
    /// </summary>
    public List<TriggerSchema> Triggers { get; set; } = [];
    /// <summary>
    /// Additional metadata about the table.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
