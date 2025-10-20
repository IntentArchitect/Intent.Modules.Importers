using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Catalog
{
    public class SpecificationDto
    {
        public SpecificationDto()
        {
            Key = null!;
            Value = null!;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public static SpecificationDto Create(string key, string value)
        {
            return new SpecificationDto
            {
                Key = key,
                Value = value
            };
        }
    }
}