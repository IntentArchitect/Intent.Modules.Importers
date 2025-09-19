using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientDocument : IPatientDocument, ICosmosDBDocument<Patient, PatientDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string PatientNumber { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public PatientPersonalInfoDocument PersonalInfo { get; set; } = default!;
        IPatientPersonalInfoDocument IPatientDocument.PersonalInfo => PersonalInfo;
        public MedicalHistoryDocument MedicalHistory { get; set; } = default!;
        IMedicalHistoryDocument IPatientDocument.MedicalHistory => MedicalHistory;
        public PatientInsuranceInfoDocument InsuranceInfo { get; set; } = default!;
        IPatientInsuranceInfoDocument IPatientDocument.InsuranceInfo => InsuranceInfo;
        public List<PatientEmergencyContactDocument> EmergencyContacts { get; set; } = default!;
        IReadOnlyList<IPatientEmergencyContactDocument> IPatientDocument.EmergencyContacts => EmergencyContacts;
        public PatientContactInfoDocument ContactInfo { get; set; } = default!;
        IPatientContactInfoDocument IPatientDocument.ContactInfo => ContactInfo;

        public Patient ToEntity(Patient? entity = default)
        {
            entity ??= new Patient();

            entity.Id = Guid.Parse(Id);
            entity.PatientNumber = PatientNumber ?? throw new Exception($"{nameof(entity.PatientNumber)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastUpdated = LastUpdated;
            entity.IsActive = IsActive;
            entity.PersonalInfo = PersonalInfo.ToEntity() ?? throw new Exception($"{nameof(entity.PersonalInfo)} is null");
            entity.MedicalHistory = MedicalHistory.ToEntity() ?? throw new Exception($"{nameof(entity.MedicalHistory)} is null");
            entity.InsuranceInfo = InsuranceInfo.ToEntity() ?? throw new Exception($"{nameof(entity.InsuranceInfo)} is null");
            entity.EmergencyContacts = EmergencyContacts.Select(x => x.ToEntity()).ToList();
            entity.ContactInfo = ContactInfo.ToEntity() ?? throw new Exception($"{nameof(entity.ContactInfo)} is null");

            return entity;
        }

        public PatientDocument PopulateFromEntity(Patient entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            PatientNumber = entity.PatientNumber;
            CreatedDate = entity.CreatedDate;
            LastUpdated = entity.LastUpdated;
            IsActive = entity.IsActive;
            PersonalInfo = PatientPersonalInfoDocument.FromEntity(entity.PersonalInfo)!;
            MedicalHistory = MedicalHistoryDocument.FromEntity(entity.MedicalHistory)!;
            InsuranceInfo = PatientInsuranceInfoDocument.FromEntity(entity.InsuranceInfo)!;
            EmergencyContacts = entity.EmergencyContacts.Select(x => PatientEmergencyContactDocument.FromEntity(x)!).ToList();
            ContactInfo = PatientContactInfoDocument.FromEntity(entity.ContactInfo)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static PatientDocument? FromEntity(Patient? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}