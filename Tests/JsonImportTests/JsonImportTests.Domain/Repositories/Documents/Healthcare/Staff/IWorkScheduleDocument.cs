using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IWorkScheduleDocument
    {
        string ShiftType { get; }
        decimal WeeklyHours { get; }
        string StartTime { get; }
        string EndTime { get; }
        IReadOnlyList<string> WorkDays { get; }
    }
}