using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class VisitDetailsTreatmentPlanDocument : IVisitDetailsTreatmentPlanDocument
    {
        public string Instructions { get; set; } = default!;
        public string FollowUpInstructions { get; set; } = default!;
        public bool NextAppointmentRecommended { get; set; }
        public List<TreatmentPlanMedicationDocument> Medications { get; set; } = default!;
        IReadOnlyList<ITreatmentPlanMedicationDocument> IVisitDetailsTreatmentPlanDocument.Medications => Medications;

        public VisitDetailsTreatmentPlan ToEntity(VisitDetailsTreatmentPlan? entity = default)
        {
            entity ??= new VisitDetailsTreatmentPlan();

            entity.Instructions = Instructions ?? throw new Exception($"{nameof(entity.Instructions)} is null");
            entity.FollowUpInstructions = FollowUpInstructions ?? throw new Exception($"{nameof(entity.FollowUpInstructions)} is null");
            entity.NextAppointmentRecommended = NextAppointmentRecommended;
            entity.Medications = Medications.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public VisitDetailsTreatmentPlanDocument PopulateFromEntity(VisitDetailsTreatmentPlan entity)
        {
            Instructions = entity.Instructions;
            FollowUpInstructions = entity.FollowUpInstructions;
            NextAppointmentRecommended = entity.NextAppointmentRecommended;
            Medications = entity.Medications.Select(x => TreatmentPlanMedicationDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static VisitDetailsTreatmentPlanDocument? FromEntity(VisitDetailsTreatmentPlan? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VisitDetailsTreatmentPlanDocument().PopulateFromEntity(entity);
        }
    }
}