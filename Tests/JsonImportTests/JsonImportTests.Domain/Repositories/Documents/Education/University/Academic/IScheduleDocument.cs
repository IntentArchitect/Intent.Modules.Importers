using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IScheduleDocument
    {
        IReadOnlyList<IMeetingTimeDocument> MeetingTimes { get; }
        IExamScheduleDocument ExamSchedule { get; }
    }
}