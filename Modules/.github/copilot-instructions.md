# AI Agent Instructions: Intent Architect Importers

This document provides guidelines for working with the Intent Architect Importers repository. It outlines the architecture, critical design patterns, testing strategy, and development practices. Follow these instructions to ensure consistency and correctness.

---

## 1. High-Level Goal & Architecture

**Primary Goal:** Build and maintain **Intent Architect modules** that import external metadata (e.g., JSON, C#, OpenAPI, RDBMS schemas) into Intent's visual designers.

### Core Architectural Pattern

All importers follow a unified, three-step process:
1.  **`PersistableFactory`**: Converts the source data (JSON file, C# class, DB table) into a standardized `Persistables` format (a collection of elements and associations).
2.  **`MetadataSynchronizer`**: The central engine that intelligently merges the new `Persistables` with existing package metadata. It uses `ExternalReference` to match elements, preventing duplicates.
3.  **`ModuleTask`**: Orchestrates the import process. It's invoked by TypeScript scripts in the Intent Architect designer.

### Core Projects

| Project | Description |
| :--- | :--- |
| **`Intent.MetadataSynchronizer/`** | The shared reconciliation engine for merging imports without duplicating elements. |
| **`Intent.Modules.Shared.FileImporter/`** | Shared utilities for file-based importers. |
| **`Intent.Modules.Json.Importer/`** | Profile-based JSON importer (Domain/Eventing/Services). |
| **`Intent.Modules.CSharp.Importer/`** | Roslyn-based C# reverse engineer. |
| **`Intent.Modules.OpenApi.Importer/`** | Imports OpenAPI specifications into CQRS/Service models. |
| **`Intent.Modules.Rdbms.Importer/`** | Imports database schemas. |

---

## 2. Core Design Patterns

These patterns are fundamental to the repository. Adhere to them strictly.

### 2.1. The `ExternalReference` (CRITICAL)

The `ExternalReference` is the **most critical concept** for ensuring data synchronization and idempotency. It is a stable, unique identifier for an element derived from its source.

- **Rule:** The `ExternalReference` **must remain stable** across re-imports. Any change will cause the synchronizer to see it as a new element.
- **Rule:** When modifying `ExternalReference` generation logic, you **must** update both the factory code and the corresponding test fixtures (`PackageModels` object mothers).

**Format Conventions:**
- **RDBMS**: `[schema].[tablename].[columnname]` (e.g., `dbo.customers.email`)
- **JSON**: Relative file path + dot notation for properties (e.g., `user.json.address.street`, `user.json.orders[0].total`)
- **C#**: `Namespace.ClassName.PropertyName`

### 2.2. Synchronization Logic (`MetadataSynchronizer`/`DbSchemaIntentMetadataMerger`)

The synchronizer is a non-destructive "add-or-modify" engine.
- It finds existing elements by matching `ExternalReference` and `SpecializationType`.
- If a match is found, it updates the element's properties and stereotypes.
- If no match is found, it adds a new element.
- **Deletions are opt-in**: Elements are only removed if `deleteExtra: true` is set and the element has an `ExternalReference` within the scope of the current import.

### 2.3. Visitor Pattern (JSON Importer)

The `ProfileFactory` in the JSON importer uses a visitor pattern to create different kinds of Intent models from the same JSON structure.
- **`DocumentDomainVisitor`**: Creates Domain Entities with associations.
- **`EventingMessagesVisitor`**: Creates Messages with nested Eventing DTOs (no associations).
- **`ServicesDtosVisitor`**: Creates DTOs with DTO-Fields (no associations).

### 2.4. Template Method Pattern (`ModuleTaskBase`)

The abstract `ModuleTaskBase<TInputModel>` class ensures a consistent execution flow for all module tasks:
1.  Deserialize and validate the input JSON from the designer script.
2.  Execute the core business logic via the abstract `ExecuteModuleTask` method.
3.  Serialize and return a `ExecuteResult<T>` object containing the outcome, data, and any warnings.

### 2.5. Configuration Object Pattern

Use dedicated configuration classes (`ImportConfiguration`) to pass settings through the layers, avoiding long parameter lists. This improves readability and simplifies adding new options.

---

## 3. Testing Guide

Testing is the backbone of this repository. All new features and bug fixes require comprehensive tests.

### 3.1. Test Data Factories (Object Mother Pattern)

**Rule:** All test data **must** be created using the Object Mother pattern. Factories exist in the `TestData/` folder of each test project. **Never use shared mutable state.**

- **`ScenarioComposer` (RDBMS Tests)**: A fluent API for building complex test scenarios by composing schemas and packages.
- **`DatabaseSchemas` / `Tables`**: Factories for creating database schema definitions.
- **`PackageModels`**: Factories for creating pre-existing Intent Architect package metadata. **CRITICAL**: The `ExternalReference`s in these models must exactly match those generated from the test schemas.
- **`ImportConfigurations`**: Factories for different import settings (e.g., `TablesOnly()`, `TablesWithDeletions()`, `DomainProfile(...)`).

### 3.2. Testing Architecture: Behavioral vs. Snapshot

| Test Type | Purpose | Naming Convention |
| :--- | :--- | :--- |
| **Behavioral** | Validate a single, specific behavior with explicit assertions. | `MethodUnderTest_Scenario_ExpectedOutcome` |
| **Snapshot** | Verify complex object mappings and catch regressions across entire structures. | `Map{Component}_{Feature}_{Scenario}_ShouldMatchSnapshot` |

### 3.3. Critical Testing Scenarios

Your test coverage **must** include these scenarios where applicable.

| Scenario | Description | Key Assertions |
| :--- | :--- | :--- |
| **Synchronization** | Verify that the `ExternalReference` logic is correct. Create a source model (e.g., `DatabaseSchema`) and a corresponding `PackageModel` with matching external references. | `ExternalReference` values are identical; elements are matched and updated, not duplicated. |
| **Idempotency** | Re-import the exact same data. | Element count remains the same. Element IDs (`Id`) are unchanged. |
| **Additions** | Import new items into an existing package. | New elements are added correctly. Existing elements are untouched. |
| **Deletions** | Remove an item from the source (e.g., a column) and re-import. Test with both `deleteExtra: true` and `deleteExtra: false`. | **With `true`**: The corresponding element is removed. **With `false`**: The element is orphaned (kept). |

### 3.4. Assertion & Snapshot Guidelines

- **DO** use `Shouldly` for fluent assertions in behavioral tests.
- **DO NOT** assert on the text of warning/error messages unless that is the specific behavior under test.
- **DO NOT** create and/or use temp files for unit tests. Instead use in-memory objects or streams.
- **DO** assert on observable state: element counts, names, IDs, types, and external references.
- **Snapshot Workflow**:
    1.  Write the test and run it to generate a `.received.txt` file.
    2.  Carefully review the received snapshot for correctness.
    3.  If correct, rename it to `.verified.txt` or use a diff tool to approve it.
    4.  Commit the `.verified.txt` file as the new baseline.

---

## 4. Development & Build

### Common Commands

```powershell
# Build a specific module
dotnet build "Intent.Modules.Json.Importer/Intent.Modules.Json.Importer.csproj" --verbosity minimal

# Run all tests in the solution
dotnet test Intent.Modules.Importers.sln

# Run tests for a specific project
dotnet test Intent.Modules.Json.Importer.Tests/Intent.Modules.Json.Importer.Tests.csproj
```

### Debugging Module Tasks

Module tasks execute inside the Intent Architect application. To debug them:
1.  Attach your debugger to the `Intent.Architect.exe` process.
2.  Set breakpoints in your `ModuleTaskBase` implementation.
3.  Invoke the task from the designer context menu in Intent Architect.

### Common Pitfalls to Avoid

1.  **Folder Creation**: Folders must be created in path order (parent before child) with correct `ParentFolderId` references.
2.  **Association Duplicates**: Always check for existing associations using `MetadataLookup.HasExistingAssociation()` before adding a new one.
3.  **Test Isolation**: Always generate fresh test data using factories in each test. Never reuse mutable objects across tests.
4.  **ExternalReference Collisions**: Ensure file-based importers include the full relative path in the reference to avoid clashes between files with similar structures.

### Key Files for Orientation

- **Core Engine**: `Intent.MetadataSynchronizer/Synchronizer.cs`
- **Lookup Index**: `Intent.MetadataSynchronizer/MetadataLookup.cs`
- **RDBMS Tests**: `Intent.Modules.Rdbms.Importer.Tests/DbSchemaIntentMetadataMergerTests.cs`
- **JSON Tests**: `Intent.Modules.Json.Importer.Tests/JsonSynchronizerTests.cs`
- **Object Mother Examples**: `Intent.Modules.Rdbms.Importer.Tests/TestData/`