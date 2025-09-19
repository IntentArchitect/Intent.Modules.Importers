using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IPhysicalExaminationDocument
    {
        string General { get; }
        IPhysicalExaminationVitalSignDocument VitalSigns { get; }
        ISystemicFindingDocument SystemicFindings { get; }
    }
}