using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PrivacyDocument : IPrivacyDocument
    {
        public string ProfileVisibility { get; set; } = default!;
        public bool ShowEmail { get; set; }
        public object ShowPhone { get; set; } = default!;
        public bool AllowSearch { get; set; }

        public Privacy ToEntity(Privacy? entity = default)
        {
            entity ??= new Privacy();

            entity.ProfileVisibility = ProfileVisibility ?? throw new Exception($"{nameof(entity.ProfileVisibility)} is null");
            entity.ShowEmail = ShowEmail;
            entity.ShowPhone = ShowPhone ?? throw new Exception($"{nameof(entity.ShowPhone)} is null");
            entity.AllowSearch = AllowSearch;

            return entity;
        }

        public PrivacyDocument PopulateFromEntity(Privacy entity)
        {
            ProfileVisibility = entity.ProfileVisibility;
            ShowEmail = entity.ShowEmail;
            ShowPhone = entity.ShowPhone;
            AllowSearch = entity.AllowSearch;

            return this;
        }

        public static PrivacyDocument? FromEntity(Privacy? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PrivacyDocument().PopulateFromEntity(entity);
        }
    }
}