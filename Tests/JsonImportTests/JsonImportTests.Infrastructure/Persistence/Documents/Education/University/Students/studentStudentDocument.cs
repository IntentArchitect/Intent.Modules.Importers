using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class studentStudentDocument : IstudentStudentDocument, ICosmosDBDocument<studentStudent, studentStudentDocument>
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
        public string StudentId { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public bool PrivacyConsent { get; set; }
        public TechnologyDocument Technology { get; set; } = default!;
        ITechnologyDocument IstudentStudentDocument.Technology => Technology;
        public StudentPersonalInfoDocument PersonalInfo { get; set; } = default!;
        IStudentPersonalInfoDocument IstudentStudentDocument.PersonalInfo => PersonalInfo;
        public HealthAndServiceDocument HealthAndServices { get; set; } = default!;
        IHealthAndServiceDocument IstudentStudentDocument.HealthAndServices => HealthAndServices;
        public StudentFinancialInfoDocument FinancialInfo { get; set; } = default!;
        IStudentFinancialInfoDocument IstudentStudentDocument.FinancialInfo => FinancialInfo;
        public List<EnrollmentHistoryDocument> EnrollmentHistory { get; set; } = default!;
        IReadOnlyList<IEnrollmentHistoryDocument> IstudentStudentDocument.EnrollmentHistory => EnrollmentHistory;
        public StudentContactInfoDocument ContactInfo { get; set; } = default!;
        IStudentContactInfoDocument IstudentStudentDocument.ContactInfo => ContactInfo;
        public ActivityDocument Activities { get; set; } = default!;
        IActivityDocument IstudentStudentDocument.Activities => Activities;
        public AcademicInfoDocument AcademicInfo { get; set; } = default!;
        IAcademicInfoDocument IstudentStudentDocument.AcademicInfo => AcademicInfo;

        public studentStudent ToEntity(studentStudent? entity = default)
        {
            entity ??= new studentStudent();

            entity.Id = Guid.Parse(Id);
            entity.StudentId = StudentId ?? throw new Exception($"{nameof(entity.StudentId)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastUpdated = LastUpdated;
            entity.IsActive = IsActive;
            entity.PrivacyConsent = PrivacyConsent;
            entity.Technology = Technology.ToEntity() ?? throw new Exception($"{nameof(entity.Technology)} is null");
            entity.PersonalInfo = PersonalInfo.ToEntity() ?? throw new Exception($"{nameof(entity.PersonalInfo)} is null");
            entity.HealthAndServices = HealthAndServices.ToEntity() ?? throw new Exception($"{nameof(entity.HealthAndServices)} is null");
            entity.FinancialInfo = FinancialInfo.ToEntity() ?? throw new Exception($"{nameof(entity.FinancialInfo)} is null");
            entity.EnrollmentHistory = EnrollmentHistory.Select(x => x.ToEntity()).ToList();
            entity.ContactInfo = ContactInfo.ToEntity() ?? throw new Exception($"{nameof(entity.ContactInfo)} is null");
            entity.Activities = Activities.ToEntity() ?? throw new Exception($"{nameof(entity.Activities)} is null");
            entity.AcademicInfo = AcademicInfo.ToEntity() ?? throw new Exception($"{nameof(entity.AcademicInfo)} is null");

            return entity;
        }

        public studentStudentDocument PopulateFromEntity(studentStudent entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            StudentId = entity.StudentId;
            CreatedDate = entity.CreatedDate;
            LastUpdated = entity.LastUpdated;
            IsActive = entity.IsActive;
            PrivacyConsent = entity.PrivacyConsent;
            Technology = TechnologyDocument.FromEntity(entity.Technology)!;
            PersonalInfo = StudentPersonalInfoDocument.FromEntity(entity.PersonalInfo)!;
            HealthAndServices = HealthAndServiceDocument.FromEntity(entity.HealthAndServices)!;
            FinancialInfo = StudentFinancialInfoDocument.FromEntity(entity.FinancialInfo)!;
            EnrollmentHistory = entity.EnrollmentHistory.Select(x => EnrollmentHistoryDocument.FromEntity(x)!).ToList();
            ContactInfo = StudentContactInfoDocument.FromEntity(entity.ContactInfo)!;
            Activities = ActivityDocument.FromEntity(entity.Activities)!;
            AcademicInfo = AcademicInfoDocument.FromEntity(entity.AcademicInfo)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static studentStudentDocument? FromEntity(studentStudent? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new studentStudentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}