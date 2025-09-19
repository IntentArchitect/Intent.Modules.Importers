using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class ConditionalDatumDocument : IConditionalDatumDocument
    {
        public string WhenPresent { get; set; } = default!;
        public object MaybeNull { get; set; } = default!;
        public List<string> OptionalArray { get; set; } = default!;
        IReadOnlyList<string> IConditionalDatumDocument.OptionalArray => OptionalArray;

        public ConditionalDatum ToEntity(ConditionalDatum? entity = default)
        {
            entity ??= new ConditionalDatum();

            entity.WhenPresent = WhenPresent ?? throw new Exception($"{nameof(entity.WhenPresent)} is null");
            entity.MaybeNull = MaybeNull ?? throw new Exception($"{nameof(entity.MaybeNull)} is null");
            entity.OptionalArray = OptionalArray ?? throw new Exception($"{nameof(entity.OptionalArray)} is null");

            return entity;
        }

        public ConditionalDatumDocument PopulateFromEntity(ConditionalDatum entity)
        {
            WhenPresent = entity.WhenPresent;
            MaybeNull = entity.MaybeNull;
            OptionalArray = entity.OptionalArray.ToList();

            return this;
        }

        public static ConditionalDatumDocument? FromEntity(ConditionalDatum? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ConditionalDatumDocument().PopulateFromEntity(entity);
        }
    }
}