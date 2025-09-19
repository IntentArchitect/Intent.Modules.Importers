using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class VisitInformationDocument : IVisitInformationDocument
    {
        public Guid AppointmentId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; } = default!;
        public Guid FacilityId { get; set; }

        public VisitInformation ToEntity(VisitInformation? entity = default)
        {
            entity ??= new VisitInformation();

            entity.AppointmentId = AppointmentId;
            entity.VisitDate = VisitDate;
            entity.VisitType = VisitType ?? throw new Exception($"{nameof(entity.VisitType)} is null");
            entity.FacilityId = FacilityId;

            return entity;
        }

        public VisitInformationDocument PopulateFromEntity(VisitInformation entity)
        {
            AppointmentId = entity.AppointmentId;
            VisitDate = entity.VisitDate;
            VisitType = entity.VisitType;
            FacilityId = entity.FacilityId;

            return this;
        }

        public static VisitInformationDocument? FromEntity(VisitInformation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VisitInformationDocument().PopulateFromEntity(entity);
        }
    }
}