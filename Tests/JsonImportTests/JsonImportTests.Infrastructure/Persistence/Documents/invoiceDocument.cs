using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IInvoiceDocument, ICosmosDBDocument<Invoice, InvoiceDocument>
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
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ItemDocument> Items { get; set; } = default!;
        IReadOnlyList<IItemDocument> IInvoiceDocument.Items => Items;

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= new Invoice();

            entity.Id = Guid.Parse(Id);
            entity.CustomerId = CustomerId;
            entity.TotalAmount = TotalAmount;
            entity.Items = Items.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            CustomerId = entity.CustomerId;
            TotalAmount = entity.TotalAmount;
            Items = entity.Items.Select(x => ItemDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static InvoiceDocument? FromEntity(Invoice? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}