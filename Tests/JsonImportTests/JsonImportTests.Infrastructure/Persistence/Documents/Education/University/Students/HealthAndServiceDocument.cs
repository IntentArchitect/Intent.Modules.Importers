using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class HealthAndServiceDocument : IHealthAndServiceDocument
    {
        public HealthAndServicesInsuranceInfoDocument InsuranceInfo { get; set; } = default!;
        IHealthAndServicesInsuranceInfoDocument IHealthAndServiceDocument.InsuranceInfo => InsuranceInfo;
        public List<DisabilityDocument> Disabilities { get; set; } = default!;
        IReadOnlyList<IDisabilityDocument> IHealthAndServiceDocument.Disabilities => Disabilities;
        public CounselingServiceDocument CounselingServices { get; set; } = default!;
        ICounselingServiceDocument IHealthAndServiceDocument.CounselingServices => CounselingServices;

        public HealthAndService ToEntity(HealthAndService? entity = default)
        {
            entity ??= new HealthAndService();
            entity.InsuranceInfo = InsuranceInfo.ToEntity() ?? throw new Exception($"{nameof(entity.InsuranceInfo)} is null");
            entity.Disabilities = Disabilities.Select(x => x.ToEntity()).ToList();
            entity.CounselingServices = CounselingServices.ToEntity() ?? throw new Exception($"{nameof(entity.CounselingServices)} is null");

            return entity;
        }

        public HealthAndServiceDocument PopulateFromEntity(HealthAndService entity)
        {
            InsuranceInfo = HealthAndServicesInsuranceInfoDocument.FromEntity(entity.InsuranceInfo)!;
            Disabilities = entity.Disabilities.Select(x => DisabilityDocument.FromEntity(x)!).ToList();
            CounselingServices = CounselingServiceDocument.FromEntity(entity.CounselingServices)!;

            return this;
        }

        public static HealthAndServiceDocument? FromEntity(HealthAndService? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new HealthAndServiceDocument().PopulateFromEntity(entity);
        }
    }
}