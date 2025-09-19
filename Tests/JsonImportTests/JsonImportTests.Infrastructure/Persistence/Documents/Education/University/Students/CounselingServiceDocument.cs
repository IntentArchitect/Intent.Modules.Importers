using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class CounselingServiceDocument : ICounselingServiceDocument
    {
        public bool IsReceivingServices { get; set; }
        public object CounselorId { get; set; } = default!;
        public object LastAppointment { get; set; } = default!;

        public CounselingService ToEntity(CounselingService? entity = default)
        {
            entity ??= new CounselingService();

            entity.IsReceivingServices = IsReceivingServices;
            entity.CounselorId = CounselorId ?? throw new Exception($"{nameof(entity.CounselorId)} is null");
            entity.LastAppointment = LastAppointment ?? throw new Exception($"{nameof(entity.LastAppointment)} is null");

            return entity;
        }

        public CounselingServiceDocument PopulateFromEntity(CounselingService entity)
        {
            IsReceivingServices = entity.IsReceivingServices;
            CounselorId = entity.CounselorId;
            LastAppointment = entity.LastAppointment;

            return this;
        }

        public static CounselingServiceDocument? FromEntity(CounselingService? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CounselingServiceDocument().PopulateFromEntity(entity);
        }
    }
}