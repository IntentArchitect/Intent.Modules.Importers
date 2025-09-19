using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IClinicalDatumDocument
    {
        string ChiefComplaint { get; }
        string HistoryOfPresentIllness { get; }
        IReviewOfSystemDocument ReviewOfSystems { get; }
        IPhysicalExaminationDocument PhysicalExamination { get; }
    }
}