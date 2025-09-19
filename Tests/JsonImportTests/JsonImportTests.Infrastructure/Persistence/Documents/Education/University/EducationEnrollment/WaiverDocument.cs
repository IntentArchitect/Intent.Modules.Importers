using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class WaiverDocument : IWaiverDocument
    {
        public Guid WaiverId { get; set; }
        public string PrerequisiteWaived { get; set; } = default!;
        public string WaiverReason { get; set; } = default!;
        public string ApprovedBy { get; set; } = default!;
        public DateTime ApprovalDate { get; set; }

        public Waiver ToEntity(Waiver? entity = default)
        {
            entity ??= new Waiver();

            entity.WaiverId = WaiverId;
            entity.PrerequisiteWaived = PrerequisiteWaived ?? throw new Exception($"{nameof(entity.PrerequisiteWaived)} is null");
            entity.WaiverReason = WaiverReason ?? throw new Exception($"{nameof(entity.WaiverReason)} is null");
            entity.ApprovedBy = ApprovedBy ?? throw new Exception($"{nameof(entity.ApprovedBy)} is null");
            entity.ApprovalDate = ApprovalDate;

            return entity;
        }

        public WaiverDocument PopulateFromEntity(Waiver entity)
        {
            WaiverId = entity.WaiverId;
            PrerequisiteWaived = entity.PrerequisiteWaived;
            WaiverReason = entity.WaiverReason;
            ApprovedBy = entity.ApprovedBy;
            ApprovalDate = entity.ApprovalDate;

            return this;
        }

        public static WaiverDocument? FromEntity(Waiver? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new WaiverDocument().PopulateFromEntity(entity);
        }
    }
}