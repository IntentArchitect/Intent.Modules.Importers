using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating DatabaseSchema test objects
/// </summary>
internal static class DatabaseSchemas
{
    public static DatabaseSchema Empty() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithSimpleUsersTable() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleUsers()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithSimpleUsersTableWithStatus() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleUsersWithStatus()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithCustomersAndOrders() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.OrdersWithCustomerFk()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithCustomerAndNewProduct()
    {
        var schema = WithSimpleCustomersTable();
        schema.Tables.Add(Tables.Product());
        return schema;
    }

    private static DatabaseSchema WithSimpleCustomersTable() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithCustomerWithNewAddressColumn() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.CustomerWithAddress()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithCustomerMissingEmailColumn() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [new()
        {
            Schema = "dbo",
            Name = "Customers",
            Columns =
            [
                Tables.Column("Id", System.Data.SqlDbType.Int, isPrimaryKey: true)
                // Email column removed
            ],
            ForeignKeys = [],
            Indexes = []
        }],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithOrderAndExistingCustomerReference() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.OrdersWithCustomerFk()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithMultipleSchemas() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.CustomerInSchema2()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithParentsAndChildrenCompositePK() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.ParentsWithCompositePK(), Tables.ChildrenWithCompositeFk()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithTableWithMultipleFKs() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.FKTable(), Tables.PrimaryTableWithMultipleFks()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithSelfReferencingTable() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SelfReferencingTable()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithLegacyTableNoPK() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.LegacyTableNoPK()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithOrdersAndUniqueIndex() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.OrdersWithUniqueIndex()],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithAspNetIdentityTables() => new()
    {
        DatabaseName = "TestDatabase",
        Tables =
        [
            Tables.AspNetUsers(),
            Tables.AspNetRoles(),
            Tables.AspNetUserRoles(),
            Tables.AspNetUserClaims(),
            Tables.AspNetRoleClaims(),
            Tables.AspNetUserLogins(),
            Tables.AspNetUserTokens()
        ],
        Views = [],
        StoredProcedures = []
    };

    public static DatabaseSchema WithCustomerAndOrderWithoutForeignKey() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.OrdersWithoutCustomerFk()],
        Views = [],
        StoredProcedures = []
    };
}
