using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Catalog
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Description = null!;
            Sku = null!;
            Category = null!;
            Images = null!;
            Specifications = null!;
        }

        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal Stock { get; set; }
        public bool IsAvailable { get; set; }
        public CategoryDto Category { get; set; }
        public List<ImageDto> Images { get; set; }
        public List<SpecificationDto> Specifications { get; set; }

        public static ProductDto Create(
            Guid productId,
            string name,
            string description,
            string sku,
            decimal price,
            decimal stock,
            bool isAvailable,
            CategoryDto category,
            List<ImageDto> images,
            List<SpecificationDto> specifications)
        {
            return new ProductDto
            {
                ProductId = productId,
                Name = name,
                Description = description,
                Sku = sku,
                Price = price,
                Stock = stock,
                IsAvailable = isAvailable,
                Category = category,
                Images = images,
                Specifications = specifications
            };
        }
    }
}