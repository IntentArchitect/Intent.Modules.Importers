using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class ComplexEntity
    {
        private Guid? _id;

        public ComplexEntity()
        {
            Name = null!;
            OptionalDescription = null!;
            NullableField = null!;
            OptionalObject = null!;
            Timestamps = null!;
            OptionalNestedObject = null!;
            MetadataMap = null!;
            Flags = null!;
            Configuration = null!;
            ConditionalFields = null!;
            ComplexInheritance = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Name { get; set; }

        public object OptionalDescription { get; set; }

        public IList<string> Tags { get; set; } = [];

        public IList<decimal> Numbers { get; set; } = [];

        public IList<decimal> Decimals { get; set; } = [];

        public IList<bool> Booleans { get; set; } = [];

        public ICollection<object> EmptyArray { get; set; } = [];

        public object NullableField { get; set; }

        public object OptionalObject { get; set; }

        public ICollection<object> NestedArrays { get; set; } = [];

        public ICollection<VersionHistory> VersionHistory { get; set; } = [];

        public Timestamp Timestamps { get; set; }

        public ICollection<Polymorphic> Polymorphic { get; set; } = [];

        public OptionalNestedObject OptionalNestedObject { get; set; }

        public ICollection<MixedTypeArray> MixedTypeArray { get; set; } = [];

        public MetadataMap MetadataMap { get; set; }

        public Flag Flags { get; set; }

        public Configuration Configuration { get; set; }

        public ConditionalField ConditionalFields { get; set; }

        public ComplexInheritance ComplexInheritance { get; set; }
    }
}