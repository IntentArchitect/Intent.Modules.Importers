using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class FlagDocument : IFlagDocument
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublic { get; set; }
        public object IsVerified { get; set; } = default!;
        public bool RequiresApproval { get; set; }
        public bool HasNotifications { get; set; }

        public Flag ToEntity(Flag? entity = default)
        {
            entity ??= new Flag();

            entity.IsActive = IsActive;
            entity.IsDeleted = IsDeleted;
            entity.IsPublic = IsPublic;
            entity.IsVerified = IsVerified ?? throw new Exception($"{nameof(entity.IsVerified)} is null");
            entity.RequiresApproval = RequiresApproval;
            entity.HasNotifications = HasNotifications;

            return entity;
        }

        public FlagDocument PopulateFromEntity(Flag entity)
        {
            IsActive = entity.IsActive;
            IsDeleted = entity.IsDeleted;
            IsPublic = entity.IsPublic;
            IsVerified = entity.IsVerified;
            RequiresApproval = entity.RequiresApproval;
            HasNotifications = entity.HasNotifications;

            return this;
        }

        public static FlagDocument? FromEntity(Flag? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FlagDocument().PopulateFromEntity(entity);
        }
    }
}