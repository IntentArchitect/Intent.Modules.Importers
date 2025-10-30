# Intent.Modules.OpenApi.Importer.Tests – AI Companion Guide

This test project validates the `OpenApiPersistableFactory` component in the OpenAPI Importer module. All test data is loaded from local OpenAPI specification files (JSON/YAML) using Object Mother factories to keep scenarios deterministic and fast. These tests were ported from the CLI.Tests project to validate that the module implementation produces identical behavior.

## Project Layout

| Path | Purpose |
| --- | --- |
| `TestData/` | Object Mother factories for OpenAPI specs, packages, and configurations. |
| `Data/` | 10 OpenAPI specification files (JSON/YAML) representing different scenarios. |
| `OpenApiPersistableFactoryTests.cs` | Behavioral tests covering all OpenAPI specs in both CQRS and Service modes. Each test follows the AAA pattern with state-based assertions. |
| `AI_GUIDE.md` (this file) | Orientation for future maintainers and AI agents. |

## Object Mother Strategy

The `TestData` folder contains the core building blocks:

### Core Factories
- **`OpenApiSpecs`** – loads OpenAPI specification files from `Data/` folder and provides reusable Stream instances for each spec (e.g. `PetStore()`, `WithEnum()`, `IntentGeneratedSecured()`).
- **`PackageModels`** – builds `PackageModelPersistable` objects representing existing Intent metadata. Provides methods like `Empty()` and `WithTypeDefinitions()`.
- **`ImportConfigurations`** – supplies the factory with consistent `ImportConfig` instances for CQRS and Service modes.

These factories intentionally model real-world Intent metadata while keeping each method small and composable.

## Test Data Files

The `Data/` folder contains 10 OpenAPI specifications covering various scenarios:

| File | Scenario | Format |
| --- | --- | --- |
| `pet-store.yaml` | Classic Swagger PetStore example with REST endpoints | YAML |
| `PetStorevOpenApi1.0.yaml` | PetStore using OpenAPI 1.0 format | YAML |
| `with-enum.json` | API with enum schema types | JSON |
| `with-query.json` | API with query parameters | JSON |
| `with-query2.json` | Additional query parameter scenarios | JSON |
| `swagger-sample.json` | Complex Swagger specification with DTOs | JSON |
| `all-of-named-enums-security.json` | AllOf schemas with enums and security | JSON |
| `azure-open-api.json` | Complex Azure OpenAPI specification | JSON |
| `clean-arch-tests.json` | Clean Architecture service patterns | JSON |
| `intent-generated-secured.json` | Secured endpoints with authentication | JSON |

## Test Conventions

### For Behavioral Tests (OpenApiPersistableFactoryTests)
- **Naming**: `MethodUnderTest_Scenario_ShouldOutcome` (e.g. `GetPersistables_PetStore_CQRS_CreatesCommandsAndQueries`).
- **Structure**: strictly follow the AAA pattern with comments `// Arrange`, `// Act`, `// Assert`. No other inline comments.
- **Assertions**: use Shouldly for readability (`ShouldContain`, `ShouldNotBeEmpty`, `ShouldBeNull`). Focus on the observable behavior relevant to the scenario.
- **No message assertions**: tests rely on state-based assertions to stay robust and maintainable.

### ⚠️ CRITICAL: Assertion Best Practices
- **DO NOT assert on warning or error messages** unless the test is explicitly designed to verify messaging behavior.
- **DO assert on actual state changes**: element counts, specialization types, presence/absence of elements.
- **WHY**: Warning/error message assertions are brittle and break when message formatting changes, making tests maintenance nightmares.

**Bad Example (Brittle):**
```csharp
result.Warnings.ShouldContain(w => w.Contains("Unable to create endpoint"));
```

**Good Example (Robust):**
```csharp
// Verify actual state - that commands were created
var commands = result.Elements.Where(c => c.SpecializationType == "Command").ToList();
commands.ShouldNotBeEmpty("Should create Command elements");
```

**Exception**: When explicitly testing warning/error generation logic itself, message assertions are appropriate.

## Test Coverage

The test suite covers:

1. **All 10 OpenAPI Specifications** - Theory tests validate all specs in both CQRS and Service modes (20 test cases).
2. **CQRS Mode** - Validates Command and Query element generation.
3. **Service Mode** - Validates Service and Operation element generation.
4. **DTO Generation** - Validates request/response schema mapping to DTO elements.
5. **Enum Handling** - Validates enum schema type mapping.
6. **Query Parameters** - Validates parameter mapping for query operations.
7. **Security** - Validates stereotype application for secured endpoints.
8. **Complex Schemas** - Validates AllOf, complex Azure specs, and Clean Architecture patterns.

**Total: 30 tests** ensuring robust OpenAPI import functionality.

## Return Value Pattern

The `OpenApiPersistableFactory.GetPersistables` method returns a `Persistables` record containing:
- **`Elements`** - List of `ElementPersistable` objects (Commands, Queries, Services, DTOs, Enums).
- **`Associations`** - List of `AssociationPersistable` objects representing relationships.

Tests should assert on `result.Elements` and `result.Associations`, **not** on the input `package` parameter which is not modified in-place.

## Adding a New Test

1. Identify the scenario to test (e.g., new OpenAPI spec, edge case, specific feature).
2. Add the OpenAPI spec file to `Data/` folder if needed.
3. Add a factory method in `OpenApiSpecs` if testing a new spec file.
4. Create a test method following the naming convention.
5. Use `ImportConfigurations` and `PackageModels` factories for consistent setup.
6. Assert on `result.Elements` state changes (counts, types, properties).
7. Keep assertions focused on observable behavior, not implementation details.

## When Extending Factories

- **>90% reuse**: use existing factory methods and adjust in the test's Arrange block.
- **≈50% reuse**: add a new factory method that composes existing ones.
- **<50% reuse**: create a brand-new factory method ensuring it mirrors real Intent metadata.

## Common Patterns

### Theory Tests with All Specs
```csharp
[Theory]
[MemberData(nameof(GetAllSpecifications))]
public void GetPersistables_AllSpecs_CQRS_ShouldNotThrow(string specName, string fileName)
{
    // Arrange
    var factory = new OpenApiPersistableFactory();
    using var stream = OpenApiSpecs.GetStream(fileName);
    var config = ImportConfigurations.CQRSMode();
    PackageModelPersistable[] packages = [PackageModels.WithTypeDefinitions()];

    // Act
    var exception = Record.Exception(() =>
    {
        factory.GetPersistables(stream, config, packages);
    });

    // Assert
    exception.ShouldBeNull($"CQRS mode should not throw for {specName}");
}
```

### Fact Tests with Specific Assertions
```csharp
[Fact]
public void GetPersistables_PetStore_CQRS_CreatesCommandsAndQueries()
{
    // Arrange
    var factory = new OpenApiPersistableFactory();
    using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
    var config = ImportConfigurations.CQRSMode();
    PackageModelPersistable[] packages = [PackageModels.WithTypeDefinitions()];

    // Act
    var result = factory.GetPersistables(stream, config, packages);

    // Assert
    result.Elements.ShouldContain(c => c.SpecializationType == "Command");
    result.Elements.ShouldContain(c => c.SpecializationType == "Query");
}
```

## Module Implementation

This test project validates the module's `OpenApiPersistableFactory` located in `Intent.Modules.OpenApi.Importer.Importer` namespace. The factory uses `OpenApiImportConfig` for configuration, which includes `ServiceType` (CQRS/Service) and `SettingPersistence` (None/All) enums.

Key differences from CLI implementation:
- Uses `Intent.Modules.OpenApi.Importer.Importer.OpenApiPersistableFactory`
- Configuration via `OpenApiImportConfig` class
- Integrated with Intent module infrastructure via `ModuleTaskBase`

Following these guidelines keeps the suite expressive, fast, and easy for both humans and AI agents to evolve.
