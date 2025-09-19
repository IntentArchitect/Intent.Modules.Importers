using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents
{
    internal class CustomerDocument : ICustomerDocument, ICosmosDBDocument<Customer, CustomerDocument>
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
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<OrderDocument> Orders { get; set; } = default!;
        IReadOnlyList<IOrderDocument> ICustomerDocument.Orders => Orders;

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Orders = Orders.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;
            Email = entity.Email;
            Orders = entity.Orders.Select(x => OrderDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static CustomerDocument? FromEntity(Customer? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}