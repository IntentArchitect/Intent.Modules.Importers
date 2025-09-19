using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class TextBook
    {
        private string? _id;

        public TextBook()
        {
            Id = null!;
            ISBN = null!;
            Title = null!;
            Author = null!;
            Edition = null!;
            Publisher = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string ISBN { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Edition { get; set; }

        public string Publisher { get; set; }

        public bool IsRequired { get; set; }
    }
}