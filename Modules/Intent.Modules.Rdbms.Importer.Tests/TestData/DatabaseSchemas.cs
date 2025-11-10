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

    public static DatabaseSchema WithCustomerAndRiskProfileSharedPrimaryKey() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.PurchaseCustomer(), Tables.CustomerRiskProfileSharedPrimaryKey()],
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

    /// <summary>
    /// Three table scenario for testing inclusive import bug:
    /// - Table A has FK to Table B (still exists)
    /// - Table B previously had FK to Table C (now removed)
    /// - Table C exists independently
    /// This represents the database state after Table B's FK to Table C was removed
    /// </summary>
    public static DatabaseSchema WithTableABCButBToC_ForeignKeyRemoved() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.TableA(), Tables.TableB(), Tables.TableC()],
        Views = [],
        StoredProcedures = []
    };

    /// <summary>
    /// Simulates an inclusive import where only Table B is being imported.
    /// This is used to test that associations from non-imported tables (A→B) are preserved.
    /// </summary>
    public static DatabaseSchema WithOnlyTableB() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.TableB()],
        Views = [],
        StoredProcedures = []
    };

    /// <summary>
    /// Item table with duplicate foreign keys (auto-generated and explicitly named)
    /// </summary>
    public static DatabaseSchema WithItemDuplicateFKs() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.Category(), Tables.ItemWithDuplicateFKs()],
        Views = [],
        StoredProcedures = []
    };

    /// <summary>
    /// Item table with both duplicate FKs using auto-generated names
    /// </summary>
    public static DatabaseSchema WithItemDuplicateAutoGeneratedFKs() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.Category(), Tables.ItemWithDuplicateAutoGeneratedFKs()],
        Views = [],
        StoredProcedures = []
    };

    /// <summary>
    /// Item table with both duplicate FKs using explicitly named conventions
    /// </summary>
    public static DatabaseSchema WithItemDuplicateNamedFKs() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.Category(), Tables.ItemWithDuplicateNamedFKs()],
        Views = [],
        StoredProcedures = []
    };
}
