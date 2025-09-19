using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface ISpecificPropertyDocument
    {
        string VehicleType { get; }
        decimal Doors { get; }
        string FuelType { get; }
        string Transmission { get; }
        IReadOnlyList<string> Features { get; }
        ISafetyDocument Safety { get; }
        IEngineDocument Engine { get; }
    }
}