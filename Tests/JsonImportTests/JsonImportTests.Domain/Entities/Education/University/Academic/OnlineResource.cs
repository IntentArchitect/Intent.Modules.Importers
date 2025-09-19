using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class OnlineResource
    {
        private string? _id;

        public OnlineResource()
        {
            Id = null!;
            Title = null!;
            URL = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Title { get; set; }

        public string URL { get; set; }

        public string Type { get; set; }

        public bool AccessRequired { get; set; }
    }
}