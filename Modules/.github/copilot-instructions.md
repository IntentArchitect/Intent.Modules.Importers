# Intent Architect Importers - AI Agent Guidelines

## Purpose & Architecture

This repository builds **Intent Architect modules** that import external metadata (JSON, C#, OpenAPI, RDBMS) into Intent's visual designers. Each importer follows a unified pattern:

1. **PersistableFactory** converts source data to `Persistables` (elements + associations)
2. **MetadataSynchronizer** merges new persistables with existing package metadata by matching on `ExternalReference`
3. **ModuleTask** orchestrates import via designer-invoked TypeScript scripts

### Core Projects
- **`Intent.MetadataSynchronizer/`** - Shared reconciliation engine that merges imports without duplicating existing elements
- **`Intent.Modules.Json.Importer/`** - Profile-based JSON importer (Domain/Eventing/Services)
- **`Intent.Modules.CSharp.Importer/`** - Roslyn-based C# reverse engineer
- **`Intent.Modules.OpenApi.Importer/`** - OpenAPI → CQRS/Service models
- **`Intent.Modules.Rdbms.Importer/`** - Database schema importer
- **`Intent.Modules.Shared.FileImporter/`** - Shared project for file-based import utilities

### Module Anatomy
Each importer module includes:
- `*.imodspec` - Module manifest (dependencies, version, metadata)
- `Importer/` - Factory classes producing `Persistables`
- `Tasks/` - `ModuleTaskBase<TInput, TOutput>` implementations for designer integration
- `DesignerScripts/*.ts` - TypeScript wizard UIs launched from designer context menus
- `resources/scripts/*.js` - Bundled JavaScript for browser execution
- `modelers/` - Designer metadata (settings, stereotypes)

## Critical Patterns

### ExternalReference Format
ExternalReferences **must** remain stable across re-imports to enable synchronization. Use:
- **Root elements**: relative file path (e.g., `user.json`, `Namespace.ClassName`)
- **Nested properties**: dot notation (e.g., `user.json.address.street`)
- **Array elements**: bracket notation (e.g., `user.json.orders[0].total`)

When modifying ExternalReference logic, update **both** factory code and test fixtures (`PackageModels` object mothers).

### Visitor Pattern (JSON Importer)
`ProfileFactory` returns `IJsonElementVisitor` implementations that define profile-specific projections:
- **`DocumentDomainVisitor`** - Creates Domain Entities with associations
- **`EventingMessagesVisitor`** - Creates Messages with nested Eventing DTOs (no associations)
- **`ServicesDtosVisitor`** - Creates DTOs with DTO-Fields (no associations)

### Specialization Type IDs
Intent metadata elements are identified by `SpecializationType` and `SpecializationTypeId` GUIDs. See `Constants.cs` in each importer for the canonical list. Examples:
- **Folder**: `4d95d53a-8855-4f35-9820-3106413fec04`
- **Type-Definition**: `d4e577cd-ad05-4180-9a2e-fff4ddea0e1e`
- **Class** (Domain): Varies by designer

## Testing Conventions

All test projects follow the **Object Mother** pattern with factories in `TestData/`:
- **`PackageModels`** - Pre-existing Intent metadata for merge scenarios
- **`ImportConfigurations`** - Input config objects
- **Profile-specific factories** - Test data sources (JSON, OpenAPI specs, C# files)

### Test Structure (AAA Pattern)
```csharp
[Fact]
public void MethodUnderTest_Scenario_ExpectedOutcome()
{
    // Arrange
    var factory = new JsonPersistableFactory();
    var config = ImportConfigurations.DomainProfile();
    var packages = new[] { PackageModels.WithTypeDefinitions() };

    // Act
    var result = factory.GetPersistables(config, packages);

    // Assert
    result.Elements.ShouldContain(e => e.Name == "Customer");
}
```

### Assertion Guidelines
- **DO** assert on observable state: element counts, names, specialization types, external references
- **DO NOT** assert on warning/error message text (brittle to formatting changes)
- Exception: message assertions are valid when explicitly testing warning/error generation logic

### Snapshot Testing
Mapping tests use **Verify** library for comprehensive output validation:
1. Run test to generate `.received.txt`
2. Review snapshot for correctness
3. Rename to `.verified.txt` (or use DiffEngine)
4. Commit `.verified.txt` as baseline

See `AI_GUIDE.md` in each test project for detailed conventions.

## Build & Development

### Commands
```powershell
# Build specific module (minimal output)
dotnet build "Intent.Modules.Json.Importer/Intent.Modules.Json.Importer.csproj" --no-incremental --verbosity minimal --nologo

# Run all tests
dotnet test Intent.Modules.Importers.sln

# Test specific project
dotnet test Intent.Modules.Json.Importer.Tests/Intent.Modules.Json.Importer.Tests.csproj
```

### C# String Literals
- Use verbatim strings (`@"..."`) for paths and strings with double quotes
- Use raw string literals (`"""..."""`) for multi-line content

### Debugging Module Tasks
Module tasks execute in Intent Architect's context. To debug:
1. Attach debugger to `Intent.Architect.exe` process
2. Set breakpoints in `ModuleTaskBase.Execute` implementations
3. Invoke task from designer context menu

## Integration Points

### Designer Invocation
TypeScript scripts in `DesignerScripts/` call `executeModuleTask(taskTypeId, inputJson)` which:
1. Serializes input to JSON
2. Loads module assembly via reflection
3. Calls `IModuleTask.Execute(args)` with JSON args
4. Returns serialized `ExecuteResult<T>` with errors/warnings/result

### MetadataSynchronizer Usage
```csharp
Synchronizer.Execute(
    targetPackage: packageModel,
    parentFolderId: targetFolder.Id,
    persistables: factoryResult,
    deleteExtra: config.AllowRemoval,
    createAttributesWithUnknownTypes: true,
    stereotypeManagementMode: StereotypeManagementMode.Merge);
```

Matches elements by `ExternalReference`, updates existing, adds new, optionally removes unmatched.

## Module Packaging
`.imodspec` files define module metadata. Key sections:
- `<dependencies>` - Required Intent modules (e.g., `Intent.Common`, `Intent.Common.Types`)
- `<files>` - DLLs to include in packaged `.imod` file
- `<metadata><install>` - Designer settings to inject on module install
- `supportedClientVersions` - Semantic version range (e.g., `[4.5.18-a-a,5.0.0)`)

## Common Pitfalls

1. **Folder creation**: Folders must be created in path order (parent→child) with correct `ParentFolderId` references
2. **Type resolution**: Unknown types become `Type-Definition` placeholders; ensure primitive types exist in `MetadataLookup`
3. **Association duplicates**: Check `MetadataLookup.HasExistingAssociation()` before adding associations
4. **Test isolation**: Use factory methods, never shared state between tests
5. **ExternalReference collisions**: When multiple files have similar structures, include full file path in reference

## Key Files for Orientation

- `Intent.MetadataSynchronizer/Synchronizer.cs` - Core merge algorithm
- `Intent.Modules.Json.Importer/Importer/ProfileFactory.cs` - Profile resolution
- `Intent.Modules.Json.Importer.Tests/AI_GUIDE.md` - Comprehensive test guide
- Each test project's `TestData/` folder - Object Mother examples
