using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IVisitDetailsVitalSignDocument
    {
        decimal Temperature { get; }
        decimal BloodPressureSystolic { get; }
        decimal BloodPressureDiastolic { get; }
        decimal HeartRate { get; }
        decimal RespiratoryRate { get; }
        decimal OxygenSaturation { get; }
        decimal Weight { get; }
        decimal Height { get; }
    }
}