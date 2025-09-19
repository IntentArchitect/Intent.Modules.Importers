using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IComplexInheritanceDocument
    {
        string BaseType { get; }
        ISpecificPropertyDocument SpecificProperties { get; }
        IPropertyDocument Properties { get; }
    }
}