# Intent.Modules.Json.Importer.Tests – AI Companion Guide

This test project exercises the `JsonPersistableFactory` component and its profile-based visitors without touching the file system beyond temporary test directories. All test data is produced through Object Mother factories to keep scenarios deterministic and fast.

## Project Layout

| Path | Purpose |
| --- | --- |
| `TestData/` | Object Mother factories and helper utilities for building test scenarios. |
| `JsonPersistableFactoryTests.cs` | Behavioural tests covering import scenarios using explicit assertions (Shouldly). Each test follows the Arrange/Act/Assert pattern. |
| `JsonSynchronizerTests.cs` | Tests for merge/re-import scenarios using the `Intent.MetadataSynchronizer`. Validates that existing elements are matched by ExternalReference and updated correctly. |
| `MultipleFileCollisionTests.cs` | Tests for ExternalReference collision scenarios when importing multiple files with similar structures. |
| `DomainVisitorMappingTests.cs` | Snapshot tests for Domain profile using Verify library. |
| `EventingMessagesVisitorMappingTests.cs` | Snapshot tests for Eventing Messages profile using Verify library. |
| `ServicesDtosVisitorMappingTests.cs` | Snapshot tests for Services DTOs profile using Verify library. |
| `AI_GUIDE.md` (this file) | Orientation for future maintainers and AI agents. |

## Object Mother Strategy

The `TestData` folder contains the core building blocks:

### Core Factories
- **`JsonDocuments`** – provides file paths to test JSON files in the `TestFiles/` directory (Domain, Services, Eventing folders).
- **`PackageModels`** – creates `PackageModelPersistable` instances with type definitions and pre-existing elements for testing merge scenarios. Used by synchronizer tests.
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
- ✅ External reference tracking (full relative path including .json extension to prevent collisions)
- ✅ Merge/re-import scenarios (synchronizer matches existing elements by ExternalReference)

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

1. **Merge Logic via Synchronizer**: JSON importer creates new persistables, then uses `Intent.MetadataSynchronizer` to merge with existing package metadata. Elements are matched by ExternalReference.
2. **File-Based Input**: Tests use actual JSON files in the `TestFiles/` directory.
3. **Three Profiles**: Each profile has distinct behaviors and requires separate test coverage.
4. **Path-Based References**: ExternalReference format is `{relative-file-path}.{property-path}` (e.g., `simple-customer.json.Id`, `user.json.identities[0].connection`). Root elements use just the file path (e.g., `simple-customer.json`).
5. **Collision Prevention**: Full file path in ExternalReference prevents collisions when multiple JSON files have similar structures (e.g., multiple files with `identities` collections).

## Test Maintenance

- **When adding new JSON features**: Add test JSON files to `TestFiles/` directory and create behavioural + snapshot tests.
- **When adding new profiles**: Create a new visitor mapping test class following existing patterns.
- **When fixing bugs**: Add a regression test that captures the fix.
- **When refactoring**: Re-run snapshot tests and update `.verified.txt` files if behavior intentionally changes.
- **When modifying ExternalReference format**: Update ALL PackageModels methods to match the new format, and re-approve all snapshot test baselines.

## ExternalReference Format

**CRITICAL**: The ExternalReference format must be consistent between:
1. The importer (JsonPersistableFactory)
2. Test fixtures (PackageModels for synchronizer tests)
3. Snapshot baselines (.verified.txt files)

**Current Format**:
- Root elements: `{relative-path-from-source-folder}` (e.g., `simple-customer.json`, `Domain/Customers/customer.json`)
- Nested elements: `{relative-path}.{json-property-path}` (e.g., `simple-customer.json.Id`, `user.json.identities[0].connection`)
- Array elements: Use `[index]` notation (e.g., `user.json.identities[0]`)

**Why**: Including the full file path prevents ExternalReference collisions when multiple JSON files have similar structures. For example, if `user.json` and `users-by-email-response.json` both have an `identities` collection, the nested class would be `user.json.identities[0]` vs `users-by-email-response.json.identities[0]`.

Following these guidelines keeps the suite expressive, fast, and easy for both humans and AI agents to evolve.

## Known Issues (As of 2025-10-30)

### Synchronizer Tests (4 failing)
The JsonSynchronizerTests are currently failing because the attributes (child elements) are not being properly matched during synchronization. The issue is that:
1. The JsonPersistableFactory returns elements in a hierarchy (attributes are in the class's ChildElements collection)
2. The Synchronizer was modified to flatten this hierarchy when creating the MetadataLookup (line 23 in Synchronizer.cs)
3. However, the synchronization still fails to match attributes correctly

**Root Cause**: The Synchronizer recursively processes children, but the matching logic for attributes may need adjustment. The attributes have correct ExternalReferences (e.g., `simple-customer.json.Id`) but aren't being found/matched.

**Next Steps**: Debug the SynchronizeElements recursive logic to understand why attributes aren't being matched even though their ExternalReferences are correct.

### MultipleFileCollisionTests (1 failing)
The `ImportUserAndUsersByEmailResponse_CheckExternalReferences` test expects to find 2 Identity classes (one from each file), but finds 0. This test was created to verify that the ExternalReference collision fix works for nested objects with the same name from different files.

**Next Steps**: Investigate why nested classes named "Identity" aren't being created from the array elements in user.json and users-by-email-response.json.
