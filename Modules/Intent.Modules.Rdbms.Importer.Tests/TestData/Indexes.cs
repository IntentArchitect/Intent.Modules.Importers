using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating IndexSchema test objects
/// </summary>
internal static class Indexes
{
    public static IndexSchema UniqueEmailIndex() => new()
    {
        Name = "IX_Customers_Email_Unique",
        IsUnique = true,
        IsClustered = false,
        HasFilter = false,
        Columns =
        [
            new IndexColumnSchema { Name = "Email", IsDescending = false, IsIncluded = false }
        ],
        Metadata = new()
    };

    public static IndexSchema ClusteredPrimaryKeyIndex() => new()
    {
        Name = "PK_Orders",
        IsUnique = true,
        IsClustered = true,
        HasFilter = false,
        Columns =
        [
            new IndexColumnSchema { Name = "Id", IsDescending = false, IsIncluded = false }
        ],
        Metadata = new()
    };

    public static IndexSchema NonClusteredCompositeIndex() => new()
    {
        Name = "IX_Orders_CustomerId_OrderDate",
        IsUnique = false,
        IsClustered = false,
        HasFilter = false,
        Columns =
        [
            new IndexColumnSchema { Name = "CustomerId", IsDescending = false, IsIncluded = false },
            new IndexColumnSchema { Name = "OrderDate", IsDescending = true, IsIncluded = false }
        ],
        Metadata = new()
    };

    public static IndexSchema IndexWithIncludedColumns() => new()
    {
        Name = "IX_Orders_Status_Include",
        IsUnique = false,
        IsClustered = false,
        HasFilter = false,
        Columns =
        [
            new IndexColumnSchema { Name = "Status", IsDescending = false, IsIncluded = false },
            new IndexColumnSchema { Name = "TotalAmount", IsDescending = false, IsIncluded = true },
            new IndexColumnSchema { Name = "OrderDate", IsDescending = false, IsIncluded = true }
        ],
        Metadata = new()
    };

    public static IndexSchema FilteredIndex() => new()
    {
        Name = "IX_Orders_Active",
        IsUnique = false,
        IsClustered = false,
        HasFilter = true,
        FilterDefinition = "WHERE Status = 'Active'",
        Columns =
        [
            new IndexColumnSchema { Name = "OrderDate", IsDescending = true, IsIncluded = false }
        ],
        Metadata = new()
    };

    public static IndexSchema DescendingIndex() => new()
    {
        Name = "IX_Products_Price_Desc",
        IsUnique = false,
        IsClustered = false,
        HasFilter = false,
        Columns =
        [
            new IndexColumnSchema { Name = "Price", IsDescending = true, IsIncluded = false }
        ],
        Metadata = new()
    };
}
