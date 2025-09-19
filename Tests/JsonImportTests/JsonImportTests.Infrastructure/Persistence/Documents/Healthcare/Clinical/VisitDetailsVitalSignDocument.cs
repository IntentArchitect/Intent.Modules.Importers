using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class VisitDetailsVitalSignDocument : IVisitDetailsVitalSignDocument
    {
        public decimal Temperature { get; set; }
        public decimal BloodPressureSystolic { get; set; }
        public decimal BloodPressureDiastolic { get; set; }
        public decimal HeartRate { get; set; }
        public decimal RespiratoryRate { get; set; }
        public decimal OxygenSaturation { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }

        public VisitDetailsVitalSign ToEntity(VisitDetailsVitalSign? entity = default)
        {
            entity ??= new VisitDetailsVitalSign();

            entity.Temperature = Temperature;
            entity.BloodPressureSystolic = BloodPressureSystolic;
            entity.BloodPressureDiastolic = BloodPressureDiastolic;
            entity.HeartRate = HeartRate;
            entity.RespiratoryRate = RespiratoryRate;
            entity.OxygenSaturation = OxygenSaturation;
            entity.Weight = Weight;
            entity.Height = Height;

            return entity;
        }

        public VisitDetailsVitalSignDocument PopulateFromEntity(VisitDetailsVitalSign entity)
        {
            Temperature = entity.Temperature;
            BloodPressureSystolic = entity.BloodPressureSystolic;
            BloodPressureDiastolic = entity.BloodPressureDiastolic;
            HeartRate = entity.HeartRate;
            RespiratoryRate = entity.RespiratoryRate;
            OxygenSaturation = entity.OxygenSaturation;
            Weight = entity.Weight;
            Height = entity.Height;

            return this;
        }

        public static VisitDetailsVitalSignDocument? FromEntity(VisitDetailsVitalSign? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VisitDetailsVitalSignDocument().PopulateFromEntity(entity);
        }
    }
}