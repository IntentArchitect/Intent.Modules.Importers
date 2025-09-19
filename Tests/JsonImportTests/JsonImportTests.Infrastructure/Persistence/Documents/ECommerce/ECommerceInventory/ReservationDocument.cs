using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class ReservationDocument : IReservationDocument
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; } = default!;
        public string ReservedBy { get; set; } = default!;

        public Reservation ToEntity(Reservation? entity = default)
        {
            entity ??= new Reservation();

            entity.Id = Id;
            entity.OrderId = OrderId;
            entity.Quantity = Quantity;
            entity.ReservedDate = ReservedDate;
            entity.ExpirationDate = ExpirationDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.ReservedBy = ReservedBy ?? throw new Exception($"{nameof(entity.ReservedBy)} is null");

            return entity;
        }

        public ReservationDocument PopulateFromEntity(Reservation entity)
        {
            Id = entity.Id;
            OrderId = entity.OrderId;
            Quantity = entity.Quantity;
            ReservedDate = entity.ReservedDate;
            ExpirationDate = entity.ExpirationDate;
            Status = entity.Status;
            ReservedBy = entity.ReservedBy;

            return this;
        }

        public static ReservationDocument? FromEntity(Reservation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ReservationDocument().PopulateFromEntity(entity);
        }
    }
}