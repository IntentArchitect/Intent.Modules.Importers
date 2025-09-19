using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IPractitionerDocument
    {
        string Id { get; }
        string EmployeeId { get; }
        DateTime HireDate { get; }
        bool IsActive { get; }
        IWorkScheduleDocument WorkSchedule { get; }
        ISupervisorDocument Supervisor { get; }
        IProfessionalInfoDocument ProfessionalInfo { get; }
        IPractitionerPersonalInfoDocument PersonalInfo { get; }
        IReadOnlyList<IEducationBackgroundDocument> EducationBackground { get; }
        IPractitionerContactInfoDocument ContactInfo { get; }
        IReadOnlyList<ICertificationDocument> Certifications { get; }
    }
}