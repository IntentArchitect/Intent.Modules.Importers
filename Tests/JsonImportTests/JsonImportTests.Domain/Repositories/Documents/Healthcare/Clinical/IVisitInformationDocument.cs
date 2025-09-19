using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IVisitInformationDocument
    {
        Guid AppointmentId { get; }
        DateTime VisitDate { get; }
        string VisitType { get; }
        Guid FacilityId { get; }
    }
}