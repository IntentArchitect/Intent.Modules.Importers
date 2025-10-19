using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

/// <summary>
/// Fluent scenario builder for composing OpenAPI import test scenarios.
/// </summary>
internal class ScenarioComposer
{
    private Stream? _openApiDocument;
    private ImportConfig? _config;
    private List<PackageModelPersistable> _packages = new();

    public ScenarioComposer WithDocument(Stream document)
    {
        _openApiDocument = document;
        return this;
    }

    public ScenarioComposer WithConfig(ImportConfig config)
    {
        _config = config;
        return this;
    }

    public ScenarioComposer WithPackage(PackageModelPersistable package)
    {
        _packages.Add(package);
        return this;
    }

    public Persistables Execute()
    {
        if (_openApiDocument == null) throw new InvalidOperationException("Document is required");
        if (_config == null) throw new InvalidOperationException("Config is required");
        if (_packages.Count == 0) _packages.Add(PackageModels.WithBasicTypes());

        var factory = new OpenApiPersistableFactory();
        return factory.GetPersistables(_openApiDocument, _config, _packages);
    }
}
