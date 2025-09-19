using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IComplexEntityDocument
    {
        string Id { get; }
        string Name { get; }
        object OptionalDescription { get; }
        IReadOnlyList<string> Tags { get; }
        IReadOnlyList<decimal> Numbers { get; }
        IReadOnlyList<decimal> Decimals { get; }
        IReadOnlyList<bool> Booleans { get; }
        IReadOnlyList<object> EmptyArray { get; }
        object NullableField { get; }
        object OptionalObject { get; }
        IReadOnlyList<object> NestedArrays { get; }
        IReadOnlyList<IVersionHistoryDocument> VersionHistory { get; }
        ITimestampDocument Timestamps { get; }
        IReadOnlyList<IPolymorphicDocument> Polymorphic { get; }
        IOptionalNestedObjectDocument OptionalNestedObject { get; }
        IReadOnlyList<IMixedTypeArrayDocument> MixedTypeArray { get; }
        IMetadataMapDocument MetadataMap { get; }
        IFlagDocument Flags { get; }
        IConfigurationDocument Configuration { get; }
        IConditionalFieldDocument ConditionalFields { get; }
        IComplexInheritanceDocument ComplexInheritance { get; }
    }
}