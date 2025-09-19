using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class InventoryItemDocument : IInventoryItemDocument, ICosmosDBDocument<InventoryItem, InventoryItemDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
        public string SKU { get; set; } = default!;
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; } = default!;
        public decimal Version { get; set; }
        public StockLevelDocument StockLevels { get; set; } = default!;
        IStockLevelDocument IInventoryItemDocument.StockLevels => StockLevels;
        public List<ReservationDocument> Reservations { get; set; } = default!;
        IReadOnlyList<IReservationDocument> IInventoryItemDocument.Reservations => Reservations;
        public QualityControlDocument QualityControl { get; set; } = default!;
        IQualityControlDocument IInventoryItemDocument.QualityControl => QualityControl;
        public List<MovementHistoryDocument> MovementHistory { get; set; } = default!;
        IReadOnlyList<IMovementHistoryDocument> IInventoryItemDocument.MovementHistory => MovementHistory;
        public InventoryItemLocationDocument Location { get; set; } = default!;
        IInventoryItemLocationDocument IInventoryItemDocument.Location => Location;
        public CostTrackingDocument CostTracking { get; set; } = default!;
        ICostTrackingDocument IInventoryItemDocument.CostTracking => CostTracking;
        public List<AlertDocument> Alerts { get; set; } = default!;
        IReadOnlyList<IAlertDocument> IInventoryItemDocument.Alerts => Alerts;

        public InventoryItem ToEntity(InventoryItem? entity = default)
        {
            entity ??= new InventoryItem();

            entity.Id = Guid.Parse(Id);
            entity.ProductId = ProductId;
            entity.VariantId = VariantId;
            entity.SKU = SKU ?? throw new Exception($"{nameof(entity.SKU)} is null");
            entity.LastUpdated = LastUpdated;
            entity.UpdatedBy = UpdatedBy ?? throw new Exception($"{nameof(entity.UpdatedBy)} is null");
            entity.Version = Version;
            entity.StockLevels = StockLevels.ToEntity() ?? throw new Exception($"{nameof(entity.StockLevels)} is null");
            entity.Reservations = Reservations.Select(x => x.ToEntity()).ToList();
            entity.QualityControl = QualityControl.ToEntity() ?? throw new Exception($"{nameof(entity.QualityControl)} is null");
            entity.MovementHistory = MovementHistory.Select(x => x.ToEntity()).ToList();
            entity.Location = Location.ToEntity() ?? throw new Exception($"{nameof(entity.Location)} is null");
            entity.CostTracking = CostTracking.ToEntity() ?? throw new Exception($"{nameof(entity.CostTracking)} is null");
            entity.Alerts = Alerts.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public InventoryItemDocument PopulateFromEntity(InventoryItem entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            ProductId = entity.ProductId;
            VariantId = entity.VariantId;
            SKU = entity.SKU;
            LastUpdated = entity.LastUpdated;
            UpdatedBy = entity.UpdatedBy;
            Version = entity.Version;
            StockLevels = StockLevelDocument.FromEntity(entity.StockLevels)!;
            Reservations = entity.Reservations.Select(x => ReservationDocument.FromEntity(x)!).ToList();
            QualityControl = QualityControlDocument.FromEntity(entity.QualityControl)!;
            MovementHistory = entity.MovementHistory.Select(x => MovementHistoryDocument.FromEntity(x)!).ToList();
            Location = InventoryItemLocationDocument.FromEntity(entity.Location)!;
            CostTracking = CostTrackingDocument.FromEntity(entity.CostTracking)!;
            Alerts = entity.Alerts.Select(x => AlertDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static InventoryItemDocument? FromEntity(InventoryItem? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new InventoryItemDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}