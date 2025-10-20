using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Catalog
{
    public class ImageDto
    {
        public ImageDto()
        {
            Url = null!;
            AltText = null!;
        }

        public string Url { get; set; }
        public string AltText { get; set; }
        public decimal Order { get; set; }

        public static ImageDto Create(string url, string altText, decimal order)
        {
            return new ImageDto
            {
                Url = url,
                AltText = altText,
                Order = order
            };
        }
    }
}