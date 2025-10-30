using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for OpenAPI specifications.
/// Loads test data from the local Data/ folder to provide reusable OpenApiDocument instances.
/// </summary>
public static class OpenApiSpecs
{
    private static readonly string DataPath = Path.Combine(
        Path.GetDirectoryName(typeof(OpenApiSpecs).Assembly.Location)!,
        "..",
        "..",
        "..",
        "Data");

    public static OpenApiDocument PetStore()
    {
        return LoadSpec("pet-store.yaml");
    }

    public static OpenApiDocument PetStoreV1()
    {
        return LoadSpec("PetStorevOpenApi1.0.yaml");
    }

    public static OpenApiDocument WithEnum()
    {
        return LoadSpec("with-enum.json");
    }

    public static OpenApiDocument WithQuery()
    {
        return LoadSpec("with-query.json");
    }

    public static OpenApiDocument WithQuery2()
    {
        return LoadSpec("with-query2.json");
    }

    public static OpenApiDocument SwaggerSample()
    {
        return LoadSpec("swagger-sample.json");
    }

    public static OpenApiDocument AllOfNamedEnumsSecurity()
    {
        return LoadSpec("all-of-named-enums-security.json");
    }

    public static OpenApiDocument AzureOpenApi()
    {
        return LoadSpec("azure-open-api.json");
    }

    public static OpenApiDocument CleanArchTests()
    {
        return LoadSpec("clean-arch-tests.json");
    }

    public static OpenApiDocument IntentGeneratedSecured()
    {
        return LoadSpec("intent-generated-secured.json");
    }

    /// <summary>
    /// Gets a stream for the specified OpenAPI specification file.
    /// Caller is responsible for disposing the stream.
    /// </summary>
    public static Stream GetStream(string fileName)
    {
        var filePath = Path.Combine(DataPath, fileName);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"OpenAPI spec file not found: {filePath}");
        }

        return File.OpenRead(filePath);
    }

    private static OpenApiDocument LoadSpec(string fileName)
    {
        using var stream = GetStream(fileName);
        var reader = new OpenApiStreamReader();
        var document = reader.Read(stream, out var diagnostic);

        if (diagnostic.Errors.Count > 0)
        {
            var errors = string.Join(Environment.NewLine, diagnostic.Errors.Select(e => e.Message));
            throw new InvalidOperationException($"Failed to parse OpenAPI document '{fileName}':{Environment.NewLine}{errors}");
        }

        return document;
    }
}
