using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class PractitionerDocument : IPractitionerDocument, ICosmosDBDocument<Practitioner, PractitionerDocument>
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
        public string EmployeeId { get; set; } = default!;
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public WorkScheduleDocument WorkSchedule { get; set; } = default!;
        IWorkScheduleDocument IPractitionerDocument.WorkSchedule => WorkSchedule;
        public SupervisorDocument Supervisor { get; set; } = default!;
        ISupervisorDocument IPractitionerDocument.Supervisor => Supervisor;
        public ProfessionalInfoDocument ProfessionalInfo { get; set; } = default!;
        IProfessionalInfoDocument IPractitionerDocument.ProfessionalInfo => ProfessionalInfo;
        public PractitionerPersonalInfoDocument PersonalInfo { get; set; } = default!;
        IPractitionerPersonalInfoDocument IPractitionerDocument.PersonalInfo => PersonalInfo;
        public List<EducationBackgroundDocument> EducationBackground { get; set; } = default!;
        IReadOnlyList<IEducationBackgroundDocument> IPractitionerDocument.EducationBackground => EducationBackground;
        public PractitionerContactInfoDocument ContactInfo { get; set; } = default!;
        IPractitionerContactInfoDocument IPractitionerDocument.ContactInfo => ContactInfo;
        public List<CertificationDocument> Certifications { get; set; } = default!;
        IReadOnlyList<ICertificationDocument> IPractitionerDocument.Certifications => Certifications;

        public Practitioner ToEntity(Practitioner? entity = default)
        {
            entity ??= new Practitioner();

            entity.Id = Guid.Parse(Id);
            entity.EmployeeId = EmployeeId ?? throw new Exception($"{nameof(entity.EmployeeId)} is null");
            entity.HireDate = HireDate;
            entity.IsActive = IsActive;
            entity.WorkSchedule = WorkSchedule.ToEntity() ?? throw new Exception($"{nameof(entity.WorkSchedule)} is null");
            entity.Supervisor = Supervisor.ToEntity() ?? throw new Exception($"{nameof(entity.Supervisor)} is null");
            entity.ProfessionalInfo = ProfessionalInfo.ToEntity() ?? throw new Exception($"{nameof(entity.ProfessionalInfo)} is null");
            entity.PersonalInfo = PersonalInfo.ToEntity() ?? throw new Exception($"{nameof(entity.PersonalInfo)} is null");
            entity.EducationBackground = EducationBackground.Select(x => x.ToEntity()).ToList();
            entity.ContactInfo = ContactInfo.ToEntity() ?? throw new Exception($"{nameof(entity.ContactInfo)} is null");
            entity.Certifications = Certifications.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public PractitionerDocument PopulateFromEntity(Practitioner entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            EmployeeId = entity.EmployeeId;
            HireDate = entity.HireDate;
            IsActive = entity.IsActive;
            WorkSchedule = WorkScheduleDocument.FromEntity(entity.WorkSchedule)!;
            Supervisor = SupervisorDocument.FromEntity(entity.Supervisor)!;
            ProfessionalInfo = ProfessionalInfoDocument.FromEntity(entity.ProfessionalInfo)!;
            PersonalInfo = PractitionerPersonalInfoDocument.FromEntity(entity.PersonalInfo)!;
            EducationBackground = entity.EducationBackground.Select(x => EducationBackgroundDocument.FromEntity(x)!).ToList();
            ContactInfo = PractitionerContactInfoDocument.FromEntity(entity.ContactInfo)!;
            Certifications = entity.Certifications.Select(x => CertificationDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static PractitionerDocument? FromEntity(Practitioner? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new PractitionerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}