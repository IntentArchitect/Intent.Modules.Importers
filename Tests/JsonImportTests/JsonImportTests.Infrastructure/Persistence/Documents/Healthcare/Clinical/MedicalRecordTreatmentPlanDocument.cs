using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class MedicalRecordTreatmentPlanDocument : IMedicalRecordTreatmentPlanDocument
    {
        public string Id { get; set; } = default!;
        public Guid PlanId { get; set; }
        public string Description { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = default!;
        public List<string> Goals { get; set; } = default!;
        IReadOnlyList<string> IMedicalRecordTreatmentPlanDocument.Goals => Goals;
        public List<InterventionDocument> Interventions { get; set; } = default!;
        IReadOnlyList<IInterventionDocument> IMedicalRecordTreatmentPlanDocument.Interventions => Interventions;

        public MedicalRecordTreatmentPlan ToEntity(MedicalRecordTreatmentPlan? entity = default)
        {
            entity ??= new MedicalRecordTreatmentPlan();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PlanId = PlanId;
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.StartDate = StartDate;
            entity.EndDate = EndDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.Goals = Goals ?? throw new Exception($"{nameof(entity.Goals)} is null");
            entity.Interventions = Interventions.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public MedicalRecordTreatmentPlanDocument PopulateFromEntity(MedicalRecordTreatmentPlan entity)
        {
            Id = entity.Id;
            PlanId = entity.PlanId;
            Description = entity.Description;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Status = entity.Status;
            Goals = entity.Goals.ToList();
            Interventions = entity.Interventions.Select(x => InterventionDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static MedicalRecordTreatmentPlanDocument? FromEntity(MedicalRecordTreatmentPlan? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalRecordTreatmentPlanDocument().PopulateFromEntity(entity);
        }
    }
}