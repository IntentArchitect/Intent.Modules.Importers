using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class AttendanceRecordDocument : IAttendanceRecordDocument
    {
        public string Id { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Status { get; set; } = default!;
        public string Notes { get; set; } = default!;

        public AttendanceRecord ToEntity(AttendanceRecord? entity = default)
        {
            entity ??= new AttendanceRecord();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Date = Date;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.Notes = Notes ?? throw new Exception($"{nameof(entity.Notes)} is null");

            return entity;
        }

        public AttendanceRecordDocument PopulateFromEntity(AttendanceRecord entity)
        {
            Id = entity.Id;
            Date = entity.Date;
            Status = entity.Status;
            Notes = entity.Notes;

            return this;
        }

        public static AttendanceRecordDocument? FromEntity(AttendanceRecord? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AttendanceRecordDocument().PopulateFromEntity(entity);
        }
    }
}