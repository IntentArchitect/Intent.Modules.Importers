using System.Data;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating TableSchema test objects
/// </summary>
internal static class Tables
{
    public static TableSchema SimpleUsers() => new()
    {
        Schema = "dbo",
        Name = "Users",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false),
            Column("Email", SqlDbType.NVarChar, length: 255, isNullable: true)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema SimpleCustomers() => new()
    {
        Schema = "dbo",
        Name = "Customers",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Email", SqlDbType.NVarChar, length: 255)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema OrdersWithCustomerFk() => new()
    {
        Schema = "dbo",
        Name = "Orders",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("CustomerId", SqlDbType.Int, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_Orders_Customers",
                TableName = "Orders",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Customers",
                Columns =
                [
                    new()
                    {
                        Name = "CustomerId",
                        ReferencedColumnName = "Id"
                    }
                ]
            }
        ],
        Indexes = []
    };

    public static TableSchema CustomerWithAddress() => new()
    {
        Schema = "dbo",
        Name = "Customers",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Email", SqlDbType.NVarChar, length: 255),
            Column("Address", SqlDbType.NVarChar, length: 500, isNullable: true) // New column
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema Product() => new()
    {
        Schema = "dbo",
        Name = "Products",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 200),
            Column("Price", SqlDbType.Decimal, precision: 18, scale: 2)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema CustomerInSchema2() => new()
    {
        Schema = "schema2",
        Name = "Customer",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema TableWithConstraints() => new()
    {
        Schema = "dbo",
        Name = "Orders",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("OrderNumber", SqlDbType.NVarChar, length: 50, isNullable: false), // Text constraint (length)
            Column("Status", SqlDbType.NVarChar, length: 20, isNullable: false, defaultValue: "'Pending'"), // Default constraint
            Column("Quantity", SqlDbType.Int, isNullable: false, defaultValue: "1"), // Default constraint
            Column("UnitPrice", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: false), // Decimal constraint (precision/scale)
            Column("TotalAmount", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: false, computedExpression: "([Quantity] * [UnitPrice])", isPersisted: true), // Computed constraint
            Column("CreatedDate", SqlDbType.DateTime2, isNullable: false, defaultValue: "getutcdate()") // Default constraint with function
        ],
        ForeignKeys = [],
        Indexes = [],
        Triggers = []
    };

    public static TableSchema TableWithIndexesAndTriggers() => new()
    {
        Schema = "dbo",
        Name = "Products",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 200, isNullable: false),
            Column("SKU", SqlDbType.NVarChar, length: 50, isNullable: false),
            Column("Price", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: false),
            Column("Status", SqlDbType.NVarChar, length: 20, isNullable: false)
        ],
        ForeignKeys = [],
        Indexes =
        [
            Indexes.UniqueEmailIndex(),
            Indexes.NonClusteredCompositeIndex()
        ],
        Triggers =
        [
            Triggers.AfterInsertTrigger("Products"),
            Triggers.AfterUpdateTrigger("Products")
        ]
    };

    public static ColumnSchema Column(
        string name,
        SqlDbType type,
        bool isPrimaryKey = false,
        bool isNullable = false,
        int? length = null,
        int? precision = null,
        int? scale = null,
        string? defaultValue = null,
        string? computedExpression = null,
        bool isPersisted = false)
    {
        var dbDataType = type.ToString().ToLower();
        var languageDataType = MapToLanguageType(type);

        return new ColumnSchema
        {
            Name = name,
            DbDataType = dbDataType,
            LanguageDataType = languageDataType,
            IsNullable = isNullable,
            IsPrimaryKey = isPrimaryKey,
            IsIdentity = isPrimaryKey, // Simplified assumption for tests
            MaxLength = length,
            NumericPrecision = precision,
            NumericScale = scale,
            DefaultConstraint = defaultValue != null ? new DefaultConstraintSchema { Text = defaultValue } : null,
            ComputedColumn = computedExpression != null ? new ComputedColumnSchema { Expression = computedExpression, IsPersisted = isPersisted } : null
        };
    }

    public static string MapToLanguageType(SqlDbType type) => type switch
    {
        SqlDbType.Int => "int",
        SqlDbType.BigInt => "long",
        SqlDbType.SmallInt => "short",
        SqlDbType.TinyInt => "byte",
        SqlDbType.Bit => "bool",
        SqlDbType.Decimal => "decimal",
        SqlDbType.Money => "decimal",
        SqlDbType.Float => "double",
        SqlDbType.Real => "float",
        SqlDbType.DateTime => "datetime",
        SqlDbType.DateTime2 => "datetime",
        SqlDbType.Date => "date",
        SqlDbType.Time => "time",
        SqlDbType.NVarChar => "string",
        SqlDbType.VarChar => "string",
        SqlDbType.NChar => "string",
        SqlDbType.Char => "string",
        SqlDbType.UniqueIdentifier => "guid",
        SqlDbType.Binary => "binary",
        SqlDbType.VarBinary => "binary",
        _ => "string"
    };
}
