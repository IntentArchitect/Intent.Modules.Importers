using System.Data;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating ViewSchema test objects
/// </summary>
internal static class Views
{
    public static ViewSchema SimpleCustomerView() => new()
    {
        Schema = "dbo",
        Name = "vw_Customers",
        Columns =
        [
            Tables.Column("Id", SqlDbType.Int),
            Tables.Column("Email", SqlDbType.NVarChar, length: 255),
            Tables.Column("Name", SqlDbType.NVarChar, length: 100)
        ],
        Triggers = [],
        Metadata = new()
    };

    public static ViewSchema CustomerOrdersSummaryView() => new()
    {
        Schema = "dbo",
        Name = "vw_CustomerOrdersSummary",
        Columns =
        [
            Tables.Column("CustomerId", SqlDbType.Int),
            Tables.Column("CustomerName", SqlDbType.NVarChar, length: 100),
            Tables.Column("TotalOrders", SqlDbType.Int),
            Tables.Column("TotalAmount", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: true)
        ],
        Triggers = [],
        Metadata = new()
    };

    public static ViewSchema ViewWithTrigger() => new()
    {
        Schema = "dbo",
        Name = "vw_ActiveUsers",
        Columns =
        [
            Tables.Column("UserId", SqlDbType.Int),
            Tables.Column("UserName", SqlDbType.NVarChar, length: 100),
            Tables.Column("IsActive", SqlDbType.Bit)
        ],
        Triggers =
        [
            Triggers.SimpleInsteadOfInsertTrigger("vw_ActiveUsers")
        ],
        Metadata = new()
    };

    public static ViewSchema ViewInSchema2() => new()
    {
        Schema = "schema2",
        Name = "vw_Products",
        Columns =
        [
            Tables.Column("ProductId", SqlDbType.UniqueIdentifier),
            Tables.Column("ProductName", SqlDbType.NVarChar, length: 200),
            Tables.Column("Price", SqlDbType.Decimal, precision: 18, scale: 2)
        ],
        Triggers = [],
        Metadata = new()
    };
}
