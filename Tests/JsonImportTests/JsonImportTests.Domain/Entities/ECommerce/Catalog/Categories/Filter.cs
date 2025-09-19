using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class Filter
    {
        private string? _id;

        public Filter()
        {
            Id = null!;
            Name = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public ICollection<Option> Options { get; set; } = [];
    }
}