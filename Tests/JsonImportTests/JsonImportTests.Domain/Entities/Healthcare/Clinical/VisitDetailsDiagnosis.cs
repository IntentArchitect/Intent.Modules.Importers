using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class VisitDetailsDiagnosis
    {
        private string? _id;

        public VisitDetailsDiagnosis()
        {
            Id = null!;
            Code = null!;
            Description = null!;
            Type = null!;
            Certainty = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Certainty { get; set; }
    }
}