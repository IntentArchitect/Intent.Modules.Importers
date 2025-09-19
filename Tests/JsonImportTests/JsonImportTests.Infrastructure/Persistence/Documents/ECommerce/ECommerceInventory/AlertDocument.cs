using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class AlertDocument : IAlertDocument
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public bool IsAcknowledged { get; set; }

        public Alert ToEntity(Alert? entity = default)
        {
            entity ??= new Alert();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Message = Message ?? throw new Exception($"{nameof(entity.Message)} is null");
            entity.Severity = Severity ?? throw new Exception($"{nameof(entity.Severity)} is null");
            entity.CreatedDate = CreatedDate;
            entity.IsAcknowledged = IsAcknowledged;

            return entity;
        }

        public AlertDocument PopulateFromEntity(Alert entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Message = entity.Message;
            Severity = entity.Severity;
            CreatedDate = entity.CreatedDate;
            IsAcknowledged = entity.IsAcknowledged;

            return this;
        }

        public static AlertDocument? FromEntity(Alert? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AlertDocument().PopulateFromEntity(entity);
        }
    }
}