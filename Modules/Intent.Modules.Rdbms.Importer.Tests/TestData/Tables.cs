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

    public static TableSchema SimpleUsersWithStatus() => new()
    {
        Schema = "dbo",
        Name = "Users",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false),
            Column("Email", SqlDbType.NVarChar, length: 255, isNullable: true),
            Column("Status", SqlDbType.Int, isNullable: false) // Status column as int in database
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

    public static TableSchema PurchaseCustomer() => new()
    {
        Schema = "purchase",
        Name = "Customer",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true, isNullable: false),
            Column("Email", SqlDbType.NVarChar, length: 250, isNullable: true),
            Column("PhoneNumber", SqlDbType.NVarChar, length: 50, isNullable: true)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema CustomerRiskProfileSharedPrimaryKey() => new()
    {
        Schema = "Customer",
        Name = "RiskProfile",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true, isNullable: false),
            Column("LastBureauDate", SqlDbType.Date, isNullable: true),
            Column("CreditCheckPassed", SqlDbType.Bit, isNullable: true)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_RiskProfile_Customer",
                TableName = "RiskProfile",
                ReferencedTableSchema = "purchase",
                ReferencedTableName = "Customer",
                Columns =
                [
                    new()
                    {
                        Name = "Id",
                        ReferencedColumnName = "Id"
                    }
                ]
            }
        ],
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

    public static TableSchema OrdersWithoutCustomerFk() => new()
    {
        Schema = "dbo",
        Name = "Orders",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("CustomerId", SqlDbType.Int, isNullable: false)
        ],
        ForeignKeys = [], // No foreign keys - FK has been removed
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

    public static TableSchema ParentsWithCompositePK() => new()
    {
        Schema = "dbo",
        Name = "Parents",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true, isNullable: false),
            Column("Id2", SqlDbType.UniqueIdentifier, isPrimaryKey: true, isNullable: false),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema ChildrenWithCompositeFk() => new()
    {
        Schema = "dbo",
        Name = "Children",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true),
            Column("ParentId", SqlDbType.UniqueIdentifier, isNullable: false),
            Column("ParentId2", SqlDbType.UniqueIdentifier, isNullable: false),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_Children_Parents",
                TableName = "Children",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Parents",
                Columns =
                [
                    new() { Name = "ParentId", ReferencedColumnName = "Id" },
                    new() { Name = "ParentId2", ReferencedColumnName = "Id2" }
                ]
            }
        ],
        Indexes = []
    };

    public static TableSchema FKTable() => new()
    {
        Schema = "dbo",
        Name = "FKTable",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema PrimaryTableWithMultipleFks() => new()
    {
        Schema = "dbo",
        Name = "PrimaryTable",
        Columns =
        [
            Column("PrimaryTableId", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false),
            Column("FKTableId1", SqlDbType.Int, isNullable: false),
            Column("FKTableId2", SqlDbType.Int, isNullable: true),
            Column("FKAsTableId3", SqlDbType.Int, isNullable: true),
            Column("FKTryTableId4", SqlDbType.Int, isNullable: true),
            Column("FKThisTableId5", SqlDbType.Int, isNullable: true)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_PrimaryTable_FKTable_Id1",
                TableName = "PrimaryTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "FKTable",
                Columns = [new() { Name = "FKTableId1", ReferencedColumnName = "Id" }]
            },
            new()
            {
                Name = "FK_PrimaryTable_FKTable_Id2",
                TableName = "PrimaryTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "FKTable",
                Columns = [new() { Name = "FKTableId2", ReferencedColumnName = "Id" }]
            },
            new()
            {
                Name = "FK_PrimaryTable_FKTable_Id3",
                TableName = "PrimaryTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "FKTable",
                Columns = [new() { Name = "FKAsTableId3", ReferencedColumnName = "Id" }]
            },
            new()
            {
                Name = "FK_PrimaryTable_FKTable_Id4",
                TableName = "PrimaryTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "FKTable",
                Columns = [new() { Name = "FKTryTableId4", ReferencedColumnName = "Id" }]
            },
            new()
            {
                Name = "FK_PrimaryTable_FKTable_Id5",
                TableName = "PrimaryTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "FKTable",
                Columns = [new() { Name = "FKThisTableId5", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema SelfReferencingTable() => new()
    {
        Schema = "dbo",
        Name = "SelfReferenceTable",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true),
            Column("ParentId", SqlDbType.UniqueIdentifier, isNullable: true),
            Column("Name", SqlDbType.NVarChar, length: 100, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_SelfReferenceTable_SelfReferenceTable",
                TableName = "SelfReferenceTable",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "SelfReferenceTable",
                Columns = [new() { Name = "ParentId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema LegacyTableNoPK() => new()
    {
        Schema = "dbo",
        Name = "Legacy_Table",
        Columns =
        [
            Column("LegacyID", SqlDbType.Int, isPrimaryKey: false), // No PK
            Column("Name", SqlDbType.NVarChar, length: 100),
            Column("BadDate", SqlDbType.DateTime, isNullable: false)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema OrdersWithUniqueIndex() => new()
    {
        Schema = "dbo",
        Name = "Orders",
        Columns =
        [
            Column("Id", SqlDbType.UniqueIdentifier, isPrimaryKey: true),
            Column("CustomerId", SqlDbType.UniqueIdentifier, isNullable: false),
            Column("RefNo", SqlDbType.NVarChar, length: 256, isNullable: false),
            Column("OrderDate", SqlDbType.DateTime, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_Orders_Customers",
                TableName = "Orders",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Customers",
                Columns = [new() { Name = "CustomerId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes =
        [
            new IndexSchema
            {
                Name = "IX_Orders_RefNo",
                IsUnique = true,
                IsClustered = false,
                HasFilter = false,
                Columns =
                [
                    new IndexColumnSchema { Name = "RefNo", IsDescending = false, IsIncluded = false }
                ],
                Metadata = new()
            },
            new IndexSchema
            {
                Name = "IX_Orders_CustomerId",
                IsUnique = false,
                IsClustered = false,
                HasFilter = false,
                Columns =
                [
                    new IndexColumnSchema { Name = "CustomerId", IsDescending = false, IsIncluded = false },
                    new IndexColumnSchema { Name = "OrderDate", IsDescending = false, IsIncluded = true }
                ],
                Metadata = new()
            }
        ]
    };

    // ASP.NET Identity Tables
    public static TableSchema AspNetUsers() => new()
    {
        Schema = "dbo",
        Name = "AspNetUsers",
        Columns =
        [
            Column("Id", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("UserName", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("NormalizedUserName", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("Email", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("NormalizedEmail", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("EmailConfirmed", SqlDbType.Bit, isNullable: false),
            Column("PasswordHash", SqlDbType.NVarChar, isNullable: true),
            Column("SecurityStamp", SqlDbType.NVarChar, isNullable: true),
            Column("ConcurrencyStamp", SqlDbType.NVarChar, isNullable: true),
            Column("PhoneNumber", SqlDbType.NVarChar, isNullable: true),
            Column("PhoneNumberConfirmed", SqlDbType.Bit, isNullable: false),
            Column("TwoFactorEnabled", SqlDbType.Bit, isNullable: false),
            Column("LockoutEnd", SqlDbType.DateTime, isNullable: true),
            Column("LockoutEnabled", SqlDbType.Bit, isNullable: false),
            Column("AccessFailedCount", SqlDbType.Int, isNullable: false)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema AspNetRoles() => new()
    {
        Schema = "dbo",
        Name = "AspNetRoles",
        Columns =
        [
            Column("Id", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("Name", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("NormalizedName", SqlDbType.NVarChar, length: 256, isNullable: true),
            Column("ConcurrencyStamp", SqlDbType.NVarChar, isNullable: true)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    public static TableSchema AspNetUserRoles() => new()
    {
        Schema = "dbo",
        Name = "AspNetUserRoles",
        Columns =
        [
            Column("UserId", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("RoleId", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_AspNetUserRoles_AspNetUsers",
                TableName = "AspNetUserRoles",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetUsers",
                Columns = [new() { Name = "UserId", ReferencedColumnName = "Id" }]
            },
            new()
            {
                Name = "FK_AspNetUserRoles_AspNetRoles",
                TableName = "AspNetUserRoles",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetRoles",
                Columns = [new() { Name = "RoleId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema AspNetUserClaims() => new()
    {
        Schema = "dbo",
        Name = "AspNetUserClaims",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true, isNullable: false),
            Column("UserId", SqlDbType.NVarChar, length: 450, isNullable: false),
            Column("ClaimType", SqlDbType.NVarChar, isNullable: true),
            Column("ClaimValue", SqlDbType.NVarChar, isNullable: true)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_AspNetUserClaims_AspNetUsers",
                TableName = "AspNetUserClaims",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetUsers",
                Columns = [new() { Name = "UserId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema AspNetRoleClaims() => new()
    {
        Schema = "dbo",
        Name = "AspNetRoleClaims",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true, isNullable: false),
            Column("RoleId", SqlDbType.NVarChar, length: 450, isNullable: false),
            Column("ClaimType", SqlDbType.NVarChar, isNullable: true),
            Column("ClaimValue", SqlDbType.NVarChar, isNullable: true)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_AspNetRoleClaims_AspNetRoles",
                TableName = "AspNetRoleClaims",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetRoles",
                Columns = [new() { Name = "RoleId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema AspNetUserLogins() => new()
    {
        Schema = "dbo",
        Name = "AspNetUserLogins",
        Columns =
        [
            Column("LoginProvider", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("ProviderKey", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("ProviderDisplayName", SqlDbType.NVarChar, isNullable: true),
            Column("UserId", SqlDbType.NVarChar, length: 450, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_AspNetUserLogins_AspNetUsers",
                TableName = "AspNetUserLogins",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetUsers",
                Columns = [new() { Name = "UserId", ReferencedColumnName = "Id" }]
            }
        ],
        Indexes = []
    };

    public static TableSchema AspNetUserTokens() => new()
    {
        Schema = "dbo",
        Name = "AspNetUserTokens",
        Columns =
        [
            Column("UserId", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("LoginProvider", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("Name", SqlDbType.NVarChar, length: 450, isPrimaryKey: true, isNullable: false),
            Column("Value", SqlDbType.NVarChar, isNullable: true)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_AspNetUserTokens_AspNetUsers",
                TableName = "AspNetUserTokens",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "AspNetUsers",
                Columns = [new() { Name = "UserId", ReferencedColumnName = "Id" }]
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

    /// <summary>
    /// Table A in the three-table scenario for testing inclusive import bug.
    /// Has FK to Table B (this FK should remain during inclusive import of Table B).
    /// </summary>
    public static TableSchema TableA() => new()
    {
        Schema = "dbo",
        Name = "TableA",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 100),
            Column("TableBId", SqlDbType.Int, isNullable: false)
        ],
        ForeignKeys =
        [
            new()
            {
                Name = "FK_TableA_TableB",
                TableName = "TableA",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "TableB",
                Columns =
                [
                    new()
                    {
                        Name = "TableBId",
                        ReferencedColumnName = "Id"
                    }
                ]
            }
        ],
        Indexes = []
    };

    /// <summary>
    /// Table B in the three-table scenario for testing inclusive import bug.
    /// Previously had FK to Table C (now removed in database). This is the table being imported.
    /// </summary>
    public static TableSchema TableB() => new()
    {
        Schema = "dbo",
        Name = "TableB",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Description", SqlDbType.NVarChar, length: 200),
            Column("TableCId", SqlDbType.Int, isNullable: true) // Column still exists but FK removed
        ],
        ForeignKeys = [], // FK to TableC has been removed
        Indexes = []
    };

    /// <summary>
    /// Table C in the three-table scenario for testing inclusive import bug.
    /// Exists independently, no longer referenced by Table B.
    /// </summary>
    public static TableSchema TableC() => new()
    {
        Schema = "dbo",
        Name = "TableC",
        Columns =
        [
            Column("Id", SqlDbType.Int, isPrimaryKey: true),
            Column("Category", SqlDbType.NVarChar, length: 50)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    /// <summary>
    /// Category table for testing duplicate FK scenarios
    /// </summary>
    public static TableSchema Category() => new()
    {
        Schema = "dbo",
        Name = "Category",
        Columns =
        [
            Column("CategoryID", SqlDbType.Int, isPrimaryKey: true),
            Column("Name", SqlDbType.NVarChar, length: 50, isNullable: false)
        ],
        ForeignKeys = [],
        Indexes = []
    };

    /// <summary>
    /// Item table with duplicate foreign keys to Category (both auto-generated and named)
    /// Simulates scenarios where a table has duplicate FK constraints
    /// </summary>
    public static TableSchema ItemWithDuplicateFKs() => new()
    {
        Schema = "dbo",
        Name = "Item",
        Columns =
        [
            Column("ItemID", SqlDbType.Int, isPrimaryKey: true),
            Column("CategoryID", SqlDbType.Int, isNullable: false),
            Column("Description", SqlDbType.NVarChar, length: 50, isNullable: false)
        ],
        ForeignKeys =
        [
            // Auto-generated FK name (SQL Server pattern with double underscores and hex suffix)
            new()
            {
                Name = "FK__Item__Category__5F6C19D1",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            },
            // Explicitly named FK
            new()
            {
                Name = "FK_Item_Category",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            }
        ],
        Indexes = []
    };

    /// <summary>
    /// Item table with both duplicate FKs using auto-generated names
    /// </summary>
    public static TableSchema ItemWithDuplicateAutoGeneratedFKs() => new()
    {
        Schema = "dbo",
        Name = "Item",
        Columns =
        [
            Column("ItemID", SqlDbType.Int, isPrimaryKey: true),
            Column("CategoryID", SqlDbType.Int, isNullable: false),
            Column("Description", SqlDbType.NVarChar, length: 50, isNullable: false)
        ],
        ForeignKeys =
        [
            // First auto-generated FK
            new()
            {
                Name = "FK__Item__Category__5F6C19D1",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            },
            // Second auto-generated FK (different hex suffix)
            new()
            {
                Name = "FK__Item__Category__6A5D3C2E",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            }
        ],
        Indexes = []
    };

    /// <summary>
    /// Item table with both duplicate FKs using explicitly named conventions
    /// </summary>
    public static TableSchema ItemWithDuplicateNamedFKs() => new()
    {
        Schema = "dbo",
        Name = "Item",
        Columns =
        [
            Column("ItemID", SqlDbType.Int, isPrimaryKey: true),
            Column("CategoryID", SqlDbType.Int, isNullable: false),
            Column("Description", SqlDbType.NVarChar, length: 50, isNullable: false)
        ],
        ForeignKeys =
        [
            // First named FK
            new()
            {
                Name = "FK_Item_Category",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            },
            // Second named FK (alternative naming)
            new()
            {
                Name = "FK_Item_Cat",
                TableName = "Item",
                ReferencedTableSchema = "dbo",
                ReferencedTableName = "Category",
                Columns =
                [
                    new()
                    {
                        Name = "CategoryID",
                        ReferencedColumnName = "CategoryID"
                    }
                ]
            }
        ],
        Indexes = []
    };
}
