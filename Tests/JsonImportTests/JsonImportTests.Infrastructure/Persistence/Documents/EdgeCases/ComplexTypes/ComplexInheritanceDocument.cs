using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class ComplexInheritanceDocument : IComplexInheritanceDocument
    {
        public string BaseType { get; set; } = default!;
        public SpecificPropertyDocument SpecificProperties { get; set; } = default!;
        ISpecificPropertyDocument IComplexInheritanceDocument.SpecificProperties => SpecificProperties;
        public PropertyDocument Properties { get; set; } = default!;
        IPropertyDocument IComplexInheritanceDocument.Properties => Properties;

        public ComplexInheritance ToEntity(ComplexInheritance? entity = default)
        {
            entity ??= new ComplexInheritance();

            entity.BaseType = BaseType ?? throw new Exception($"{nameof(entity.BaseType)} is null");
            entity.SpecificProperties = SpecificProperties.ToEntity() ?? throw new Exception($"{nameof(entity.SpecificProperties)} is null");
            entity.Properties = Properties.ToEntity() ?? throw new Exception($"{nameof(entity.Properties)} is null");

            return entity;
        }

        public ComplexInheritanceDocument PopulateFromEntity(ComplexInheritance entity)
        {
            BaseType = entity.BaseType;
            SpecificProperties = SpecificPropertyDocument.FromEntity(entity.SpecificProperties)!;
            Properties = PropertyDocument.FromEntity(entity.Properties)!;

            return this;
        }

        public static ComplexInheritanceDocument? FromEntity(ComplexInheritance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ComplexInheritanceDocument().PopulateFromEntity(entity);
        }
    }
}