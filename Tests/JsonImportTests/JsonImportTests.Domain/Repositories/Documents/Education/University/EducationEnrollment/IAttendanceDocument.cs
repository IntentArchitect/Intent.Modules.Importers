using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IAttendanceDocument
    {
        bool RequiredAttendance { get; }
        decimal TotalSessions { get; }
        decimal AttendedSessions { get; }
        decimal ExcusedAbsences { get; }
        decimal UnexcusedAbsences { get; }
        decimal AttendancePercentage { get; }
        IReadOnlyList<IAttendanceRecordDocument> AttendanceRecords { get; }
    }
}