using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IAppointmentLocationDocument
    {
        string Building { get; }
        string Floor { get; }
        string RoomNumber { get; }
        Guid FacilityId { get; }
    }
}