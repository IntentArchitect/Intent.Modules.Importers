using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class GradingInfoDocument : IGradingInfoDocument
    {
        public string CurrentGrade { get; set; } = default!;
        public decimal CurrentPercentage { get; set; }
        public decimal GradePoints { get; set; }
        public decimal QualityPoints { get; set; }
        public bool IsPassFail { get; set; }
        public object IncompleteReason { get; set; } = default!;
        public List<GradeChangeHistoryDocument> GradeChangeHistory { get; set; } = default!;
        IReadOnlyList<IGradeChangeHistoryDocument> IGradingInfoDocument.GradeChangeHistory => GradeChangeHistory;

        public GradingInfo ToEntity(GradingInfo? entity = default)
        {
            entity ??= new GradingInfo();

            entity.CurrentGrade = CurrentGrade ?? throw new Exception($"{nameof(entity.CurrentGrade)} is null");
            entity.CurrentPercentage = CurrentPercentage;
            entity.GradePoints = GradePoints;
            entity.QualityPoints = QualityPoints;
            entity.IsPassFail = IsPassFail;
            entity.IncompleteReason = IncompleteReason ?? throw new Exception($"{nameof(entity.IncompleteReason)} is null");
            entity.GradeChangeHistory = GradeChangeHistory.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public GradingInfoDocument PopulateFromEntity(GradingInfo entity)
        {
            CurrentGrade = entity.CurrentGrade;
            CurrentPercentage = entity.CurrentPercentage;
            GradePoints = entity.GradePoints;
            QualityPoints = entity.QualityPoints;
            IsPassFail = entity.IsPassFail;
            IncompleteReason = entity.IncompleteReason;
            GradeChangeHistory = entity.GradeChangeHistory.Select(x => GradeChangeHistoryDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static GradingInfoDocument? FromEntity(GradingInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GradingInfoDocument().PopulateFromEntity(entity);
        }
    }
}