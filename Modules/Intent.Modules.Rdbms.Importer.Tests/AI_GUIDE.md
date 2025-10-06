# Intent.Modules.Rdbms.Importer.Tests – AI Companion Guide

This test project exercises the `DbSchemaIntentMetadataMerger` component without touching the file system or a real database. All test data lives in-memory and is produced through factory helpers so scenarios stay deterministic and fast.

## Project Layout

| Path | Purpose |
| --- | --- |
| `TestData/` | Object Mother factories and composition helpers used to build schemas, packages, and supporting metadata. |
| `DbSchemaIntentMetadataMergerTests.cs` | Behavioural tests covering merge scenarios. Each test is self-documenting through its name and the `Arrange / Act / Assert` sections. |
| `DbSchemaToElementMappingTests.cs` | Integration tests validating end-to-end mapping scenarios using snapshot testing (Verify library). |
| `DbSchemaComprehensiveMappingTests.cs` | Comprehensive test suite (22 tests) validating mapping of all database objects: tables, columns, PKs, FKs, indexes, constraints, triggers, views, stored procedures. Uses snapshot testing. |
| `DbSchemaComprehensiveMappingTests.README.md` | Detailed documentation for the comprehensive test suite including test coverage, factory files, and maintenance guide. |
| `AI_GUIDE.md` (this file) | Orientation for future maintainers and AI agents. |

## Object Mother Strategy

The `TestData` folder contains the core building blocks:

### Core Factories
- **`DatabaseSchemas`** – produces reusable `DatabaseSchema` instances (e.g. `WithCustomersAndOrders`, `WithCustomerWithNewAddressColumn`).
- **`Tables`** – creates `TableSchema` objects with columns, FKs, indexes. Enhanced with `TableWithConstraints()` and `TableWithIndexesAndTriggers()` for comprehensive testing.
- **`Elements`** – builds `ElementPersistable` objects representing Intent classes and attributes.
- **`Associations`** – creates `AssociationPersistable` objects for relationships.
- **`Stereotypes`** – produces RDBMS stereotype instances (Table, Column, PrimaryKey, ForeignKey, View).
- **`PackageModels`** – builds `PackageModelPersistable` instances that represent existing Intent metadata.
- **`ImportConfigurations`** – supplies the merger with consistent configuration objects.

### Database Object Factories (New)
- **`Views`** – creates `ViewSchema` objects (simple, complex, with triggers, multi-schema).
- **`StoredProcedures`** – creates `StoredProcedureSchema` objects with parameters (In/Out/InOut) and result sets.
- **`Indexes`** – creates `IndexSchema` objects (unique, clustered, non-clustered, composite, filtered, with included columns).
- **`Triggers`** – creates `TriggerSchema` objects (After Insert/Update/Delete, Instead Of, for tables and views).

These factories intentionally model real-world Intent metadata while keeping each method small and composable.

## Scenario Composer

`TestData/ScenarioComposer.cs` wraps a schema and package into a lightweight `Scenario` record and offers fluent helpers:

```csharp
var scenario = ScenarioComposer.Create(
    DatabaseSchemas.WithCustomersAndOrders(),
    PackageModels.Empty());

scenario = scenario
    .WithSchemaMutation(schema => { /* tweak schema as needed */ })
    .WithPackageMutation(package => { /* tweak package state */ });
```

Use the composer when a test needs to adjust a base scenario without creating a brand-new factory method. Prefer:

1. Start from the closest factory (`DatabaseSchemas.*`, `PackageModels.*`).
2. Apply targeted mutations with the composer helpers.
3. Keep adjustments inside the `// Arrange` block so the setup reads like a story.

## Test Conventions

### For Behavioural Tests (DbSchemaIntentMetadataMergerTests)
- **Naming**: `MethodUnderTest_WhenCondition_ShouldOutcome` (e.g. `MergeSchemaAndPackage_NewTableDetected_AddsOnlyNewTable`).
- **Structure**: strictly follow the AAA pattern with comments `// Arrange`, `// Act`, `// Assert`. No other inline comments.
- **Assertions**: use Shouldly for readability (`ShouldContain`, `ShouldHaveSingleItem`, `ShouldBe`). Focus on the observable behaviour relevant to the scenario.
- **No snapshots**: tests rely on explicit assertions to stay clear and maintainable.

### For Snapshot Tests (DbSchemaToElementMappingTests, DbSchemaComprehensiveMappingTests)
- **Naming**: `Map{Feature}_{Scenario}_ShouldMatchSnapshot` (e.g. `MapTable_BasicProperties_ShouldMatchSnapshot`).
- **Structure**: follow AAA pattern with `// Arrange`, `// Act`, `// Assert` comments.
- **Assertions**: use Shouldly for basic validation, then capture full output with `await Verify(snapshot).UseParameters("parameter-name")`.
- **Snapshots**: use Verify library to generate `.received.txt` files, review and approve as `.verified.txt` for baseline comparison.
- **Parameters**: use `.UseParameters()` to create unique snapshot filenames for each test variant.

## Adding a New Test

1. Identify the closest existing schema/package factories.
2. Build a scenario via `ScenarioComposer` and apply minimal mutations required for the case.
3. Instantiate `DbSchemaIntentMetadataMerger` with the appropriate `ImportConfigurations` helper.
4. Assert exactly what should change (counts, names, external references, associations, etc.).
5. Keep the test short—if setup grows beyond a few lines, extract the adjustment into a helper inside `ScenarioComposer` or a dedicated factory method that composes existing pieces.

## When Extending Factories

- **>90% reuse**: call the factory and adjust inside the test using `ScenarioComposer`.
- **≈50% reuse**: add a new factory that composes existing ones and performs the required tweaks.
- **<50% reuse**: create a brand-new factory method and ensure it still mirrors real Intent metadata.

Following these guidelines keeps the suite expressive, fast, and easy for both humans and AI agents to evolve.

## Comprehensive Test Suite Overview

The `DbSchemaComprehensiveMappingTests` suite provides granular coverage of all database schema object mappings:

### Coverage Areas (22 tests total)
1. **Tables** - Basic properties, multi-schema support
2. **Columns** - Data types (Int, NVarChar, Decimal, UniqueIdentifier), nullable properties
3. **Primary Keys** - Simple PKs with identity, composite PKs
4. **Foreign Keys** - Simple and composite FK relationships
5. **Indexes** - Unique, clustered, non-clustered, composite, filtered, with included columns
6. **Constraints**:
   - Text constraints (MaxLength)
   - Decimal constraints (Precision, Scale)
   - Default constraints (values and functions)
   - Computed columns (expressions, persisted)
7. **Triggers** - After Insert/Update/Delete, Instead Of (for tables and views)
8. **Views** - Basic, complex (with aggregations), with triggers
9. **Stored Procedures** - Input/Output/InOut parameters, result sets, complex scenarios
10. **Integration** - Complete schemas with all features, multi-schema databases

### When to Add Comprehensive Tests
- Adding support for new database objects (e.g., sequences, user-defined types)
- Validating edge cases for existing object types
- Documenting expected mapping behavior for specific database features
- Regression testing after mapper refactoring

### Snapshot Testing Workflow
1. **Create Test**: Write test method following naming convention `Map{Feature}_{Scenario}_ShouldMatchSnapshot`
2. **Run Test**: Execute to generate `.received.txt` snapshot file
3. **Review**: Examine snapshot to verify correct mapping structure
4. **Approve**: Rename `.received.txt` to `.verified.txt` or use DiffEngine tool
5. **Commit**: Check in `.verified.txt` as the baseline for future comparisons
6. **Regression**: Future runs compare against `.verified.txt` and fail if mappings change

### Factory Pattern for Comprehensive Tests
Each comprehensive test should:
1. Use or create factory methods in `TestData/` files (Views, StoredProcedures, Indexes, Triggers, Tables)
2. Compose a complete `DatabaseSchema` with relevant objects
3. Create an empty or pre-populated `PackageModelPersistable`
4. Execute merge through `DbSchemaIntentMetadataMerger`
5. Build snapshot using `BuildPackageSnapshot()` helper method
6. Verify snapshot with descriptive parameter name via `.UseParameters()`

Example:
```csharp
[Fact]
public async Task MapView_BasicView_ShouldMatchSnapshot()
{
    // Arrange
    var schema = new DatabaseSchema
    {
        DatabaseName = "TestDatabase",
        Tables = [],
        Views = [Views.SimpleCustomerView()],
        StoredProcedures = []
    };
    var package = PackageModels.Empty();
    var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

    // Act
    var result = merger.MergeSchemaAndPackage(schema, package);

    // Assert
    result.IsSuccessful.ShouldBeTrue();
    var snapshot = BuildPackageSnapshot(package);
    await Verify(snapshot).UseParameters("view-basic");
}
```

## Test File Reference

| Test File | Purpose | Test Count | Testing Approach |
| --- | --- | --- | --- |
| `DbSchemaIntentMetadataMergerTests.cs` | Merge behavior, edge cases, error handling, composite keys, legacy scenarios, deletion tracking | 16 | Explicit assertions (Shouldly) |
| `DbSchemaToElementMappingTests.cs` | Integration scenarios, real-world use cases | 3 | Snapshot testing (Verify) |
| `DbSchemaComprehensiveMappingTests.cs` | Granular mapping validation for all DB objects, composite keys, ASP.NET Identity schema | 26 | Snapshot testing (Verify) |

**Total: 45 tests** (previously 44) ensuring robust database schema import functionality.

### Recently Added Test Coverage

The test suite now includes comprehensive coverage for advanced database scenarios:

#### Composite Keys (2 assertion + 2 snapshot tests)
- **Composite Primary Keys**: Tables with multi-column PKs (e.g., `Parents` with `Id` + `Id2`)
- **Composite Foreign Keys**: Relationships referencing composite PKs (e.g., `Children` → `Parents`)
- Validates that all PK columns receive stereotypes and associations link correctly

#### Complex Relationships (2 assertion tests)
- **Multiple Foreign Keys**: Tables with many FKs to the same or different tables (e.g., `PrimaryTable` with 5 FKs)
- **Self-Referencing Tables**: Circular relationships where a table references itself

#### Legacy and Edge Cases (2 assertion tests)
- **Tables Without Primary Keys**: Legacy tables with no PK constraint (e.g., `Legacy_Table`)
- **Naming Conventions**: Underscore-based naming (`Legacy_Table` → `LegacyTable`)

#### ASP.NET Identity Schema (1 snapshot test)
- **Complete Identity Framework**: All 7 ASP.NET Identity tables with their relationships
  - `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` (composite PK)
  - `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins` (composite PK), `AspNetUserTokens` (composite PK)
- Validates complex multi-table schema with various PK/FK patterns

#### Unique Indexes (1 snapshot test)
- **Business Constraints**: Unique indexes as business rules (e.g., `IX_Orders_RefNo`)
- Validates unique index mapping alongside foreign key relationships

#### Deletion Tracking (1 assertion test)
- **Association Removal**: When `AllowDeletions = true`, validates that associations are removed when their corresponding foreign keys no longer exist in the database
- **Critical Gap Coverage**: Previously untested scenario where `RemoveObsoleteAssociations` method ensures orphaned associations are cleaned up
- Tests the complete deletion workflow: FK removed from DB → association removed from package → warning logged

See `DbSchemaComprehensiveMappingTests.README.md` for detailed documentation of the comprehensive test suite.
