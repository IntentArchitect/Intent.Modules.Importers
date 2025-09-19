using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class DiagnosticTest
    {
        private string? _id;

        public DiagnosticTest()
        {
            Id = null!;
            TestName = null!;
            TestType = null!;
            Results = null!;
            Interpretation = null!;
            OrderingPhysician = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid TestId { get; set; }

        public string TestName { get; set; }

        public string TestType { get; set; }

        public DateTime OrderedDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public string Results { get; set; }

        public string Interpretation { get; set; }

        public string OrderingPhysician { get; set; }
    }
}