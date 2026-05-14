# Intent Modules Importers Guidance

Use these rules when working in this repository.

---

## 🏗️ Architecture Overview

**Primary Goal:** Build and maintain **Intent Architect modules** that import external metadata (e.g., JSON, C#, OpenAPI, RDBMS schemas) into Intent's visual designers.

### Three-Step Import Process

All importers follow this unified flow:

1. **`PersistableFactory`** — Converts source data (JSON file, C# class, DB table, schema) into standardized `Persistables` format (elements + associations).
2. **`MetadataSynchronizer`** — The central reconciliation engine that intelligently merges new `Persistables` with existing package metadata using `ExternalReference` matching to prevent duplicates.
3. **`ModuleTask`** — Orchestrates the import process; invoked by TypeScript scripts in the Intent Architect designer.

### Core Projects

| Project | Description |
|---------|-------------|
| `Intent.MetadataSynchronizer/` | Shared reconciliation engine for merging imports without duplicating elements. |
| `Intent.Modules.Shared.FileImporter/` | Shared utilities for file-based importers. |
| `Intent.Modules.Json.Importer/` | Profile-based JSON importer (Domain/Eventing/Services). |
| `Intent.Modules.CSharp.Importer/` | Roslyn-based C# reverse engineer. |
| `Intent.Modules.OpenApi.Importer/` | Imports OpenAPI specifications into CQRS/Service models. |
| `Intent.Modules.Rdbms.Importer/` | Imports database schemas. |

---

## 🎯 Element Lookup & Resolution Standard

When resolving or matching elements (types, classes, enums, type definitions, etc.), apply this **precedence order**:

1. **ExternalReference match** — First priority. Lookup by exact external reference (compiled fully-qualified identifier).  
   *Why: Stable across re-imports; preserves idempotency and existing element IDs.*

2. **Type name/specialization match** — Second priority. For type definitions specifically, lookup by name + generic parameter count.  
   *Why: Type definitions are identified by semantic name, not syntax.*

3. **Element name match** — Last priority (fallback). Lookup by display/canonical name within the current scope.  
   *Why: Weakest signal; use only when external reference and type specialization fail.*

4. **Create new if no match** — All lookups exhausted.  
   *Why: Ensures required elements exist; cascaded type definitions are created as needed.*

This order is enforced in `SetTypeReference()`, `GetElementByReference()`, `TryGetElementByName()`, and all importer-specific reference resolution logic. **Do not invert or skip tiers** in matching logic; doing so breaks re-import stability.

---

## � Synchronization Logic & ExternalReference

### How Synchronization Works

The synchronizer is a non-destructive "add-or-modify" engine:
- Finds existing elements by matching `ExternalReference` and `SpecializationType`.
- If a match is found, updates the element's properties and stereotypes **in place**.
- If no match is found, adds a new element.
- **Deletions are opt-in**: Elements are only removed if `deleteExtra: true` is set and the element has an `ExternalReference` within the scope of the current import.

### ExternalReference Format Conventions

The `ExternalReference` is the **most critical concept** for ensuring data synchronization and idempotency. It must remain stable across re-imports.

| Format | Example | Used By |
|--------|---------|---------|
| **RDBMS** | `dbo.customers.email` | Rdbms Importer |
| **JSON** | `user.json.address.street` | Json Importer |
| **C#** | `Namespace.ClassName.PropertyName` | CSharp Importer |

**Critical Rules:**
- The `ExternalReference` **must remain stable** across re-imports. Any change causes the synchronizer to treat it as a new element.
- When modifying `ExternalReference` generation logic, update both the factory code and corresponding test fixtures (`PackageModels` object mothers).

---

## 🎨 Design Patterns Used

### Visitor Pattern (JSON Importer)

The `ProfileFactory` uses a visitor pattern to create different kinds of Intent models from the same JSON structure:
- **`DocumentDomainVisitor`**: Creates Domain Entities with associations.
- **`EventingMessagesVisitor`**: Creates Messages with nested Eventing DTOs (no associations).
- **`ServicesDtosVisitor`**: Creates DTOs with DTO-Fields (no associations).

### Template Method Pattern (ModuleTaskBase)

The abstract `ModuleTaskBase<TInputModel>` class ensures consistent execution flow:
1. Deserialize and validate input JSON from the designer script.
2. Execute core business logic via abstract `ExecuteModuleTask` method.
3. Serialize and return an `ExecuteResult<T>` object with outcome, data, and warnings.

### Configuration Object Pattern

Use dedicated configuration classes (`ImportConfiguration`) to pass settings through layers, avoiding long parameter lists. This improves readability and simplifies adding new options.

---

## 🧪 Testing Standards

### Test Data Factories (Object Mother Pattern)

**Rule:** All test data **must** be created using the Object Mother pattern. Factories exist in the `TestData/` folder of each test project. **Never use shared mutable state.**

- **`ScenarioComposer` (RDBMS Tests)**: Fluent API for building complex test scenarios by composing schemas and packages.
- **`DatabaseSchemas` / `Tables`**: Factories for creating database schema definitions.
- **`PackageModels`**: Factories for creating pre-existing Intent Architect package metadata. **CRITICAL**: The `ExternalReference`s in these models must exactly match those generated from the test schemas.
- **`ImportConfigurations`**: Factories for different import settings (e.g., `TablesOnly()`, `TablesWithDeletions()`, `DomainProfile(...)`).

### Testing Architecture: Behavioral vs. Snapshot

| Test Type | Purpose | Naming Convention |
|-----------|---------|-------------------|
| **Behavioral** | Validate a single, specific behavior with explicit assertions. | `MethodUnderTest_Scenario_ExpectedOutcome` |
| **Snapshot** | Verify complex object mappings and catch regressions across entire structures. | `Map{Component}_{Feature}_{Scenario}_ShouldMatchSnapshot` |

### Critical Testing Scenarios

Your test coverage **must** include these scenarios where applicable:

| Scenario | Key Assertions |
|----------|----------------|
| **Synchronization** | `ExternalReference` values match; elements are updated in place, not duplicated. |
| **Idempotency** | Re-import same data; element count and IDs remain unchanged. |
| **Additions** | Import new items into existing package; new elements added, existing untouched. |
| **Deletions** | Remove item from source and re-import. With `deleteExtra: true`, element removed. With `false`, element orphaned (kept). |

### Assertion & Snapshot Guidelines

- **DO** use `Shouldly` for fluent assertions in behavioral tests.
- **DO NOT** assert on warning/error message text unless that is the specific behavior under test.
- **DO NOT** create and/or use temp files for unit tests. Use in-memory objects or streams instead.
- **DO** assert on observable state: element counts, names, IDs, types, and external references.
- **Snapshot Workflow**:
  1. Write the test and run it to generate a `.received.txt` file.
  2. Carefully review the received snapshot for correctness.
  3. If correct, rename it to `.verified.txt` or use a diff tool to approve it.
  4. Commit the `.verified.txt` file as the new baseline.

---

## 🚀 Development & Build

### Common Commands

```powershell
# Build a specific module
dotnet build "Modules/Intent.Modules.Json.Importer/Intent.Modules.Json.Importer.csproj" --verbosity minimal

# Run all tests in the solution
dotnet test Modules/Intent.Modules.Importers.sln

# Run tests for a specific project
dotnet test Modules/Intent.Modules.Json.Importer.Tests/Intent.Modules.Json.Importer.Tests.csproj

# Run pre-commit checks
./run-pre-commit-checks.ps1
```

### Debugging Module Tasks

Module tasks execute inside the Intent Architect application. To debug:
1. Attach your debugger to the `Intent.Architect.exe` process.
2. Set breakpoints in your `ModuleTaskBase` implementation.
3. Invoke the task from the designer context menu in Intent Architect.

### Key Files for Orientation

- **Core Engine**: `Intent.MetadataSynchronizer/Synchronizer.cs`
- **Lookup Index**: `Intent.MetadataSynchronizer/MetadataLookup.cs`
- **RDBMS Tests**: `Intent.Modules.Rdbms.Importer.Tests/DbSchemaIntentMetadataMergerTests.cs`
- **JSON Tests**: `Intent.Modules.Json.Importer.Tests/JsonSynchronizerTests.cs`
- **Object Mother Examples**: `Intent.Modules.Rdbms.Importer.Tests/TestData/`

---

## ⚠️ Common Pitfalls to Avoid

1. **Folder Creation**: Folders must be created in path order (parent before child) with correct `ParentFolderId` references.
2. **Association Duplicates**: Always check for existing associations using `MetadataLookup.HasExistingAssociation()` before adding a new one.
3. **Test Isolation**: Always generate fresh test data using factories in each test. Never reuse mutable objects across tests.
4. **ExternalReference Collisions**: Ensure file-based importers include the full relative path in the reference to avoid clashes between files with similar structures.
5. **ExternalReference Stability**: Do not change `ExternalReference` generation logic casually. Any change breaks existing element matching and causes duplicates on re-import.

---

## �🔄 Core Principles

- Treat re-import idempotency as a core invariant. Changes to importer behavior should preserve existing element and association IDs whenever an existing model item can still be matched.
- Preserve `ExternalReference` semantics. Do not change matching keys, generated reference formats, or precedence rules casually; most importer stability depends on them.
- Prefer fixing importer behavior in the real mapping or synchronization layer instead of patching model output after the fact.
- Keep task entrypoints thin. Validate request models early, log the input and output shape, and push behavior into analyzers, factories, visitors, synchronizers, or mergers.
- Preserve manually modeled metadata where the current behavior and tests expect it to survive re-imports.
- When changing importer behavior, update the closest importer-specific tests and add regression coverage for merge, deletion, collision, or precedence behavior as applicable.
- Reuse `TestData` object mothers, scenario composers, and snapshot builders instead of constructing large test graphs inline.
- Run focused tests for the touched importer, and prefer running `/run-pre-commit-checks.ps1` before concluding broader work.
- Do not generalize shared infrastructure unless the behavior is genuinely common across multiple importers.
- Treat the existing tests as the primary specification for merge semantics, naming, and model preservation rules.