namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a default constraint in the database.
/// </summary>
public class DefaultConstraintSchema
{
    /// <summary>
    /// The text of the default constraint.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
