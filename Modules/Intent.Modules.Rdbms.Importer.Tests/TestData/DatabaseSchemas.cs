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

    public static DatabaseSchema WithCustomersAndOrders() => new()
    {
        DatabaseName = "TestDatabase",
        Tables = [Tables.SimpleCustomers(), Tables.OrdersWithCustomerFk()],
        Views = [],
        StoredProcedures = []
    };
}
