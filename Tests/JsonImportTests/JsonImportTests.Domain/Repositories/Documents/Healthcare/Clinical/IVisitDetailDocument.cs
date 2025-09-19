using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IVisitDetailDocument
    {
        IVisitDetailsVitalSignDocument VitalSigns { get; }
        IVisitDetailsTreatmentPlanDocument TreatmentPlan { get; }
        IReadOnlyList<ISymptomDocument> Symptoms { get; }
        IReadOnlyList<IVisitDetailsDiagnosisDocument> Diagnosis { get; }
    }
}