using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantsImageDocument : IVariantsImageDocument
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = default!;
        public string AltText { get; set; } = default!;
        public decimal Position { get; set; }
        public bool IsPrimary { get; set; }

        public VariantsImage ToEntity(VariantsImage? entity = default)
        {
            entity ??= new VariantsImage();

            entity.Id = Id;
            entity.Url = Url ?? throw new Exception($"{nameof(entity.Url)} is null");
            entity.AltText = AltText ?? throw new Exception($"{nameof(entity.AltText)} is null");
            entity.Position = Position;
            entity.IsPrimary = IsPrimary;

            return entity;
        }

        public VariantsImageDocument PopulateFromEntity(VariantsImage entity)
        {
            Id = entity.Id;
            Url = entity.Url;
            AltText = entity.AltText;
            Position = entity.Position;
            IsPrimary = entity.IsPrimary;

            return this;
        }

        public static VariantsImageDocument? FromEntity(VariantsImage? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantsImageDocument().PopulateFromEntity(entity);
        }
    }
}