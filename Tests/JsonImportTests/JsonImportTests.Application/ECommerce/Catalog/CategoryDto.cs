using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Catalog
{
    public class CategoryDto
    {
        public CategoryDto()
        {
            Name = null!;
            Description = null!;
        }

        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static CategoryDto Create(Guid categoryId, string name, string description)
        {
            return new CategoryDto
            {
                CategoryId = categoryId,
                Name = name,
                Description = description
            };
        }
    }
}