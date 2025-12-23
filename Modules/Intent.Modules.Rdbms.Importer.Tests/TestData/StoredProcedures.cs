using System.Data;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating StoredProcedureSchema test objects
/// </summary>
internal static class StoredProcedures
{
    public static StoredProcedureSchema GetCustomerById() => new()
    {
        Schema = "dbo",
        Name = "sp_GetCustomerById",
        Parameters =
        [
            Parameter("@CustomerId", SqlDbType.Int, StoredProcedureParameterDirection.In)
        ],
        ResultSetColumns =
        [
            ResultColumn("Id", SqlDbType.Int, isNullable: false),
            ResultColumn("Email", SqlDbType.NVarChar, length: 255, isNullable: false),
            ResultColumn("Name", SqlDbType.NVarChar, length: 100, isNullable: true)
        ],
        Metadata = new()
    };

    public static StoredProcedureSchema CreateCustomer() => new()
    {
        Schema = "dbo",
        Name = "sp_CreateCustomer",
        Parameters =
        [
            Parameter("@Email", SqlDbType.NVarChar, StoredProcedureParameterDirection.In, length: 255),
            Parameter("@Name", SqlDbType.NVarChar, StoredProcedureParameterDirection.In, length: 100),
            Parameter("@NewCustomerId", SqlDbType.Int, StoredProcedureParameterDirection.Out)
        ],
        ResultSetColumns = [], // No result set, uses output parameter
        Metadata = new()
    };

    public static StoredProcedureSchema UpdateCustomerBalance() => new()
    {
        Schema = "dbo",
        Name = "sp_UpdateCustomerBalance",
        Parameters =
        [
            Parameter("@CustomerId", SqlDbType.Int, StoredProcedureParameterDirection.In),
            Parameter("@Amount", SqlDbType.Decimal, StoredProcedureParameterDirection.In, precision: 18, scale: 2),
            Parameter("@Balance", SqlDbType.Decimal, StoredProcedureParameterDirection.Both, precision: 18, scale: 2)
        ],
        ResultSetColumns = [],
        Metadata = new()
    };

    public static StoredProcedureSchema GetOrdersSummary() => new()
    {
        Schema = "dbo",
        Name = "sp_GetOrdersSummary",
        Parameters =
        [
            Parameter("@StartDate", SqlDbType.DateTime2, StoredProcedureParameterDirection.In),
            Parameter("@EndDate", SqlDbType.DateTime2, StoredProcedureParameterDirection.In)
        ],
        ResultSetColumns =
        [
            ResultColumn("OrderId", SqlDbType.Int, isNullable: false, sourceSchema: "dbo", sourceTable: "Orders"),
            ResultColumn("CustomerId", SqlDbType.Int, isNullable: false, sourceSchema: "dbo", sourceTable: "Orders"),
            ResultColumn("OrderDate", SqlDbType.DateTime2, isNullable: false, sourceSchema: "dbo", sourceTable: "Orders"),
            ResultColumn("TotalAmount", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: false)
        ],
        Metadata = new()
    };

    public static StoredProcedureSchema ProcedureInSchema2() => new()
    {
        Schema = "schema2",
        Name = "sp_GetProducts",
        Parameters = [],
        ResultSetColumns =
        [
            ResultColumn("ProductId", SqlDbType.UniqueIdentifier, isNullable: false),
            ResultColumn("ProductName", SqlDbType.NVarChar, length: 200, isNullable: false),
            ResultColumn("Price", SqlDbType.Decimal, precision: 18, scale: 2, isNullable: false)
        ],
        Metadata = new()
    };

    public static StoredProcedureSchema WithMultipleResultSets() => new()
    {
        Schema = "dbo",
        Name = "sp_GetCustomerOrderDetails",
        Parameters =
        [
            Parameter("@CustomerId", SqlDbType.Int, StoredProcedureParameterDirection.In)
        ],
        // Note: Current schema only supports single result set - using first result set
        ResultSetColumns =
        [
            ResultColumn("Id", SqlDbType.Int, isNullable: false),
            ResultColumn("Email", SqlDbType.NVarChar, length: 255, isNullable: false),
            ResultColumn("TotalOrders", SqlDbType.Int, isNullable: false)
        ],
        Metadata = new()
    };

    public static StoredProcedureParameterSchema Parameter(
        string name,
        SqlDbType type,
        StoredProcedureParameterDirection direction,
        int? length = null,
        int? precision = null,
        int? scale = null)
    {
        var dbDataType = type.ToString().ToLower();
        var languageDataType = Tables.MapToLanguageType(type);

        return new StoredProcedureParameterSchema
        {
            Name = name,
            DbDataType = dbDataType,
            LanguageDataType = languageDataType,
            Direction = direction,
            MaxLength = length,
            NumericPrecision = precision,
            NumericScale = scale,
            Metadata = new()
        };
    }

    public static ResultSetColumnSchema ResultColumn(
        string name,
        SqlDbType type,
        bool isNullable = false,
        int? length = null,
        int? precision = null,
        int? scale = null,
        string? sourceSchema = null,
        string? sourceTable = null)
    {
        var dbDataType = type.ToString().ToLower();
        var languageDataType = Tables.MapToLanguageType(type);

        return new ResultSetColumnSchema
        {
            Name = name,
            DbDataType = dbDataType,
            LanguageDataType = languageDataType,
            IsNullable = isNullable,
            MaxLength = length,
            NumericPrecision = precision,
            NumericScale = scale,
            SourceSchema = sourceSchema,
            SourceTable = sourceTable,
            Metadata = new()
        };
    }

    public static StoredProcedureSchema TestWithOutParam() => new()
    {
        Schema = "dbo",
        Name = "sp_TestWithOutParam",
        Parameters =
        [
            Parameter("@Id", SqlDbType.UniqueIdentifier, StoredProcedureParameterDirection.In),
            Parameter("@ErrorMessage", SqlDbType.NVarChar, StoredProcedureParameterDirection.Out, length: 255)
        ],
        ResultSetColumns =
        [
            ResultColumn("TestResult1", SqlDbType.NVarChar, length: 100, isNullable: false),
            ResultColumn("TestResult2", SqlDbType.NVarChar, length: 100, isNullable: false)
        ],
        Metadata = new()
    };

    public static StoredProcedureSchema GetCustomerNoOutParams() => new()
    {
        Schema = "dbo",
        Name = "sp_GetCustomer",
        Parameters =
        [
            Parameter("@CustomerId", SqlDbType.Int, StoredProcedureParameterDirection.In)
        ],
        ResultSetColumns =
        [
            ResultColumn("Name", SqlDbType.NVarChar, length: 100, isNullable: false)
        ],
        Metadata = new()
    };

    public static StoredProcedureSchema WithUserDefinedTableTypeParameter() => new()
    {
        Schema = "dbo",
        Name = "sp_InsertOrders",
        Parameters =
        [
            new StoredProcedureParameterSchema
            {
                Name = "@OrdersTable",
                Direction = StoredProcedureParameterDirection.In,
                UserDefinedTableType = new UserDefinedTableTypeSchema
                {
                    Schema = "dbo",
                    Name = "OrderTableType",
                    Columns =
                    [
                        new ColumnSchema
                        {
                            Name = "OrderId",
                            DbDataType = "int",
                            LanguageDataType = "int",
                            IsNullable = false,
                            IsPrimaryKey = false,
                            IsIdentity = false
                        },
                        new ColumnSchema
                        {
                            Name = "CustomerId",
                            DbDataType = "int",
                            LanguageDataType = "int",
                            IsNullable = false,
                            IsPrimaryKey = false,
                            IsIdentity = false
                        },
                        new ColumnSchema
                        {
                            Name = "OrderDate",
                            DbDataType = "datetime2",
                            LanguageDataType = "datetime",
                            IsNullable = false,
                            IsPrimaryKey = false,
                            IsIdentity = false
                        },
                        new ColumnSchema
                        {
                            Name = "Total",
                            DbDataType = "decimal",
                            LanguageDataType = "decimal",
                            IsNullable = true,
                            IsPrimaryKey = false,
                            IsIdentity = false,
                            NumericPrecision = 18,
                            NumericScale = 2
                        }
                    ],
                    Metadata = new()
                },
                Metadata = new()
            }
        ],
        ResultSetColumns = [],
        Metadata = new()
    };
}
