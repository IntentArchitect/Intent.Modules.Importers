using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class FilterDocument : IFilterDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<OptionDocument> Options { get; set; } = default!;
        IReadOnlyList<IOptionDocument> IFilterDocument.Options => Options;

        public Filter ToEntity(Filter? entity = default)
        {
            entity ??= new Filter();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Options = Options.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public FilterDocument PopulateFromEntity(Filter entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Type = entity.Type;
            Options = entity.Options.Select(x => OptionDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static FilterDocument? FromEntity(Filter? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FilterDocument().PopulateFromEntity(entity);
        }
    }
}