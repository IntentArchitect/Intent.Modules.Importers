using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating StereotypePersistable test objects
/// </summary>
internal static class Stereotypes
{
    public static StereotypePersistable Table(string schema, string tableName) => new()
    {
        Name = "Table",
        DefinitionId = "table-stereotype-id",
        Properties = new List<StereotypePropertyPersistable>
        {
            Property("Schema", schema),
            Property("Name", tableName)
        }
    };

    public static StereotypePersistable Column(
        string? type = null,
        int? precision = null,
        int? scale = null,
        int? length = null,
        bool isNullable = false)
    {
        var properties = new List<StereotypePropertyPersistable>
        {
            Property("Type", type ?? "nvarchar"),
            Property("IsNullable", isNullable.ToString())
        };

        if (precision.HasValue)
            properties.Add(Property("Precision", precision.Value.ToString()));
        if (scale.HasValue)
            properties.Add(Property("Scale", scale.Value.ToString()));
        if (length.HasValue)
            properties.Add(Property("Length", length.Value.ToString()));

        return new StereotypePersistable
        {
            Name = "Column",
            DefinitionId = "column-stereotype-id",
            Properties = properties
        };
    }

    public static StereotypePersistable PrimaryKey() => new()
    {
        Name = "Primary Key",
        DefinitionId = "pk-stereotype-id",
        Properties = new List<StereotypePropertyPersistable>()
    };

    public static StereotypePersistable ForeignKey(string columnName) => new()
    {
        Name = "Foreign Key",
        DefinitionId = "fk-stereotype-id",
        Properties = new List<StereotypePropertyPersistable>
        {
            Property("Column Name", columnName)
        }
    };

    public static StereotypePersistable View(string schema) => new()
    {
        Name = "View",
        DefinitionId = "view-stereotype-id",
        Properties = new List<StereotypePropertyPersistable>
        {
            Property("Schema", schema)
        }
    };

    public static StereotypePropertyPersistable Property(string name, string value) => new()
    {
        Name = name,
        Value = value,
        IsActive = true
    };
}
