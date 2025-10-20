using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Finance.Banking
{
    public class MetadataDto
    {
        public MetadataDto()
        {
            IpAddress = null!;
            DeviceId = null!;
            Location = null!;
        }

        public string IpAddress { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }

        public static MetadataDto Create(string ipAddress, string deviceId, string location)
        {
            return new MetadataDto
            {
                IpAddress = ipAddress,
                DeviceId = deviceId,
                Location = location
            };
        }
    }
}