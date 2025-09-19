using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class NotificationDocument : INotificationDocument
    {
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool Push { get; set; }
        public bool InApp { get; set; }

        public Notification ToEntity(Notification? entity = default)
        {
            entity ??= new Notification();

            entity.Email = Email;
            entity.SMS = SMS;
            entity.Push = Push;
            entity.InApp = InApp;

            return entity;
        }

        public NotificationDocument PopulateFromEntity(Notification entity)
        {
            Email = entity.Email;
            SMS = entity.SMS;
            Push = entity.Push;
            InApp = entity.InApp;

            return this;
        }

        public static NotificationDocument? FromEntity(Notification? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new NotificationDocument().PopulateFromEntity(entity);
        }
    }
}