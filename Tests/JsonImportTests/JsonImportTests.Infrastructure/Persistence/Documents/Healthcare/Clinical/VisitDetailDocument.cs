using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class VisitDetailDocument : IVisitDetailDocument
    {
        public VisitDetailsVitalSignDocument VitalSigns { get; set; } = default!;
        IVisitDetailsVitalSignDocument IVisitDetailDocument.VitalSigns => VitalSigns;
        public VisitDetailsTreatmentPlanDocument TreatmentPlan { get; set; } = default!;
        IVisitDetailsTreatmentPlanDocument IVisitDetailDocument.TreatmentPlan => TreatmentPlan;
        public List<SymptomDocument> Symptoms { get; set; } = default!;
        IReadOnlyList<ISymptomDocument> IVisitDetailDocument.Symptoms => Symptoms;
        public List<VisitDetailsDiagnosisDocument> Diagnosis { get; set; } = default!;
        IReadOnlyList<IVisitDetailsDiagnosisDocument> IVisitDetailDocument.Diagnosis => Diagnosis;

        public VisitDetail ToEntity(VisitDetail? entity = default)
        {
            entity ??= new VisitDetail();
            entity.VitalSigns = VitalSigns.ToEntity() ?? throw new Exception($"{nameof(entity.VitalSigns)} is null");
            entity.TreatmentPlan = TreatmentPlan.ToEntity() ?? throw new Exception($"{nameof(entity.TreatmentPlan)} is null");
            entity.Symptoms = Symptoms.Select(x => x.ToEntity()).ToList();
            entity.Diagnosis = Diagnosis.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public VisitDetailDocument PopulateFromEntity(VisitDetail entity)
        {
            VitalSigns = VisitDetailsVitalSignDocument.FromEntity(entity.VitalSigns)!;
            TreatmentPlan = VisitDetailsTreatmentPlanDocument.FromEntity(entity.TreatmentPlan)!;
            Symptoms = entity.Symptoms.Select(x => SymptomDocument.FromEntity(x)!).ToList();
            Diagnosis = entity.Diagnosis.Select(x => VisitDetailsDiagnosisDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static VisitDetailDocument? FromEntity(VisitDetail? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VisitDetailDocument().PopulateFromEntity(entity);
        }
    }
}