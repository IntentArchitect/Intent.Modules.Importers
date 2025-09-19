using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class ConditionalFieldDocument : IConditionalFieldDocument
    {
        public string UserType { get; set; } = default!;
        public object AdminInfo { get; set; } = default!;
        public object GuestInfo { get; set; } = default!;
        public PremiumInfoDocument PremiumInfo { get; set; } = default!;
        IPremiumInfoDocument IConditionalFieldDocument.PremiumInfo => PremiumInfo;
        public BasicInfoDocument BasicInfo { get; set; } = default!;
        IBasicInfoDocument IConditionalFieldDocument.BasicInfo => BasicInfo;

        public ConditionalField ToEntity(ConditionalField? entity = default)
        {
            entity ??= new ConditionalField();

            entity.UserType = UserType ?? throw new Exception($"{nameof(entity.UserType)} is null");
            entity.AdminInfo = AdminInfo ?? throw new Exception($"{nameof(entity.AdminInfo)} is null");
            entity.GuestInfo = GuestInfo ?? throw new Exception($"{nameof(entity.GuestInfo)} is null");
            entity.PremiumInfo = PremiumInfo.ToEntity() ?? throw new Exception($"{nameof(entity.PremiumInfo)} is null");
            entity.BasicInfo = BasicInfo.ToEntity() ?? throw new Exception($"{nameof(entity.BasicInfo)} is null");

            return entity;
        }

        public ConditionalFieldDocument PopulateFromEntity(ConditionalField entity)
        {
            UserType = entity.UserType;
            AdminInfo = entity.AdminInfo;
            GuestInfo = entity.GuestInfo;
            PremiumInfo = PremiumInfoDocument.FromEntity(entity.PremiumInfo)!;
            BasicInfo = BasicInfoDocument.FromEntity(entity.BasicInfo)!;

            return this;
        }

        public static ConditionalFieldDocument? FromEntity(ConditionalField? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ConditionalFieldDocument().PopulateFromEntity(entity);
        }
    }
}