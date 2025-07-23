namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a foreign key in the database.
/// </summary>
public class ForeignKeySchema
{
    /// <summary>
    /// The name of the foreign key.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The name of the current table.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the referenced table.
    /// </summary>
    public string ReferencedTableSchema { get; set; } = string.Empty;
    /// <summary>
    /// The name of the referenced table.
    /// </summary>
    public string ReferencedTableName { get; set; } = string.Empty;
    /// <summary>
    /// The columns in the foreign key.
    /// </summary>
    public List<ForeignKeyColumnSchema> Columns { get; set; } = [];
    /// <summary>
    /// Additional metadata about the foreign key.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents a column in a foreign key.
/// </summary>
public class ForeignKeyColumnSchema
{
    /// <summary>
    /// The name of the column in the current table.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The name of the referenced column in the referenced table.
    /// </summary>
    public string ReferencedColumnName { get; set; } = string.Empty;
}
