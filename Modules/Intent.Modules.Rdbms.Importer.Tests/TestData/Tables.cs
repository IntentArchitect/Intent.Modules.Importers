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

    public static ColumnSchema Column(
        string name,
        SqlDbType type,
        bool isPrimaryKey = false,
        bool isNullable = false,
        int? length = null,
        int? precision = null,
        int? scale = null)
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
            NumericScale = scale
        };
    }

    private static string MapToLanguageType(SqlDbType type) => type switch
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
