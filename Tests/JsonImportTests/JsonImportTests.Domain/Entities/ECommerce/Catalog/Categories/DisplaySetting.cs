using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class DisplaySetting
    {
        public DisplaySetting()
        {
            BannerImage = null!;
            IconUrl = null!;
            Color = null!;
            LayoutTemplate = null!;
        }

        public string BannerImage { get; set; }

        public string IconUrl { get; set; }

        public string Color { get; set; }

        public string LayoutTemplate { get; set; }
    }
}