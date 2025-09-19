using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class PhysicalExaminationDocument : IPhysicalExaminationDocument
    {
        public string General { get; set; } = default!;
        public PhysicalExaminationVitalSignDocument VitalSigns { get; set; } = default!;
        IPhysicalExaminationVitalSignDocument IPhysicalExaminationDocument.VitalSigns => VitalSigns;
        public SystemicFindingDocument SystemicFindings { get; set; } = default!;
        ISystemicFindingDocument IPhysicalExaminationDocument.SystemicFindings => SystemicFindings;

        public PhysicalExamination ToEntity(PhysicalExamination? entity = default)
        {
            entity ??= new PhysicalExamination();

            entity.General = General ?? throw new Exception($"{nameof(entity.General)} is null");
            entity.VitalSigns = VitalSigns.ToEntity() ?? throw new Exception($"{nameof(entity.VitalSigns)} is null");
            entity.SystemicFindings = SystemicFindings.ToEntity() ?? throw new Exception($"{nameof(entity.SystemicFindings)} is null");

            return entity;
        }

        public PhysicalExaminationDocument PopulateFromEntity(PhysicalExamination entity)
        {
            General = entity.General;
            VitalSigns = PhysicalExaminationVitalSignDocument.FromEntity(entity.VitalSigns)!;
            SystemicFindings = SystemicFindingDocument.FromEntity(entity.SystemicFindings)!;

            return this;
        }

        public static PhysicalExaminationDocument? FromEntity(PhysicalExamination? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PhysicalExaminationDocument().PopulateFromEntity(entity);
        }
    }
}