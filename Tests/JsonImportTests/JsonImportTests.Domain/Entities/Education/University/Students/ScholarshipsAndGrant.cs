using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class ScholarshipsAndGrant
    {
        private string? _id;

        public ScholarshipsAndGrant()
        {
            Id = null!;
            Name = null!;
            Type = null!;
            Semester = null!;
            Requirements = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid AwardId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string Semester { get; set; }

        public decimal Year { get; set; }

        public bool Renewable { get; set; }

        public string Requirements { get; set; }
    }
}