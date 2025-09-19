using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class MovementHistoryDocument : IMovementHistoryDocument
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public decimal Quantity { get; set; }
        public string Reason { get; set; } = default!;
        public Guid ReferenceId { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public decimal UnitCost { get; set; }

        public MovementHistory ToEntity(MovementHistory? entity = default)
        {
            entity ??= new MovementHistory();

            entity.Id = Id;
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Quantity = Quantity;
            entity.Reason = Reason ?? throw new Exception($"{nameof(entity.Reason)} is null");
            entity.ReferenceId = ReferenceId;
            entity.Date = Date;
            entity.User = User ?? throw new Exception($"{nameof(entity.User)} is null");
            entity.Notes = Notes ?? throw new Exception($"{nameof(entity.Notes)} is null");
            entity.UnitCost = UnitCost;

            return entity;
        }

        public MovementHistoryDocument PopulateFromEntity(MovementHistory entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Quantity = entity.Quantity;
            Reason = entity.Reason;
            ReferenceId = entity.ReferenceId;
            Date = entity.Date;
            User = entity.User;
            Notes = entity.Notes;
            UnitCost = entity.UnitCost;

            return this;
        }

        public static MovementHistoryDocument? FromEntity(MovementHistory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MovementHistoryDocument().PopulateFromEntity(entity);
        }
    }
}