using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class Option
    {
        private string? _id;

        public Option()
        {
            Id = null!;
            Label = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Label { get; set; }

        public decimal MinValue { get; set; }

        public decimal MaxValue { get; set; }
    }
}