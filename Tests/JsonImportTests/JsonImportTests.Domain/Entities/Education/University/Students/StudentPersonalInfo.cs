using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class StudentPersonalInfo
    {
        public StudentPersonalInfo()
        {
            FirstName = null!;
            LastName = null!;
            MiddleName = null!;
            Gender = null!;
            Nationality = null!;
            CitizenshipStatus = null!;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }

        public string CitizenshipStatus { get; set; }
    }
}