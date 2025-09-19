using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IProfessionalInfoDocument
    {
        string Title { get; }
        string Department { get; }
        string Specialization { get; }
        string LicenseNumber { get; }
        DateTime LicenseExpirationDate { get; }
        decimal YearsOfExperience { get; }
    }
}