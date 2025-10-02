# Intent.Modules.Rdbms.Importer.Tests – AI Companion Guide

This test project exercises the `DbSchemaIntentMetadataMerger` component without touching the file system or a real database. All test data lives in-memory and is produced through factory helpers so scenarios stay deterministic and fast.

## Project Layout

| Path | Purpose |
| --- | --- |
| `TestData/` | Object Mother factories and composition helpers used to build schemas, packages, and supporting metadata. |
| `DbSchemaIntentMetadataMergerTests.cs` | Behavioural tests covering merge scenarios. Each test is self-documenting through its name and the `Arrange / Act / Assert` sections. |
| `AI_GUIDE.md` (this file) | Orientation for future maintainers and AI agents. |

## Object Mother Strategy

The `TestData` folder contains the core building blocks:

- **`DatabaseSchemas`** – produces reusable `DatabaseSchema` instances (e.g. `WithCustomersAndOrders`, `WithCustomerWithNewAddressColumn`).
- **`Tables`, `Elements`, `Associations`, `Stereotypes`** – focused factories for low-level types.
- **`PackageModels`** – builds `PackageModelPersistable` instances that represent existing Intent metadata.
- **`ImportConfigurations`** – supplies the merger with consistent configuration objects.

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

- **Naming**: `MethodUnderTest_WhenCondition_ShouldOutcome` (e.g. `MergeSchemaAndPackage_NewTableDetected_AddsOnlyNewTable`).
- **Structure**: strictly follow the AAA pattern with comments `// Arrange`, `// Act`, `// Assert`. No other inline comments.
- **Assertions**: use Shouldly for readability (`ShouldContain`, `ShouldHaveSingleItem`, `ShouldBe`). Focus on the observable behaviour relevant to the scenario.
- **No snapshots**: tests rely on explicit assertions to stay clear and maintainable.

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
