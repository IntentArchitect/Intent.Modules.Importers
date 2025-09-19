using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class GradeChangeHistoryDocument : IGradeChangeHistoryDocument
    {
        public string Id { get; set; } = default!;
        public Guid ChangeId { get; set; }
        public string OldGrade { get; set; } = default!;
        public string NewGrade { get; set; } = default!;
        public string ChangeReason { get; set; } = default!;
        public string ChangedBy { get; set; } = default!;
        public DateTime ChangeDate { get; set; }
        public string ApprovedBy { get; set; } = default!;

        public GradeChangeHistory ToEntity(GradeChangeHistory? entity = default)
        {
            entity ??= new GradeChangeHistory();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ChangeId = ChangeId;
            entity.OldGrade = OldGrade ?? throw new Exception($"{nameof(entity.OldGrade)} is null");
            entity.NewGrade = NewGrade ?? throw new Exception($"{nameof(entity.NewGrade)} is null");
            entity.ChangeReason = ChangeReason ?? throw new Exception($"{nameof(entity.ChangeReason)} is null");
            entity.ChangedBy = ChangedBy ?? throw new Exception($"{nameof(entity.ChangedBy)} is null");
            entity.ChangeDate = ChangeDate;
            entity.ApprovedBy = ApprovedBy ?? throw new Exception($"{nameof(entity.ApprovedBy)} is null");

            return entity;
        }

        public GradeChangeHistoryDocument PopulateFromEntity(GradeChangeHistory entity)
        {
            Id = entity.Id;
            ChangeId = entity.ChangeId;
            OldGrade = entity.OldGrade;
            NewGrade = entity.NewGrade;
            ChangeReason = entity.ChangeReason;
            ChangedBy = entity.ChangedBy;
            ChangeDate = entity.ChangeDate;
            ApprovedBy = entity.ApprovedBy;

            return this;
        }

        public static GradeChangeHistoryDocument? FromEntity(GradeChangeHistory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GradeChangeHistoryDocument().PopulateFromEntity(entity);
        }
    }
}