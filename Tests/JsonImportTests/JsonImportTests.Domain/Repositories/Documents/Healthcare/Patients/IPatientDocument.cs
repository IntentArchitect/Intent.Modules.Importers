using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IPatientDocument
    {
        string Id { get; }
        string PatientNumber { get; }
        DateTime CreatedDate { get; }
        DateTime LastUpdated { get; }
        bool IsActive { get; }
        IPatientPersonalInfoDocument PersonalInfo { get; }
        IMedicalHistoryDocument MedicalHistory { get; }
        IPatientInsuranceInfoDocument InsuranceInfo { get; }
        IReadOnlyList<IPatientEmergencyContactDocument> EmergencyContacts { get; }
        IPatientContactInfoDocument ContactInfo { get; }
    }
}