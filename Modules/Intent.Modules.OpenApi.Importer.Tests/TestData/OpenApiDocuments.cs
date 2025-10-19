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
    // === Phase 1: Generic Type Handling (CRITICAL) ===
    
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
    
    /// <summary>
    /// Types like ProfileData that should NOT be parsed as generics.
    /// Tests validation that "Of" is followed by uppercase letter.
    /// </summary>
    public static Stream WithFalsePositives() => LoadEmbeddedResource("with-false-positives.json");
    
    // === Phase 2: Core Features ===
    
    /// <summary>
    /// Simple CRUD operations: GET (list), GET (single), POST, PUT, DELETE.
    /// Tests basic HTTP verb mapping and route extraction.
    /// </summary>
    public static Stream BasicCRUD() => LoadEmbeddedResource("basic-crud.json");
    
    /// <summary>
    /// CQRS pattern with Commands (POST, PUT, DELETE) and Queries (GET).
    /// Tests command/query naming, postfix handling, and return types.
    /// </summary>
    public static Stream CQRSPattern() => LoadEmbeddedResource("cqrs-commands.json");
    
    /// <summary>
    /// Traditional service operations with methods and parameters.
    /// Tests service-style operation mapping.
    /// </summary>
    public static Stream ServicePattern() => LoadEmbeddedResource("service-operations.json");
    
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
    
    // === Phase 3: Advanced Features ===
    
    /// <summary>
    /// Schemas using allOf for inheritance/composition.
    /// Tests property merging from multiple schemas.
    /// </summary>
    public static Stream WithAllOfSchemas() => LoadEmbeddedResource("with-allof-schemas.json");
    
    /// <summary>
    /// Required vs optional properties, nullable types.
    /// Tests IsNullable flag and required property handling.
    /// </summary>
    public static Stream WithNullableTypes() => LoadEmbeddedResource("with-nullable-types.json");
    
    /// <summary>
    /// Arrays and collections in request/response bodies.
    /// Tests IsCollection flag and array handling.
    /// </summary>
    public static Stream WithCollections() => LoadEmbeddedResource("with-collections.json");
    
    /// <summary>
    /// Query, path, and header parameters.
    /// Tests parameter extraction and stereotype application.
    /// </summary>
    public static Stream WithParameters() => LoadEmbeddedResource("with-parameters.json");
    
    /// <summary>
    /// Multiple HTTP status codes (200, 201, 204, 400, 404, 500).
    /// Tests success code mapping and error response handling.
    /// </summary>
    public static Stream WithMultipleResponses() => LoadEmbeddedResource("with-multiple-responses.json");
    
    /// <summary>
    /// Security schemes (OAuth, API Key, Bearer).
    /// Tests secured operation detection.
    /// </summary>
    public static Stream WithSecurity() => LoadEmbeddedResource("with-security.json");
    
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
    
    /// <summary>
    /// Schemas with additionalProperties (Dictionary/Map types).
    /// Tests generic Dictionary&lt;string, TValue&gt; creation.
    /// </summary>
    public static Stream WithDictionaries() => LoadEmbeddedResource("with-dictionaries.json");
    
    // === Phase 4: Real-World Examples ===
    
    /// <summary>
    /// Azure Functions HTTP trigger pattern.
    /// Tests Azure Function stereotype application.
    /// </summary>
    public static Stream AzureFunctionsPattern() => LoadEmbeddedResource("azure-functions.json");
    
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
