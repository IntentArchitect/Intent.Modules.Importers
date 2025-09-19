using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class AttendanceDocument : IAttendanceDocument
    {
        public bool RequiredAttendance { get; set; }
        public decimal TotalSessions { get; set; }
        public decimal AttendedSessions { get; set; }
        public decimal ExcusedAbsences { get; set; }
        public decimal UnexcusedAbsences { get; set; }
        public decimal AttendancePercentage { get; set; }
        public List<AttendanceRecordDocument> AttendanceRecords { get; set; } = default!;
        IReadOnlyList<IAttendanceRecordDocument> IAttendanceDocument.AttendanceRecords => AttendanceRecords;

        public Attendance ToEntity(Attendance? entity = default)
        {
            entity ??= new Attendance();

            entity.RequiredAttendance = RequiredAttendance;
            entity.TotalSessions = TotalSessions;
            entity.AttendedSessions = AttendedSessions;
            entity.ExcusedAbsences = ExcusedAbsences;
            entity.UnexcusedAbsences = UnexcusedAbsences;
            entity.AttendancePercentage = AttendancePercentage;
            entity.AttendanceRecords = AttendanceRecords.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public AttendanceDocument PopulateFromEntity(Attendance entity)
        {
            RequiredAttendance = entity.RequiredAttendance;
            TotalSessions = entity.TotalSessions;
            AttendedSessions = entity.AttendedSessions;
            ExcusedAbsences = entity.ExcusedAbsences;
            UnexcusedAbsences = entity.UnexcusedAbsences;
            AttendancePercentage = entity.AttendancePercentage;
            AttendanceRecords = entity.AttendanceRecords.Select(x => AttendanceRecordDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static AttendanceDocument? FromEntity(Attendance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AttendanceDocument().PopulateFromEntity(entity);
        }
    }
}