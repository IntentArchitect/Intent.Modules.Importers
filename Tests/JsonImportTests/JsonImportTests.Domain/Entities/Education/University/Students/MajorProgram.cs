using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class MajorProgram
    {
        private string? _id;

        public MajorProgram()
        {
            Id = null!;
            ProgramName = null!;
            DegreeType = null!;
            Major = null!;
            Minor = null!;
            Concentration = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid ProgramId { get; set; }

        public string ProgramName { get; set; }

        public string DegreeType { get; set; }

        public string Major { get; set; }

        public string Minor { get; set; }

        public string Concentration { get; set; }

        public DateTime DeclaredDate { get; set; }
    }
}