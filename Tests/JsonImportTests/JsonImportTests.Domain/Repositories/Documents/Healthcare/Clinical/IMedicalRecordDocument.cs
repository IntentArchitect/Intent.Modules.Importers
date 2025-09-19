using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IMedicalRecordDocument
    {
        string Id { get; }
        Guid PatientId { get; }
        string RecordNumber { get; }
        string RecordType { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        string ModifiedBy { get; }
        decimal Version { get; }
        bool IsActive { get; }
        IVisitInformationDocument VisitInformation { get; }
        IReadOnlyList<IMedicalRecordTreatmentPlanDocument> TreatmentPlans { get; }
        IReadOnlyList<IMedicalRecordMedicationDocument> Medications { get; }
        IReadOnlyList<IDiagnosticTestDocument> DiagnosticTests { get; }
        IReadOnlyList<IMedicalRecordDiagnosisDocument> Diagnoses { get; }
        ICreatedByDocument CreatedBy { get; }
        IClinicalDatumDocument ClinicalData { get; }
        IReadOnlyList<IMedicalRecordAllergyDocument> Allergies { get; }
    }
}