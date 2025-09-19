using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IAppointmentDocument
    {
        string Id { get; }
        string AppointmentNumber { get; }
        Guid PatientId { get; }
        Guid PractitionerId { get; }
        string AppointmentType { get; }
        string Status { get; }
        string Priority { get; }
        DateTime ScheduledDateTime { get; }
        decimal EstimatedDuration { get; }
        DateTime ActualStartTime { get; }
        DateTime ActualEndTime { get; }
        string ReasonForVisit { get; }
        string ChiefComplaint { get; }
        string Notes { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        IVisitDetailDocument VisitDetails { get; }
        IAppointmentLocationDocument Location { get; }
    }
}