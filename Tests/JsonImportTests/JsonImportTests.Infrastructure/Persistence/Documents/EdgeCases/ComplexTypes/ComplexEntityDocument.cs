using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class ComplexEntityDocument : IComplexEntityDocument, ICosmosDBDocument<ComplexEntity, ComplexEntityDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string Name { get; set; } = default!;
        public object OptionalDescription { get; set; } = default!;
        public List<string> Tags { get; set; } = default!;
        IReadOnlyList<string> IComplexEntityDocument.Tags => Tags;
        public List<decimal> Numbers { get; set; } = default!;
        IReadOnlyList<decimal> IComplexEntityDocument.Numbers => Numbers;
        public List<decimal> Decimals { get; set; } = default!;
        IReadOnlyList<decimal> IComplexEntityDocument.Decimals => Decimals;
        public List<bool> Booleans { get; set; } = default!;
        IReadOnlyList<bool> IComplexEntityDocument.Booleans => Booleans;
        public List<object> EmptyArray { get; set; } = default!;
        IReadOnlyList<object> IComplexEntityDocument.EmptyArray => EmptyArray;
        public object NullableField { get; set; } = default!;
        public object OptionalObject { get; set; } = default!;
        public List<object> NestedArrays { get; set; } = default!;
        IReadOnlyList<object> IComplexEntityDocument.NestedArrays => NestedArrays;
        public List<VersionHistoryDocument> VersionHistory { get; set; } = default!;
        IReadOnlyList<IVersionHistoryDocument> IComplexEntityDocument.VersionHistory => VersionHistory;
        public TimestampDocument Timestamps { get; set; } = default!;
        ITimestampDocument IComplexEntityDocument.Timestamps => Timestamps;
        public List<PolymorphicDocument> Polymorphic { get; set; } = default!;
        IReadOnlyList<IPolymorphicDocument> IComplexEntityDocument.Polymorphic => Polymorphic;
        public OptionalNestedObjectDocument OptionalNestedObject { get; set; } = default!;
        IOptionalNestedObjectDocument IComplexEntityDocument.OptionalNestedObject => OptionalNestedObject;
        public List<MixedTypeArrayDocument> MixedTypeArray { get; set; } = default!;
        IReadOnlyList<IMixedTypeArrayDocument> IComplexEntityDocument.MixedTypeArray => MixedTypeArray;
        public MetadataMapDocument MetadataMap { get; set; } = default!;
        IMetadataMapDocument IComplexEntityDocument.MetadataMap => MetadataMap;
        public FlagDocument Flags { get; set; } = default!;
        IFlagDocument IComplexEntityDocument.Flags => Flags;
        public ConfigurationDocument Configuration { get; set; } = default!;
        IConfigurationDocument IComplexEntityDocument.Configuration => Configuration;
        public ConditionalFieldDocument ConditionalFields { get; set; } = default!;
        IConditionalFieldDocument IComplexEntityDocument.ConditionalFields => ConditionalFields;
        public ComplexInheritanceDocument ComplexInheritance { get; set; } = default!;
        IComplexInheritanceDocument IComplexEntityDocument.ComplexInheritance => ComplexInheritance;

        public ComplexEntity ToEntity(ComplexEntity? entity = default)
        {
            entity ??= new ComplexEntity();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.OptionalDescription = OptionalDescription ?? throw new Exception($"{nameof(entity.OptionalDescription)} is null");
            entity.Tags = Tags ?? throw new Exception($"{nameof(entity.Tags)} is null");
            entity.Numbers = Numbers ?? throw new Exception($"{nameof(entity.Numbers)} is null");
            entity.Decimals = Decimals ?? throw new Exception($"{nameof(entity.Decimals)} is null");
            entity.Booleans = Booleans ?? throw new Exception($"{nameof(entity.Booleans)} is null");
            entity.EmptyArray = EmptyArray ?? throw new Exception($"{nameof(entity.EmptyArray)} is null");
            entity.NullableField = NullableField ?? throw new Exception($"{nameof(entity.NullableField)} is null");
            entity.OptionalObject = OptionalObject ?? throw new Exception($"{nameof(entity.OptionalObject)} is null");
            entity.NestedArrays = NestedArrays ?? throw new Exception($"{nameof(entity.NestedArrays)} is null");
            entity.VersionHistory = VersionHistory.Select(x => x.ToEntity()).ToList();
            entity.Timestamps = Timestamps.ToEntity() ?? throw new Exception($"{nameof(entity.Timestamps)} is null");
            entity.Polymorphic = Polymorphic.Select(x => x.ToEntity()).ToList();
            entity.OptionalNestedObject = OptionalNestedObject.ToEntity() ?? throw new Exception($"{nameof(entity.OptionalNestedObject)} is null");
            entity.MixedTypeArray = MixedTypeArray.Select(x => x.ToEntity()).ToList();
            entity.MetadataMap = MetadataMap.ToEntity() ?? throw new Exception($"{nameof(entity.MetadataMap)} is null");
            entity.Flags = Flags.ToEntity() ?? throw new Exception($"{nameof(entity.Flags)} is null");
            entity.Configuration = Configuration.ToEntity() ?? throw new Exception($"{nameof(entity.Configuration)} is null");
            entity.ConditionalFields = ConditionalFields.ToEntity() ?? throw new Exception($"{nameof(entity.ConditionalFields)} is null");
            entity.ComplexInheritance = ComplexInheritance.ToEntity() ?? throw new Exception($"{nameof(entity.ComplexInheritance)} is null");

            return entity;
        }

        public ComplexEntityDocument PopulateFromEntity(ComplexEntity entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;
            OptionalDescription = entity.OptionalDescription;
            Tags = entity.Tags.ToList();
            Numbers = entity.Numbers.ToList();
            Decimals = entity.Decimals.ToList();
            Booleans = entity.Booleans.ToList();
            EmptyArray = entity.EmptyArray.ToList();
            NullableField = entity.NullableField;
            OptionalObject = entity.OptionalObject;
            NestedArrays = entity.NestedArrays.ToList();
            VersionHistory = entity.VersionHistory.Select(x => VersionHistoryDocument.FromEntity(x)!).ToList();
            Timestamps = TimestampDocument.FromEntity(entity.Timestamps)!;
            Polymorphic = entity.Polymorphic.Select(x => PolymorphicDocument.FromEntity(x)!).ToList();
            OptionalNestedObject = OptionalNestedObjectDocument.FromEntity(entity.OptionalNestedObject)!;
            MixedTypeArray = entity.MixedTypeArray.Select(x => MixedTypeArrayDocument.FromEntity(x)!).ToList();
            MetadataMap = MetadataMapDocument.FromEntity(entity.MetadataMap)!;
            Flags = FlagDocument.FromEntity(entity.Flags)!;
            Configuration = ConfigurationDocument.FromEntity(entity.Configuration)!;
            ConditionalFields = ConditionalFieldDocument.FromEntity(entity.ConditionalFields)!;
            ComplexInheritance = ComplexInheritanceDocument.FromEntity(entity.ComplexInheritance)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ComplexEntityDocument? FromEntity(ComplexEntity? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ComplexEntityDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}