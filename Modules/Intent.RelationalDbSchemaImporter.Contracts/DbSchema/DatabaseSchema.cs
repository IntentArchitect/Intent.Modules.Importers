namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents the complete schema of a database.
/// </summary>
public class DatabaseSchema
{
    /// <summary>
    /// The name of the database.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;
    /// <summary>
    /// The tables in the database.
    /// </summary>
    public List<TableSchema> Tables { get; set; } = [];
    /// <summary>
    /// The views in the database.
    /// </summary>
    public List<ViewSchema> Views { get; set; } = [];
    /// <summary>
    /// The stored procedures in the database.
    /// </summary>
    public List<StoredProcedureSchema> StoredProcedures { get; set; } = [];
    /// <summary>
    /// Identified custom types used by other Database objects.
    /// </summary>
    public List<UserDefinedTableTypeSchema> UserDefinedTableTypes { get; set; } = [];
}
