using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class SettingDocument : ISettingDocument
    {
        public string Theme { get; set; } = default!;
        public string Language { get; set; } = default!;
        public string Timezone { get; set; } = default!;
        public PrivacyDocument Privacy { get; set; } = default!;
        IPrivacyDocument ISettingDocument.Privacy => Privacy;
        public NotificationDocument Notifications { get; set; } = default!;
        INotificationDocument ISettingDocument.Notifications => Notifications;

        public Setting ToEntity(Setting? entity = default)
        {
            entity ??= new Setting();

            entity.Theme = Theme ?? throw new Exception($"{nameof(entity.Theme)} is null");
            entity.Language = Language ?? throw new Exception($"{nameof(entity.Language)} is null");
            entity.Timezone = Timezone ?? throw new Exception($"{nameof(entity.Timezone)} is null");
            entity.Privacy = Privacy.ToEntity() ?? throw new Exception($"{nameof(entity.Privacy)} is null");
            entity.Notifications = Notifications.ToEntity() ?? throw new Exception($"{nameof(entity.Notifications)} is null");

            return entity;
        }

        public SettingDocument PopulateFromEntity(Setting entity)
        {
            Theme = entity.Theme;
            Language = entity.Language;
            Timezone = entity.Timezone;
            Privacy = PrivacyDocument.FromEntity(entity.Privacy)!;
            Notifications = NotificationDocument.FromEntity(entity.Notifications)!;

            return this;
        }

        public static SettingDocument? FromEntity(Setting? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SettingDocument().PopulateFromEntity(entity);
        }
    }
}