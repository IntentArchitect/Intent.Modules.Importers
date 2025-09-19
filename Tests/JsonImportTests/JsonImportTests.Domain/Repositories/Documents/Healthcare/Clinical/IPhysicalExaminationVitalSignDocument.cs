using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IPhysicalExaminationVitalSignDocument
    {
        decimal Temperature { get; }
        string BloodPressure { get; }
        decimal HeartRate { get; }
        decimal RespiratoryRate { get; }
        decimal OxygenSaturation { get; }
    }
}