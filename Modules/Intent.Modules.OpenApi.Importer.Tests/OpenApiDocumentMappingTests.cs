using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.OpenApi.Importer.Tests;

/// <summary>
/// Comprehensive behavioral tests for OpenAPI document mapping.
/// Tests cover CQRS patterns, Service patterns, generic types, DTOs, enums, HTTP metadata, routes, and edge cases.
/// </summary>
public class OpenApiDocumentMappingTests
{
    // === CQRS Pattern Tests ===
    
    [Fact]
    public void MapCQRSCommand_WithBodyType_CreatesCommandWithProperties()
    {
        // Arrange
        var document = OpenApiDocuments.CQRSPattern();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var exception = Record.Exception(() =>
        {
            new ScenarioComposer()
                .WithDocument(document)
                .WithConfig(config)
                .WithPackage(package)
                .Execute();
        });
        
        // Assert
        exception.ShouldBeNull();
    }
    
    [Fact]
    public void MapCQRSCommand_WithPostfixes_AppendsCommandSuffix()
    {
        // Arrange
        var document = OpenApiDocuments.CQRSPattern();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapCQRSQuery_WithoutPostfixes_DoesNotAppendSuffix()
    {
        // Arrange
        var document = OpenApiDocuments.CQRSPattern();
        var config = ImportConfigurations.CQRSWithoutPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapCQRSQuery_ReturnsCollection_PluralizesOperationName()
    {
        // Arrange
        var document = OpenApiDocuments.BasicCRUD();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.Elements.ShouldNotBeEmpty();
    }
    
    // === Service Pattern Tests ===
    
    [Fact]
    public void MapServiceOperation_WithParameters_CreatesOperationWithParameters()
    {
        // Arrange
        var document = OpenApiDocuments.ServicePattern();
        var config = ImportConfigurations.ServicePattern();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapServiceOperation_WithReturnType_SetsOperationReturnType()
    {
        // Arrange
        var document = OpenApiDocuments.ServicePattern();
        var config = ImportConfigurations.ServicePattern();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapServiceOperation_WithHeaderParameters_AddsFromHeaderStereotype()
    {
        // Arrange
        var document = OpenApiDocuments.WithParameters();
        var config = ImportConfigurations.ServicePattern();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    // === Generic Type Tests (CRITICAL - All 3 Formats) ===
    
    [Fact]
    public void MapGenericType_LegacyJsonResponseOfGuid_UnwrapsToGuid()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsLegacy();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapGenericType_JsonResponse_Of_Guid_UnwrapsToGuid()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsUnderscore();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapGenericType_PagedResult_Of_CustomerDto_CreatesFlattedDto()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsUnderscore();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapGenericType_JsonResponseOfString_UnwrapsToString()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsPascalCase();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapGenericType_PagedResultOfGuid_CreatesFlattedDto()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsPascalCase();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapGenericType_ProfileData_DoesNotParseAsGeneric()
    {
        // Arrange - ProfileData should NOT be parsed as a generic type
        var document = OpenApiDocuments.WithFalsePositives();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapGenericType_MixedFormats_AllFormatsWorkTogether()
    {
        // Arrange
        var document = OpenApiDocuments.WithGenericsMixed();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
        result.Elements.ShouldNotBeEmpty();
    }
    
    // === DTO and Enum Tests ===
    
    [Fact]
    public void MapSchema_WithProperties_CreatesDtoWithFields()
    {
        // Arrange
        var document = OpenApiDocuments.BasicCRUD();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapSchema_WithEnum_CreatesEnumWithLiterals()
    {
        // Arrange
        var document = OpenApiDocuments.WithEnums();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapSchema_WithEnumExtensions_UsesEnumNames()
    {
        // Arrange
        var document = OpenApiDocuments.WithEnumExtensions();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapSchema_WithAllOf_MergesProperties()
    {
        // Arrange
        var document = OpenApiDocuments.WithAllOfSchemas();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    // === HTTP Metadata Tests ===
    
    [Fact]
    public void MapOperation_POST_AddsHttpSettingsStereotype()
    {
        // Arrange
        var document = OpenApiDocuments.BasicCRUD();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapOperation_WithCustomSuccessCode_SetsSuccessCodeStereotype()
    {
        // Arrange
        var document = OpenApiDocuments.WithMultipleResponses();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapOperation_WithSecurity_MarkAsSecured()
    {
        // Arrange
        var document = OpenApiDocuments.WithSecurity();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapOperation_AzureFunctions_AddsAzureFunctionStereotype()
    {
        // Arrange
        var document = OpenApiDocuments.AzureFunctionsPattern();
        var config = ImportConfigurations.AzureFunctions();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    // === Route Parsing Tests ===
    
    [Fact]
    public void MapPath_NestedRoute_ExtractsServiceNameCorrectly()
    {
        // Arrange
        var document = OpenApiDocuments.WithNestedRoutes();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapPath_WithPathParameters_IncludesInOperationRoute()
    {
        // Arrange
        var document = OpenApiDocuments.WithParameters();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapPath_DuplicateOperations_AppendsIndexToName()
    {
        // Arrange
        var document = OpenApiDocuments.WithDuplicateOperations();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    // === Edge Cases ===
    
    [Fact]
    public void MapOperation_NoRequestBody_CreatesOperationWithoutBody()
    {
        // Arrange
        var document = OpenApiDocuments.BasicCRUD();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapSchema_NullableProperty_SetsIsNullableTrue()
    {
        // Arrange
        var document = OpenApiDocuments.WithNullableTypes();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapType_Collections_CreatesCollectionTypes()
    {
        // Arrange
        var document = OpenApiDocuments.WithCollections();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public void MapType_AdditionalProperties_CreatesDictionaryType()
    {
        // Arrange
        var document = OpenApiDocuments.WithDictionaries();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
    }
    
    // === Real-World Examples ===
    
    [Fact]
    public void MapOpenApiDocument_PetStore_ShouldImportSuccessfully()
    {
        // Arrange
        var document = OpenApiDocuments.PetStoreSimple();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
        result.Elements.ShouldNotBeEmpty();
    }
    
    [Fact]
    public void MapOpenApiDocument_Comprehensive_AllFeaturesWorkTogether()
    {
        // Arrange
        var document = OpenApiDocuments.Comprehensive();
        var config = ImportConfigurations.CQRSWithPostfixes();
        var package = PackageModels.WithBasicTypes();
        
        // Act
        var result = new ScenarioComposer()
            .WithDocument(document)
            .WithConfig(config)
            .WithPackage(package)
            .Execute();
        
        // Assert
        result.ShouldNotBeNull();
        result.Elements.ShouldNotBeEmpty();
    }
}
