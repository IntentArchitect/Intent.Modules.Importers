# OpenAPI Importer Refactoring Implementation Guide

## Overview

This document provides a comprehensive guide for refactoring the OpenAPI Importer to move all CLI components into the module project, following the patterns established by the JSON and C# importers. The CLI project (`Intent.MetadataSynchronizer.OpenApi.CLI`) will be archived after this refactoring.

---

## Table of Contents

1. [Architecture Goals](#architecture-goals)
2. [Migration Strategy](#migration-strategy)
3. [Logging Pattern](#logging-pattern)
4. [Generic Type Handling](#generic-type-handling)
5. [Project Structure](#project-structure)
6. [Implementation Steps](#implementation-steps)
7. [Test Project Design](#test-project-design)
8. [Test Coverage Requirements](#test-coverage-requirements)
9. [Validation Checklist](#validation-checklist)

---

## Architecture Goals

### Current State
- **OpenAPI CLI** (`Intent.MetadataSynchronizer.OpenApi.CLI`): Standalone executable
- **OpenAPI Importer Module** (`Intent.Modules.OpenApi.Importer`): Spawns external process to run CLI
- Process-based communication with serialized configuration

### Target State
- **OpenAPI Importer Module**: Contains all business logic (migrated from CLI)
- **Direct Execution**: In-process method invocation using `ModuleTaskBase<T>`
- **CLI Project**: Archived (no longer maintained)
- **Shared Infrastructure**: Uses `Intent.Modules.Shared.FileImporter`

### Reference Patterns

**Follow**: JSON Importer pattern (direct integration + maintained CLI tool)
- Move code into module
- Direct method invocation
- Keep CLI structure but archive it

**Do NOT Follow**: RDBMS Importer pattern (external tool with build-time copy)
- No external process spawning
- No build-time binary copying

---

## Migration Strategy

### Phase 1: Move CLI Components to Module

Create the following folder structure in `Intent.Modules.OpenApi.Importer`:

```
Intent.Modules.OpenApi.Importer/
├── Importer/
│   ├── OpenApiPersistableFactory.cs          (from CLI)
│   ├── IOpenApiPresistableFactory.cs         (from CLI)
│   ├── AbstractServiceModel.cs                (from CLI)
│   ├── ImportConfig.cs                        (from CLI)
│   └── ServiceCreation/
│       ├── IServiceCreationStrategy.cs        (from CLI)
│       ├── ServiceCreationStrategyBase.cs     (from CLI - 658 lines)
│       ├── CQRSServiceCreationStrategy.cs     (from CLI)
│       ├── ServiceServiceCreationStrategy.cs  (from CLI)
│       └── DomainImplementation.cs            (from CLI)
└── Tasks/
    ├── OpenApiImport.cs                       (REFACTOR - replace process spawning)
    ├── OpenApiImportInputModel.cs             (NEW - replace ImportSettings)
    └── Helpers/                               (if needed)
```

**DO NOT MOVE**:
- `Program.cs` - CLI entry point (leave in CLI project)
- `Logging.cs` - Will be replaced with Intent logging

---

## Logging Pattern

### Current CLI Implementation (REMOVE)

```csharp
// Intent.MetadataSynchronizer.OpenApi.CLI/Logging.cs
internal class Logging
{
    public static void LogWarning(string message)
    {
        Console.WriteLine("Warning: " + message);
    }

    public static void LogError(string message)
    {
        Console.WriteLine("Error: " + message);
    }
}
```

### Target Implementation (USE)

**Add using statement**:
```csharp
using Intent.Utils;
```

**Replace all logging calls**:

| Current CLI Code | New Module Code |
|-----------------|-----------------|
| `Logging.LogWarning(message)` | `Logging.Log.Warning(message)` |
| `Logging.LogError(message)` | `Logging.Log.Failure(message)` |
| N/A | `Logging.Log.Info(message)` |

**Example from JSON Importer**:
```csharp
Logging.Log.Info($"Starting OpenAPI import from {config.OpenApiSpecificationFile} into package {config.PackageId}");
```

**Example from RDBMS Importer**:
```csharp
foreach (var message in result.Warnings)
{
    Logging.Log.Warning(message);
}

foreach (var message in result.Errors)
{
    Logging.Log.Failure(message);
}
```

---

## Generic Type Handling

### Critical: Three Generic Format Types

The OpenAPI importer must handle **three distinct generic type formats** as recently fixed in `ServiceCreationStrategyBase.cs`. These formats appear in OpenAPI schema references and must be correctly parsed and mapped.

---

### Format 1: Legacy C# Full Type Name (Backtick Syntax)

**Pattern**: `JsonResponse`1[[System.Guid, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=...]]`

**Example Reference IDs**:
```
JsonResponse`1[[System.Guid, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
JsonResponse`1[[MyApp.Domain.Customer, MyApp.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

**Parsing Logic** (Lines 53-61 in ServiceCreationStrategyBase.cs):
```csharp
// Backward compatibility for when JsonResponse was represented in full C# full type name.
if (name == "JsonResponse" && type != null)
{
    var start = type.OpenApiType.Reference.Id.IndexOf("JsonResponse`1[[", StringComparison.Ordinal) + "JsonResponse`1[[".Length;
    var end = type.OpenApiType.Reference.Id.IndexOf(",", start, StringComparison.Ordinal);
    var fullTypeName = type.OpenApiType.Reference.Id.Substring(start, end - start);
    var wrappedTypeName = fullTypeName[(fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1)..];
    result = OpenApiHelper.GetIntentType(wrappedTypeName.ToLower());
    return result;
}
```

**Behavior**:
- ✅ Detects `JsonResponse` with C# generic backtick syntax
- ✅ Extracts inner type from full CLR type name (e.g., `System.Guid` → `Guid`)
- ✅ Returns unwrapped inner type directly (no wrapper DTO created)
- ✅ Only applies to `JsonResponse` wrapper types
- ✅ Legacy format from older OpenAPI specifications

**Test Cases Required**:
- `JsonResponse`1[[System.Guid, ...]]` → unwraps to `guid`
- `JsonResponse`1[[System.String, ...]]` → unwraps to `string`
- `JsonResponse`1[[MyApp.CustomerDto, ...]]` → unwraps to `CustomerDto`

---

### Format 2: Underscore-Separated (GenericType_Of_Parameter)

**Pattern**: `JsonResponse_Of_Guid`, `PagedResult_Of_CustomerDto`, `Result_Of_System.String`

**Example Reference IDs**:
```
JsonResponse_Of_Guid
PagedResult_Of_CustomerDto
Result_Of_System.String
JsonResponse_Of_System.Collections.Generic.List_Of_CustomerDto
```

**Parsing Logic** (Lines 63-112 in ServiceCreationStrategyBase.cs):
```csharp
// Check for any generic type with _Of_ pattern (e.g., JsonResponse_Of_, PagedResult_Of_, etc.)
if (refId.Contains("_Of_"))
{
    var ofIndex = refId.IndexOf("_Of_", StringComparison.Ordinal);
    if (ofIndex != -1)
    {
        // Find the start of the generic type name (after the last dot before _Of_)
        var beforeOf = refId.Substring(0, ofIndex);
        var lastDotBeforeOf = beforeOf.LastIndexOf('.');
        var genericTypeStart = lastDotBeforeOf != -1 ? lastDotBeforeOf + 1 : 0;
        
        var fullGenericExpression = refId.Substring(genericTypeStart);
        
        // Extract the generic type name (before _Of_)
        var genericTypeName = fullGenericExpression.Substring(0, fullGenericExpression.IndexOf("_Of_", StringComparison.Ordinal));
        
        // Extract the parameter type name (after _Of_)
        var parameterTypeName = fullGenericExpression.Substring(fullGenericExpression.IndexOf("_Of_", StringComparison.Ordinal) + "_Of_".Length);
        
        // Extract just the type name (after the last dot, if any)
        var lastDotIndex = parameterTypeName.LastIndexOf(".", StringComparison.Ordinal);
        if (lastDotIndex != -1)
        {
            parameterTypeName = parameterTypeName.Substring(lastDotIndex + 1);
        }
        
        // For JsonResponse, unwrap and return the inner type directly
        if (genericTypeName.Equals("JsonResponse", StringComparison.OrdinalIgnoreCase))
        {
            result = OpenApiHelper.GetIntentType(parameterTypeName.ToLower());
            return result;
        }
        
        // For other generic types (like PagedResult), create a distinct DTO name
        name = $"{genericTypeName}Of{parameterTypeName}";
        externalReference = $"{folder.Name}.{name}";
        key = externalReference;
        
        // Check if we've already created this combined type
        if (_addedServiceTypes.TryGetValue(key, out result))
        {
            return result;
        }
        
        // Fall through to normal DTO creation with the modified name
    }
}
```

**Behavior**:
- ✅ Detects underscore-separated format: `GenericType_Of_Parameter`
- ✅ Handles namespace-qualified parameter types (e.g., `System.String` → `String`)
- ✅ **JsonResponse**: Unwraps and returns inner type directly
- ✅ **Other generics** (PagedResult, Result, etc.): Creates flattened DTO with name `PagedResultOfGuid`
- ✅ Caches created types to prevent duplicates
- ✅ Strips namespace from parameter type name

**Test Cases Required**:
- `JsonResponse_Of_Guid` → unwraps to `guid`
- `JsonResponse_Of_System.String` → unwraps to `string`
- `PagedResult_Of_CustomerDto` → creates DTO named `PagedResultOfCustomerDto`
- `PagedResult_Of_System.Guid` → creates DTO named `PagedResultOfGuid`
- Duplicate detection: same type referenced twice should reuse existing DTO

---

### Format 3: PascalCase (GenericTypeOfParameter)

**Pattern**: `JsonResponseOfGuid`, `PagedResultOfCustomerDto`, `ResultOfString`

**Example Reference IDs**:
```
JsonResponseOfGuid
PagedResultOfCustomerDto
ResultOfString
PagedResultOfListOfCustomerDto
```

**Parsing Logic** (Lines 114-150 in ServiceCreationStrategyBase.cs):
```csharp
// Check for simplified format: GenericTypeOfParameter (e.g., PagedResultOfGuid, JsonResponseOfString)
else
{
    // Extract the last segment after the final dot (if any) to get the simple type name
    var lastDotIndex = refId.LastIndexOf('.');
    var simpleTypeName = lastDotIndex != -1 ? refId.Substring(lastDotIndex + 1) : refId;
    
    // Look for "Of" pattern in PascalCase (must have at least one char before and after "Of")
    var ofIndex = simpleTypeName.IndexOf("Of", StringComparison.Ordinal);
    if (ofIndex > 0 && ofIndex + 2 < simpleTypeName.Length)
    {
        // Ensure "Of" is followed by an uppercase letter (PascalCase pattern)
        var charAfterOf = simpleTypeName[ofIndex + 2];
        if (char.IsUpper(charAfterOf))
        {
            var genericTypeName = simpleTypeName.Substring(0, ofIndex);
            var parameterTypeName = simpleTypeName.Substring(ofIndex + 2);
            
            // For JsonResponse, unwrap and return the inner type directly
            if (genericTypeName.Equals("JsonResponse", StringComparison.OrdinalIgnoreCase))
            {
                result = OpenApiHelper.GetIntentType(parameterTypeName.ToLower());
                return result;
            }
            
            // For other generic types (like PagedResult), create a distinct DTO name
            name = simpleTypeName; // Use the full name as-is (e.g., "PagedResultOfGuid")
            externalReference = $"{folder.Name}.{name}";
            key = externalReference;
            
            // Check if we've already created this combined type
            if (_addedServiceTypes.TryGetValue(key, out result))
            {
                return result;
            }
            
            // Fall through to normal DTO creation with the modified name
        }
    }
}
```

**Behavior**:
- ✅ Detects PascalCase format: `GenericTypeOfParameter`
- ✅ Validates that "Of" is followed by uppercase letter (avoids false positives like "ProfileData")
- ✅ Splits on "Of" to extract generic type and parameter type
- ✅ **JsonResponse**: Unwraps and returns inner type directly
- ✅ **Other generics**: Uses full name as-is (e.g., `PagedResultOfGuid`)
- ✅ Caches created types to prevent duplicates

**Test Cases Required**:
- `JsonResponseOfGuid` → unwraps to `guid`
- `JsonResponseOfString` → unwraps to `string`
- `PagedResultOfCustomerDto` → creates DTO named `PagedResultOfCustomerDto`
- `ResultOfListOfCustomerDto` → creates DTO named `ResultOfListOfCustomerDto`
- False positive prevention: `ProfileData` should NOT be parsed as generic (no uppercase after "Of")

---

### Generic Type Handling Summary

| Format | Example | JsonResponse Behavior | Other Generics Behavior |
|--------|---------|----------------------|-------------------------|
| **Legacy C# Backtick** | `JsonResponse`1[[System.Guid, ...]]` | Unwrap to inner type | N/A (only JsonResponse uses this) |
| **Underscore** | `PagedResult_Of_CustomerDto` | Unwrap to inner type | Create DTO: `PagedResultOfCustomerDto` |
| **PascalCase** | `PagedResultOfCustomerDto` | Unwrap to inner type | Create DTO: `PagedResultOfCustomerDto` |

**Key Rules**:
1. **JsonResponse is ALWAYS unwrapped** - never create a DTO wrapper
2. **Other generics create flattened DTOs** - e.g., `PagedResultOfGuid`
3. **Namespace stripping** - `System.String` becomes `String`
4. **Deduplication** - use `_addedServiceTypes` cache with external reference as key
5. **Validation** - PascalCase format requires uppercase after "Of"

---

## Project Structure

### Update Intent.Modules.OpenApi.Importer.csproj

**Add Package References**:
```xml
<ItemGroup>
  <PackageReference Include="Intent.Architect.Persistence" Version="3.7.4" />
  <PackageReference Include="Intent.Modules.Common" Version="3.7.2" />
  <PackageReference Include="Intent.Modules.Common.CSharp" Version="3.8.1" />
  <PackageReference Include="Intent.Modules.Common.Types" Version="4.0.0" />
  <PackageReference Include="Intent.Packager" Version="3.5.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Intent.RoslynWeaver.Attributes" Version="2.1.7" />
  <PackageReference Include="Intent.SoftwareFactory.SDK" Version="3.7.0" />
  
  <!-- OpenAPI-specific dependencies -->
  <PackageReference Include="Intent.Modules.Modelers.Services" Version="4.0.1" />
  <PackageReference Include="Intent.Modules.Modelers.Services.CQRS" Version="6.0.1" />
  <PackageReference Include="Microsoft.OpenApi.Readers" Version="1.6.15" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="..\Intent.MetadataSynchronizer\Intent.MetadataSynchronizer.csproj" />
</ItemGroup>

<Import Project="..\Intent.Modules.Shared.FileImporter\Intent.Modules.Shared.FileImporter.projitems" Label="Shared" />
```

---

## Implementation Steps

### Step 1: Create Folder Structure

1. Create `Intent.Modules.OpenApi.Importer/Importer/` folder
2. Create `Intent.Modules.OpenApi.Importer/Importer/ServiceCreation/` subfolder

### Step 2: Copy and Adapt Files

**Copy from CLI to Module** (with adaptations):

1. **OpenApiPersistableFactory.cs**:
   - Copy to `Importer/OpenApiPersistableFactory.cs`
   - Change namespace: `Intent.MetadataSynchronizer.OpenApi.CLI` → `Intent.Modules.OpenApi.Importer.Importer`
   - Replace `Logging.LogWarning()` → `Logging.Log.Warning()`
   - Replace `Logging.LogError()` → `Logging.Log.Failure()`
   - Add `using Intent.Utils;`

2. **IOpenApiPresistableFactory.cs**:
   - Copy to `Importer/IOpenApiPresistableFactory.cs`
   - Change namespace

3. **AbstractServiceModel.cs**:
   - Copy to `Importer/AbstractServiceModel.cs`
   - Change namespace
   - Contains: `AbstractServiceOperationModel`, `Parameter`, `ResolvedType` classes

4. **ImportConfig.cs**:
   - Copy to `Importer/ImportConfig.cs`
   - Change namespace
   - Contains: `ImportConfig`, `ServiceType` enum, `SettingPersistence` enum

5. **ServiceCreation/*.cs** (4 files):
   - Copy all files to `Importer/ServiceCreation/`
   - Change namespace: `Intent.MetadataSynchronizer.OpenApi.CLI.ServiceCreation` → `Intent.Modules.OpenApi.Importer.Importer.ServiceCreation`
   - Replace all logging calls with Intent logging
   - Files: `IServiceCreationStrategy.cs`, `ServiceCreationStrategyBase.cs`, `CQRSServiceCreationStrategy.cs`, `ServiceServiceCreationStrategy.cs`, `DomainImplementation.cs`

### Step 3: Create OpenApiImportInputModel

Create `Tasks/OpenApiImportInputModel.cs`:

```csharp
using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tasks;

public class OpenApiImportInputModel
{
    public string OpenApiSpecificationFile { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public string? TargetFolderId { get; set; }
    public ServiceType ServiceType { get; set; } = ServiceType.CQRS;
    public bool AddPostFixes { get; set; } = true;
    public bool IsAzureFunctions { get; set; }
    public bool AllowRemoval { get; set; } = true;
    public SettingPersistence SettingPersistence { get; set; } = SettingPersistence.None;
    public bool ReverseEngineerImplementation { get; set; }
    public string? DomainPackageId { get; set; }
}
```

### Step 4: Refactor OpenApiImport.cs

Replace process-spawning implementation with direct execution:

```csharp
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tasks.Helpers;
using Intent.Modules.OpenApi.Importer.Tasks.Models;
using Intent.Utils;

namespace Intent.Modules.OpenApi.Importer.Tasks;

public class OpenApiImport : ModuleTaskBase<OpenApiImportInputModel>
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public OpenApiImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public override string TaskTypeId => "Intent.Modules.OpenApi.Importer.Tasks.OpenApiImport";
    public override string TaskTypeName => "OpenApi Document Import";

    protected override ValidationResult ValidateInputModel(OpenApiImportInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.OpenApiSpecificationFile))
            return ValidationResult.ErrorResult("OpenAPI specification file is required.");

        if (!File.Exists(inputModel.OpenApiSpecificationFile) && !inputModel.OpenApiSpecificationFile.StartsWith("http"))
            return ValidationResult.ErrorResult($"OpenAPI specification file does not exist: {inputModel.OpenApiSpecificationFile}");

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
            return ValidationResult.ErrorResult("Package ID is required.");

        // Validate that the package exists
        var application = _configurationProvider.GetApplicationConfig();
        if (!_metadataManager.TryGetApplicationPackage(application.Id, "Services", inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult($"Package validation failed: {errorMessage}");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(OpenApiImportInputModel inputModel)
    {
        var executionResult = new ExecuteResult();

        // Get .isln file path
        var islnFile = Directory.GetFiles(_configurationProvider.GetSolutionConfig().SolutionRootLocation, "*.isln").First();

        // Get application config
        var application = _configurationProvider.GetApplicationConfig();

        // Build ImportConfig from UI input
        var config = new ImportConfig
        {
            OpenApiSpecificationFile = inputModel.OpenApiSpecificationFile,
            IslnFile = islnFile,
            ApplicationName = application.Name,
            PackageId = inputModel.PackageId,
            TargetFolderId = inputModel.TargetFolderId,
            ServiceType = inputModel.ServiceType,
            AddPostFixes = inputModel.AddPostFixes,
            IsAzureFunctions = inputModel.IsAzureFunctions,
            AllowRemoval = inputModel.AllowRemoval,
            SettingPersistence = inputModel.SettingPersistence,
            ReverseEngineerImplementation = inputModel.ReverseEngineerImplementation,
            DomainPackageId = inputModel.DomainPackageId
        };

        Logging.Log.Info($"Starting OpenAPI import from {config.OpenApiSpecificationFile} into package {config.PackageId}");

        // Create factory
        var factory = new OpenApiPersistableFactory();

        // Invoke the core synchronizer logic directly (no process spawn)
        Intent.MetadataSynchronizer.Helpers.Execute(
            intentSolutionPath: config.IslnFile,
            applicationName: config.ApplicationName,
            designerName: "Services",
            packageId: config.PackageId,
            targetFolderId: config.TargetFolderId,
            deleteExtra: config.AllowRemoval,
            debug: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge,
            additionalPreconditionChecks: null,
            getPersistables: packages => factory.GetPersistables(config, packages),
            persistAdditionalMetadata: package => factory.PersistAdditionalMetadata(package),
            packageTypeId: "df45eaf6-9202-4c25-8dd5-677e9ba1e906");

        // Handle domain reverse engineering if enabled
        if (config.ReverseEngineerImplementation && !string.IsNullOrWhiteSpace(config.DomainPackageId))
        {
            Logging.Log.Info($"Reverse engineering domain implementation into package {config.DomainPackageId}");
            
            Intent.MetadataSynchronizer.Helpers.Execute(
                intentSolutionPath: config.IslnFile,
                applicationName: config.ApplicationName,
                designerName: "Domain",
                packageId: config.DomainPackageId,
                targetFolderId: null,
                deleteExtra: false,
                debug: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge,
                additionalPreconditionChecks: null,
                getPersistables: packages => factory.GetDomainPersistables(),
                persistAdditionalMetadata: null,
                packageTypeId: "1a824508-4623-45d9-accc-f572091ade5a");
        }

        return executionResult;
    }
}
```

### Step 5: Delete Obsolete Files

In `Intent.Modules.OpenApi.Importer/Tasks/`:
- Delete or archive `ImportSettings.cs` (replaced by `OpenApiImportInputModel`)

---

## Test Project Design

### Create Intent.Modules.OpenApi.Importer.Tests

**Project Structure**:
```
Intent.Modules.OpenApi.Importer.Tests/
├── AI_GUIDE.md
├── Intent.Modules.OpenApi.Importer.Tests.csproj
├── OpenApiDocumentMappingTests.cs          (behavioral tests)
├── OpenApiComprehensiveMappingTests.cs     (snapshot tests)
└── TestData/
    ├── OpenApiDocuments.cs                 (Object Mother factory)
    ├── PackageModels.cs                    (Object Mother factory)
    ├── ImportConfigurations.cs             (Object Mother factory)
    ├── ScenarioComposer.cs                 (fluent scenario builder)
    └── SampleSchemas/                      (embedded OpenAPI files)
        ├── basic-crud.json                 (Simple GET/POST/PUT/DELETE)
        ├── cqrs-commands.json              (CQRS Commands and Queries)
        ├── service-operations.json         (Traditional Service pattern)
        ├── with-enums.json                 (String and integer enums)
        ├── with-enum-extensions.json       (x-enumNames for integer enums)
        ├── with-generics-legacy.json       (Legacy C# backtick format)
        ├── with-generics-underscore.json   (Underscore format)
        ├── with-generics-pascalcase.json   (PascalCase format)
        ├── with-generics-mixed.json        (All three formats in one doc)
        ├── with-allof-schemas.json         (Schema composition with allOf)
        ├── with-nullable-types.json        (Required vs optional properties)
        ├── with-collections.json           (Arrays and collections)
        ├── with-parameters.json            (Query, path, header parameters)
        ├── with-multiple-responses.json    (Various HTTP status codes)
        ├── with-security.json              (Security schemes)
        ├── with-nested-routes.json         (Deep route nesting)
        ├── with-duplicate-operations.json  (Duplicate operation names)
        ├── with-dictionaries.json          (additionalProperties)
        ├── with-false-positives.json       (ProfileData - not generic)
        ├── azure-functions.json            (Azure Functions pattern)
        ├── petstore-simple.json            (Simplified PetStore)
        └── comprehensive.json              (Combined feature test)
```

### Intent.Modules.OpenApi.Importer.Tests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Intent.Architect.Persistence" Version="3.7.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.3">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Shouldly" Version="4.3.0" />
    <PackageReference Include="Verify.Xunit" Version="28.5.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Intent.Modules.OpenApi.Importer\Intent.Modules.OpenApi.Importer.csproj" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="TestData\SampleSchemas\*.json" />
    <EmbeddedResource Include="TestData\SampleSchemas\*.yaml" />
  </ItemGroup>

</Project>
```

---

## Test Coverage Requirements

### Test Coverage Summary

| Category | Behavioral Tests | Snapshot Tests | Total |
|----------|-----------------|----------------|-------|
| CQRS Pattern | 4 | 1 | 5 |
| Service Pattern | 3 | 1 | 4 |
| Generic Types | 9 | 4 | 13 |
| DTOs & Enums | 4 | 2 | 6 |
| HTTP Metadata | 4 | 2 | 6 |
| Route Parsing | 4 | 2 | 6 |
| Edge Cases | 4 | 2 | 6 |
| Azure Functions | 1 | 1 | 2 |
| Real-World | 2 | 2 | 4 |
| Comprehensive | 0 | 1 | 1 |
| Domain RE | 0 | 1 | 1 |
| **TOTAL** | **35** | **21** | **56** |

### Behavioral Tests (OpenApiDocumentMappingTests.cs)

Use explicit assertions with Shouldly. Follow AAA pattern.

**Test Categories**:

#### 1. CQRS Pattern Tests
```csharp
[Fact]
public void MapCQRSCommand_WithBodyType_CreatesCommandWithProperties()

[Fact]
public void MapCQRSCommand_WithPostfixes_AppendsCommandSuffix()

[Fact]
public void MapCQRSQuery_WithoutPostfixes_DoesNotAppendSuffix()

[Fact]
public void MapCQRSQuery_ReturnsCollection_PluralizesOperationName()
```

#### 2. Service Pattern Tests
```csharp
[Fact]
public void MapServiceOperation_WithParameters_CreatesOperationWithParameters()

[Fact]
public void MapServiceOperation_WithReturnType_SetsOperationReturnType()

[Fact]
public void MapServiceOperation_WithHeaderParameters_AddsFromHeaderStereotype()
```

#### 3. Generic Type Tests (CRITICAL - All 3 Formats)
```csharp
// Format 1: Legacy C# Backtick
[Fact]
public void MapGenericType_LegacyJsonResponseOfGuid_UnwrapsToGuid()

[Fact]
public void MapGenericType_LegacyJsonResponseOfCustomType_UnwrapsToCustomType()

// Format 2: Underscore
[Fact]
public void MapGenericType_JsonResponse_Of_Guid_UnwrapsToGuid()

[Fact]
public void MapGenericType_PagedResult_Of_CustomerDto_CreatesFlattedDto()

[Fact]
public void MapGenericType_Underscore_WithNamespace_StripsNamespace()

// Format 3: PascalCase
[Fact]
public void MapGenericType_JsonResponseOfString_UnwrapsToString()

[Fact]
public void MapGenericType_PagedResultOfGuid_CreatesFlattedDto()

[Fact]
public void MapGenericType_ProfileData_DoesNotParseAsGeneric() // False positive check
```

#### 4. DTO and Enum Tests
```csharp
[Fact]
public void MapSchema_WithProperties_CreatesDtoWithFields()

[Fact]
public void MapSchema_WithEnum_CreatesEnumWithLiterals()

[Fact]
public void MapSchema_WithEnumExtensions_UsesEnumNames()

[Fact]
public void MapSchema_WithAllOf_MergesProperties()
```

#### 5. HTTP Metadata Tests
```csharp
[Fact]
public void MapOperation_POST_AddsHttpSettingsStereotype()

[Fact]
public void MapOperation_WithCustomSuccessCode_SetsSuccessCodeStereotype()

[Fact]
public void MapOperation_WithSecurity_MarkAsSecured()

[Fact]
public void MapOperation_AzureFunctions_AddsAzureFunctionStereotype()
```

#### 6. Route Parsing Tests
```csharp
[Fact]
public void MapPath_NestedRoute_ExtractsServiceNameCorrectly()

[Fact]
public void MapPath_WithPathParameters_IncludesInOperationRoute()

[Fact]
public void MapPath_MultipleOperations_DetectsDuplicates()

[Fact]
public void MapPath_DuplicateOperations_AppendsIndexToName()
```

#### 7. Edge Cases
```csharp
[Fact]
public void MapOperation_NoRequestBody_CreatesOperationWithoutBody()

[Fact]
public void MapOperation_NoResponse_CreatesOperationWithoutReturnType()

[Fact]
public void MapSchema_NullableProperty_SetsIsNullableTrue()

[Fact]
public void MapType_AdditionalProperties_CreatesDictionaryType()
```

---

### Snapshot Tests (OpenApiComprehensiveMappingTests.cs)

Use Verify library with `.UseParameters()` for unique snapshot names.

**Test Categories** (21 total snapshot tests):

```csharp
// === Basic Operations ===
[Fact]
public async Task MapOpenApiDocument_BasicCRUD_ShouldMatchSnapshot()

// === Pattern Tests ===
[Fact]
public async Task MapOpenApiDocument_CQRSPattern_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_ServicePattern_ShouldMatchSnapshot()

// === Enum Tests ===
[Fact]
public async Task MapOpenApiDocument_WithEnums_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithEnumExtensions_ShouldMatchSnapshot()

// === Generic Type Tests (CRITICAL - 4 tests) ===
[Fact]
public async Task MapOpenApiDocument_WithGenericsLegacy_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithGenericsUnderscore_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithGenericsPascalCase_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithGenericsMixed_ShouldMatchSnapshot()

// === Schema Composition ===
[Fact]
public async Task MapOpenApiDocument_WithAllOfSchemas_ShouldMatchSnapshot()

// === Type Tests ===
[Fact]
public async Task MapOpenApiDocument_WithNullableTypes_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithCollections_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithDictionaries_ShouldMatchSnapshot()

// === Parameter Tests ===
[Fact]
public async Task MapOpenApiDocument_WithParameters_ShouldMatchSnapshot()

// === HTTP Metadata ===
[Fact]
public async Task MapOpenApiDocument_WithMultipleResponses_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithSecurity_ShouldMatchSnapshot()

// === Route Tests ===
[Fact]
public async Task MapOpenApiDocument_WithNestedRoutes_ShouldMatchSnapshot()

[Fact]
public async Task MapOpenApiDocument_WithDuplicateOperations_ShouldMatchSnapshot()

// === Azure Functions ===
[Fact]
public async Task MapOpenApiDocument_AzureFunctions_ShouldMatchSnapshot()

// === Real-World Examples ===
[Fact]
public async Task MapOpenApiDocument_PetStore_ShouldMatchSnapshot()

// === Comprehensive Test ===
[Fact]
public async Task MapOpenApiDocument_Comprehensive_ShouldMatchSnapshot()

// === Domain Reverse Engineering ===
[Fact]
public async Task MapOpenApiDocument_WithDomainReverseEngineering_ShouldMatchSnapshot()
```

---

### Object Mother Factories

#### OpenApiDocuments.cs

```csharp
using System.IO;
using System.Reflection;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for OpenAPI document test data.
/// Each method returns a Stream containing an embedded OpenAPI schema JSON file
/// designed to test specific importer features.
/// </summary>
internal static class OpenApiDocuments
{
    // === Basic CRUD Operations ===
    /// <summary>
    /// Simple CRUD operations: GET (list), GET (single), POST, PUT, DELETE.
    /// Tests basic HTTP verb mapping and route extraction.
    /// </summary>
    public static Stream BasicCRUD() => LoadEmbeddedResource("basic-crud.json");
    
    // === CQRS Pattern ===
    /// <summary>
    /// CQRS pattern with Commands (POST, PUT, DELETE) and Queries (GET).
    /// Tests command/query naming, postfix handling, and return types.
    /// </summary>
    public static Stream CQRSPattern() => LoadEmbeddedResource("cqrs-commands.json");
    
    // === Service Pattern ===
    /// <summary>
    /// Traditional service operations with methods and parameters.
    /// Tests service-style operation mapping.
    /// </summary>
    public static Stream ServicePattern() => LoadEmbeddedResource("service-operations.json");
    
    // === Enum Handling ===
    /// <summary>
    /// String and integer enums without extensions.
    /// Tests basic enum literal extraction.
    /// </summary>
    public static Stream WithEnums() => LoadEmbeddedResource("with-enums.json");
    
    /// <summary>
    /// Integer enums with x-enumNames extension.
    /// Tests enum name mapping from OpenAPI extensions.
    /// </summary>
    public static Stream WithEnumExtensions() => LoadEmbeddedResource("with-enum-extensions.json");
    
    // === Generic Type Handling (CRITICAL) ===
    /// <summary>
    /// Legacy C# backtick format: JsonResponse`1[[System.Guid, ...]].
    /// Tests backward compatibility with full CLR type names.
    /// </summary>
    public static Stream WithGenericsLegacy() => LoadEmbeddedResource("with-generics-legacy.json");
    
    /// <summary>
    /// Underscore format: JsonResponse_Of_Guid, PagedResult_Of_CustomerDto.
    /// Tests namespace stripping and DTO flattening.
    /// </summary>
    public static Stream WithGenericsUnderscore() => LoadEmbeddedResource("with-generics-underscore.json");
    
    /// <summary>
    /// PascalCase format: JsonResponseOfGuid, PagedResultOfCustomerDto.
    /// Tests PascalCase parsing and false positive prevention (ProfileData).
    /// </summary>
    public static Stream WithGenericsPascalCase() => LoadEmbeddedResource("with-generics-pascalcase.json");
    
    /// <summary>
    /// All three generic formats in one document.
    /// Tests that all formats work together without conflicts.
    /// </summary>
    public static Stream WithGenericsMixed() => LoadEmbeddedResource("with-generics-mixed.json");
    
    // === Schema Composition ===
    /// <summary>
    /// Schemas using allOf for inheritance/composition.
    /// Tests property merging from multiple schemas.
    /// </summary>
    public static Stream WithAllOfSchemas() => LoadEmbeddedResource("with-allof-schemas.json");
    
    // === Nullability ===
    /// <summary>
    /// Required vs optional properties, nullable types.
    /// Tests IsNullable flag and required property handling.
    /// </summary>
    public static Stream WithNullableTypes() => LoadEmbeddedResource("with-nullable-types.json");
    
    // === Collections ===
    /// <summary>
    /// Arrays and collections in request/response bodies.
    /// Tests IsCollection flag and array handling.
    /// </summary>
    public static Stream WithCollections() => LoadEmbeddedResource("with-collections.json");
    
    // === Parameters ===
    /// <summary>
    /// Query, path, and header parameters.
    /// Tests parameter extraction and stereotype application.
    /// </summary>
    public static Stream WithParameters() => LoadEmbeddedResource("with-parameters.json");
    
    // === HTTP Metadata ===
    /// <summary>
    /// Multiple HTTP status codes (200, 201, 204, 400, 404, 500).
    /// Tests success code mapping and error response handling.
    /// </summary>
    public static Stream WithMultipleResponses() => LoadEmbeddedResource("with-multiple-responses.json");
    
    // === Security ===
    /// <summary>
    /// Security schemes (OAuth, API Key, Bearer).
    /// Tests secured operation detection.
    /// </summary>
    public static Stream WithSecurity() => LoadEmbeddedResource("with-security.json");
    
    // === Route Parsing ===
    /// <summary>
    /// Nested routes: /api/customers/{id}/orders/{orderId}/items.
    /// Tests service name extraction and concept name derivation.
    /// </summary>
    public static Stream WithNestedRoutes() => LoadEmbeddedResource("with-nested-routes.json");
    
    /// <summary>
    /// Duplicate operation names within the same service.
    /// Tests automatic renaming with index suffixes (CreateCustomer1, CreateCustomer2).
    /// </summary>
    public static Stream WithDuplicateOperations() => LoadEmbeddedResource("with-duplicate-operations.json");
    
    // === Dictionary Types ===
    /// <summary>
    /// Schemas with additionalProperties (Dictionary/Map types).
    /// Tests generic Dictionary&lt;string, TValue&gt; creation.
    /// </summary>
    public static Stream WithDictionaries() => LoadEmbeddedResource("with-dictionaries.json");
    
    // === False Positives ===
    /// <summary>
    /// Types like ProfileData that should NOT be parsed as generics.
    /// Tests validation that "Of" is followed by uppercase letter.
    /// </summary>
    public static Stream WithFalsePositives() => LoadEmbeddedResource("with-false-positives.json");
    
    // === Azure Functions ===
    /// <summary>
    /// Azure Functions HTTP trigger pattern.
    /// Tests Azure Function stereotype application.
    /// </summary>
    public static Stream AzureFunctionsPattern() => LoadEmbeddedResource("azure-functions.json");
    
    // === Real-World Examples ===
    /// <summary>
    /// Simplified Swagger PetStore example.
    /// Tests realistic API structure.
    /// </summary>
    public static Stream PetStoreSimple() => LoadEmbeddedResource("petstore-simple.json");
    
    /// <summary>
    /// Comprehensive test combining multiple features:
    /// - CQRS + Service patterns
    /// - All three generic formats
    /// - Enums, parameters, security
    /// - Nested routes, multiple responses
    /// Tests that all features work together.
    /// </summary>
    public static Stream Comprehensive() => LoadEmbeddedResource("comprehensive.json");

    private static Stream LoadEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Intent.Modules.OpenApi.Importer.Tests.TestData.SampleSchemas.{fileName}";
        return assembly.GetManifestResourceStream(resourceName) 
               ?? throw new FileNotFoundException($"Embedded resource not found: {resourceName}");
    }
}
```

#### ImportConfigurations.cs

```csharp
using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

internal static class ImportConfigurations
{
    public static ImportConfig CQRSWithPostfixes() => new()
    {
        ServiceType = ServiceType.CQRS,
        AddPostFixes = true,
        AllowRemoval = false,
        IsAzureFunctions = false,
        ReverseEngineerImplementation = false
    };

    public static ImportConfig CQRSWithoutPostfixes() => new()
    {
        ServiceType = ServiceType.CQRS,
        AddPostFixes = false,
        AllowRemoval = false
    };

    public static ImportConfig ServicePattern() => new()
    {
        ServiceType = ServiceType.Service,
        AddPostFixes = false,
        AllowRemoval = false
    };

    public static ImportConfig AzureFunctions() => new()
    {
        ServiceType = ServiceType.CQRS,
        IsAzureFunctions = true,
        AddPostFixes = true
    };

    public static ImportConfig WithDomainReverseEngineering() => new()
    {
        ServiceType = ServiceType.CQRS,
        ReverseEngineerImplementation = true,
        DomainPackageId = "test-domain-package-id"
    };

    public static ImportConfig WithAllowRemoval() => new()
    {
        ServiceType = ServiceType.CQRS,
        AllowRemoval = true
    };
}
```

#### PackageModels.cs

```csharp
using Intent.IArchitect.Agent.Persistence.Model;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

internal static class PackageModels
{
    public static PackageModelPersistable Empty() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TestServicesPackage",
        Classes = new List<ElementPersistable>(),
        Associations = new List<AssociationPersistable>()
    };

    public static PackageModelPersistable WithExistingCommand(string commandName)
    {
        var package = Empty();
        var command = ElementPersistable.Create(
            specializationType: "Command",
            specializationTypeId: "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9",
            name: commandName,
            parentId: null,
            externalReference: $"Commands.{commandName}");
        package.Classes.Add(command);
        return package;
    }

    public static PackageModelPersistable WithExistingDTO(string dtoName)
    {
        var package = Empty();
        var dto = ElementPersistable.Create(
            specializationType: "DTO",
            specializationTypeId: "fee0edca-4aa0-4f77-a524-6bbd84e78734",
            name: dtoName,
            parentId: null,
            externalReference: $"DTOs.{dtoName}");
        package.Classes.Add(dto);
        return package;
    }
}
```

---

### Test Example Pattern

```csharp
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tests.TestData;
using Shouldly;
using Xunit;

namespace Intent.Modules.OpenApi.Importer.Tests;

public class OpenApiDocumentMappingTests
{
    [Fact]
    public void MapCQRSCommand_WithBodyType_CreatesCommandWithProperties()
    {
        // Arrange
        var document = OpenApiDocuments.SimpleCQRSExample();
        var package = PackageModels.Empty();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var factory = new OpenApiPersistableFactory();

        // Act
        var persistables = factory.GetPersistables(document, config, new[] { package });

        // Assert - check that commands were created
        var commands = persistables.Elements
            .Where(e => e.SpecializationType == "Command")
            .ToList();
        
        commands.ShouldNotBeEmpty();
        commands.ShouldContain(c => c.Name == "CreateCustomerCommand");
        
        var createCommand = commands.First(c => c.Name == "CreateCustomerCommand");
        createCommand.ChildElements.ShouldNotBeEmpty();
        createCommand.ChildElements.ShouldContain(p => p.Name == "Name");
        createCommand.ChildElements.ShouldContain(p => p.Name == "Email");
    }
}
```

---

## Validation Checklist

### Pre-Implementation
- [ ] Read and understand all three generic type formats
- [ ] Review JSON Importer implementation pattern
- [ ] Review RDBMS test patterns
- [ ] Understand `ModuleTaskBase<T>` usage

### During Implementation
- [ ] All CLI files copied to `Importer/` folder
- [ ] Namespaces updated correctly
- [ ] Logging replaced with `Intent.Utils.Logging.Log`
- [ ] Package references added to `.csproj`
- [ ] Shared project imported
- [ ] `OpenApiImport.cs` refactored to use `ModuleTaskBase`
- [ ] No process spawning code remains
- [ ] `OpenApiImportInputModel` created

### Test Implementation
- [ ] Test project created with correct structure
- [ ] All factory classes implemented (Object Mother pattern)
- [ ] 21 schema files created and embedded as resources
- [ ] Behavioral tests cover all feature areas (35+ tests)
- [ ] Snapshot tests use Verify library correctly (21 tests)
- [ ] Generic type tests cover all 3 formats (minimum 9 tests)
- [ ] Test data embedded as resources
- [ ] OpenApiDocuments factory has all 21 methods
- [ ] Each schema file documented with XML comments

### Post-Implementation
- [ ] All behavioral tests pass
- [ ] All snapshot tests reviewed and approved
- [ ] No compilation errors or warnings
- [ ] Manual testing with actual OpenAPI files
- [ ] CLI project marked as archived (README update)
- [ ] Documentation updated

---

## Success Criteria

1. ✅ **No External Process**: Module executes OpenAPI import in-process
2. ✅ **All Generics Work**: Three generic formats correctly parsed and mapped
3. ✅ **Logging Consistent**: Uses `Intent.Utils.Logging.Log` throughout
4. ✅ **Tests Comprehensive**: 35+ behavioral tests + 21 snapshot tests
5. ✅ **Schema Coverage Complete**: 21 test schemas covering all OpenAPI features
6. ✅ **Zero Regressions**: All existing OpenAPI import scenarios still work
7. ✅ **CLI Archived**: CLI project no longer maintained but preserved for reference

---

## Reference Files

**Study These Files**:
- `Intent.Modules.Json.Importer/Tasks/JsonImport.cs` - Direct execution pattern
- `Intent.Modules.Rdbms.Importer.Tests/AI_GUIDE.md` - Test documentation pattern
- `Intent.Modules.Rdbms.Importer.Tests/TestData/` - Object Mother pattern
- `Intent.MetadataSynchronizer.OpenApi.CLI/ServiceCreation/ServiceCreationStrategyBase.cs` - Generic handling logic (lines 53-150)

**Key Code Sections**:
- Lines 53-61: Legacy C# backtick format handling
- Lines 63-112: Underscore format handling
- Lines 114-150: PascalCase format handling

---

## Test Schema Creation Guidelines

When creating the 21 OpenAPI test schema files, follow these guidelines:

### Schema File Requirements

Each schema file should:
1. **Be Valid OpenAPI 3.0.1+** - Must parse with `Microsoft.OpenApi.Readers`
2. **Test Specific Features** - Focus on one or two features per file
3. **Be Minimal** - Only include what's needed to test the feature
4. **Be Realistic** - Mirror real-world API patterns
5. **Include Comments** - Use `description` fields to document test intent

### Recommended Content Structure

**Minimum Required**:
```json
{
  "openapi": "3.0.1",
  "info": { "title": "Test API", "version": "1.0" },
  "paths": { /* test operations */ },
  "components": { "schemas": { /* test schemas */ } }
}
```

### Priority Schema Files (Must Have)

1. **with-generics-legacy.json** - Critical for backward compatibility
2. **with-generics-underscore.json** - Critical for current format
3. **with-generics-pascalcase.json** - Critical for simplified format
4. **with-generics-mixed.json** - Critical to ensure no conflicts
5. **comprehensive.json** - Integration test for all features

### Optional Enhancement Schemas

If time permits, create these for edge cases:
- `with-multiple-content-types.json` - application/json, application/xml, text/plain
- `with-deprecated-operations.json` - Operations marked as deprecated
- `with-external-refs.json` - $ref pointing to external files
- `with-custom-response-headers.json` - Response headers in schema

---

## Notes

- The CLI project will remain in the repository but will not be maintained
- Ensure all three generic formats are tested with real-world OpenAPI files
- JsonResponse ALWAYS unwraps - this is critical behavior
- Other generics create flattened DTOs with combined names
- Deduplication via `_addedServiceTypes` cache is essential
- Namespace stripping only applies to underscore format
- Test schema files should be created incrementally - start with critical generics tests
- Use existing CLI test data (`Intent.MetadataSynchronizer.OpenApi.CLI/Data/`) as reference

---

## Questions?

If you encounter issues during implementation:
1. Review the reference patterns (JSON and RDBMS importers)
2. Check the generic handling logic in `ServiceCreationStrategyBase.cs`
3. Verify logging is using `Intent.Utils.Logging.Log`
4. Ensure test factories follow Object Mother pattern
5. Validate snapshot tests use Verify library correctly

Good luck with the implementation! 🚀
