using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class BasicInfoDocument : IBasicInfoDocument
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public BasicInfo ToEntity(BasicInfo? entity = default)
        {
            entity ??= new BasicInfo();

            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");

            return entity;
        }

        public BasicInfoDocument PopulateFromEntity(BasicInfo entity)
        {
            Name = entity.Name;
            Email = entity.Email;

            return this;
        }

        public static BasicInfoDocument? FromEntity(BasicInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BasicInfoDocument().PopulateFromEntity(entity);
        }
    }
}