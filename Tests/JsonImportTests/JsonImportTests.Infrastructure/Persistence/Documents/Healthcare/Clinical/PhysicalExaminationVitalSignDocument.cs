using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class PhysicalExaminationVitalSignDocument : IPhysicalExaminationVitalSignDocument
    {
        public decimal Temperature { get; set; }
        public string BloodPressure { get; set; } = default!;
        public decimal HeartRate { get; set; }
        public decimal RespiratoryRate { get; set; }
        public decimal OxygenSaturation { get; set; }

        public PhysicalExaminationVitalSign ToEntity(PhysicalExaminationVitalSign? entity = default)
        {
            entity ??= new PhysicalExaminationVitalSign();

            entity.Temperature = Temperature;
            entity.BloodPressure = BloodPressure ?? throw new Exception($"{nameof(entity.BloodPressure)} is null");
            entity.HeartRate = HeartRate;
            entity.RespiratoryRate = RespiratoryRate;
            entity.OxygenSaturation = OxygenSaturation;

            return entity;
        }

        public PhysicalExaminationVitalSignDocument PopulateFromEntity(PhysicalExaminationVitalSign entity)
        {
            Temperature = entity.Temperature;
            BloodPressure = entity.BloodPressure;
            HeartRate = entity.HeartRate;
            RespiratoryRate = entity.RespiratoryRate;
            OxygenSaturation = entity.OxygenSaturation;

            return this;
        }

        public static PhysicalExaminationVitalSignDocument? FromEntity(PhysicalExaminationVitalSign? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PhysicalExaminationVitalSignDocument().PopulateFromEntity(entity);
        }
    }
}