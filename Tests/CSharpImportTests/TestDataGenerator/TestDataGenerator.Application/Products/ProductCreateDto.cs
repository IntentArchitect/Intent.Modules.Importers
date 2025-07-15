using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public ProductType Type { get; set; }

        public static ProductCreateDto Create(string name, ProductType type)
        {
            return new ProductCreateDto
            {
                Name = name,
                Type = type
            };
        }
    }
}