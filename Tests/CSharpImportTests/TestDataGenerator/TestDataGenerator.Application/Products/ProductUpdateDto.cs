using System;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ProductType Type { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, ProductType type)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Type = type
            };
        }
    }
}