# Intent.Modules.Json.Importer.Tests – AI Companion Guide

This test project exercises the `JsonPersistableFactory` component and its profile-based visitors without touching the file system beyond temporary test directories. All test data is produced through Object Mother factories to keep scenarios deterministic and fast.

## Project Layout

| Path | Purpose |
| --- | --- |
| `TestData/` | Object Mother factories and helper utilities for building test scenarios. |
| `JsonPersistableFactoryTests.cs` | Behavioural tests covering import scenarios using explicit assertions (Shouldly). Each test follows the Arrange/Act/Assert pattern. |
| `DomainVisitorMappingTests.cs` | Snapshot tests for Domain profile using Verify library. |
| `EventingMessagesVisitorMappingTests.cs` | Snapshot tests for Eventing Messages profile using Verify library. |
| `ServicesDtosVisitorMappingTests.cs` | Snapshot tests for Services DTOs profile using Verify library. |
| `AI_GUIDE.md` (this file) | Orientation for future maintainers and AI agents. |

## Object Mother Strategy

The `TestData` folder contains the core building blocks:

### Core Factories
- **`JsonDocuments`** – produces reusable JSON string content for various scenarios (simple, nested, collections, edge cases).
- **`PackageModels`** – creates `PackageModelPersistable` instances with type definitions for Domain and Services packages.
- **`ImportConfigurations`** – builds `JsonConfig` objects for each profile with sensible defaults.
- **`SnapshotBuilder`** – creates readable text snapshots of `Persistables` for Verify tests.

These factories intentionally model real-world scenarios while keeping each method small and composable.

## Test Conventions

### For Behavioural Tests (JsonPersistableFactoryTests)
- **Naming**: `MethodUnderTest_Scenario_ExpectedOutcome` (e.g., `GetPersistables_SimpleJson_CreatesClassWithAttributes`).
- **Structure**: strictly follow the AAA pattern with comments `// Arrange`, `// Act`, `// Assert`. No other inline comments.
- **Assertions**: use Shouldly for readability (`ShouldContain`, `ShouldHaveSingleItem`, `ShouldBe`). Focus on the observable behaviour relevant to the scenario.
- **No snapshots**: tests rely on explicit assertions to stay clear and maintainable.

### ⚠️ CRITICAL: Assertion Best Practices
- **DO assert on actual state changes**: element counts, IDs, names, external references, presence/absence of objects, stereotypes, metadata.
- **DO NOT assert on warning or error messages** unless the test is explicitly designed to verify messaging behavior.
- **WHY**: Message assertions are brittle and break when formatting changes, making tests maintenance nightmares.

**Good Example (Robust):**
```csharp
// Verify element was created with correct properties
result.Elements.ShouldHaveSingleItem();
var customer = result.Elements.First();
customer.Name.ShouldBe("Customer");
customer.SpecializationType.ShouldBe("Class");
```

### For Snapshot Tests (Profile-Specific Mapping Tests)
- **Naming**: `Map{Profile}_{Feature}_{Scenario}_ShouldMatchSnapshot` (e.g., `MapDomain_SimpleClass_BasicProperties_ShouldMatchSnapshot`).
- **Structure**: follow AAA pattern with `// Arrange`, `// Act`, `// Assert` comments.
- **Assertions**: use Shouldly for basic validation (e.g., element count checks), then capture full output with `await Verify(snapshot).UseParameters("parameter-name")`.
- **Snapshots**: use Verify library to generate `.received.txt` files, review and approve as `.verified.txt` for baseline comparison.
- **Parameters**: use `.UseParameters()` to create unique snapshot filenames for each test variant.

## Architecture Overview

The JSON Importer uses a **visitor-based architecture**:

1. **`JsonPersistableFactory`** – Core factory that orchestrates the import process. It reads JSON files, classifies properties, and delegates creation to the appropriate visitor.
2. **Profile-Based Visitors**:
   - **`DocumentDomainVisitor`** – Creates Domain classes with associations.
   - **`EventingMessagesVisitor`** – Creates Messages and Eventing DTOs (no associations).
   - **`ServicesDtosVisitor`** – Creates DTOs with DTO-Fields (no associations).
3. **`ProfileFactory`** – Resolves the correct visitor based on the selected `ImportProfile`.

## Test Coverage Areas

### Profile-Specific Behaviors
- ✅ **Domain**: Classes with associations, PK stereotypes on root Id attribute
- ✅ **Eventing**: Messages with Eventing DTOs (nested objects become properties, no associations)
- ✅ **Services**: DTOs with DTO-Fields (nested objects become properties, no associations)

### JSON Features
- ✅ Simple properties (string, number, bool, guid, datetime)
- ✅ Nested objects (composition)
- ✅ Arrays/Collections (single-level and nested)
- ✅ Empty arrays (creates unknown type with remarks)
- ✅ Null/undefined values (creates unknown type with remarks)

### Folder Structures
- ✅ Flat structure (root-level JSON files)
- ✅ Nested folders (e.g., `Domain/Customers/customer.json`)
- ✅ Multiple files in same folder
- ✅ Folder hierarchy creation and reuse

### Casing Conventions
- ✅ PascalCase transformation
- ✅ AsIs preservation

### Other Features
- ✅ Duplicate name resolution (path-based disambiguation)
- ✅ Selected files (process subset of available files)
- ✅ External reference tracking (path-based)

## Adding a New Test

1. **Identify the scenario**: Determine if it's profile-specific, a JSON feature, or an edge case.
2. **Choose test type**:
   - **Behavioural test**: For specific behaviors that need explicit assertions.
   - **Snapshot test**: For comprehensive validation of visitor output.
3. **Use factories**: Start with existing `JsonDocuments`, `PackageModels`, and `ImportConfigurations` factories.
4. **Follow naming conventions**: Use the prescribed format for test method names.
5. **Keep tests focused**: Each test should validate one specific scenario or behavior.

## Snapshot Testing Workflow

1. **Create Test**: Write test method following naming convention.
2. **Run Test**: Execute to generate `.received.txt` snapshot file in the test project.
3. **Review**: Examine snapshot to verify correct mapping structure.
4. **Approve**: Rename `.received.txt` to `.verified.txt` or use DiffEngine tool.
5. **Commit**: Check in `.verified.txt` as the baseline for future comparisons.
6. **Regression**: Future runs compare against `.verified.txt` and fail if mappings change.

## Key Differences from RDBMS Importer Tests

1. **No Merge Logic**: JSON importer creates new elements from scratch; no merging with existing metadata.
2. **File-Based Input**: Tests write JSON to temporary directories rather than in-memory schemas.
3. **Three Profiles**: Each profile has distinct behaviors and requires separate test coverage.
4. **Path-Based References**: ExternalReference uses relative file paths.
5. **Simpler Model**: No complex relationship tracking or deletion scenarios.

## Test Maintenance

- **When adding new JSON features**: Add to `JsonDocuments` factory and create behavioural + snapshot tests.
- **When adding new profiles**: Create a new visitor mapping test class following existing patterns.
- **When fixing bugs**: Add a regression test that captures the fix.
- **When refactoring**: Re-run snapshot tests and update `.verified.txt` files if behavior intentionally changes.

Following these guidelines keeps the suite expressive, fast, and easy for both humans and AI agents to evolve.
