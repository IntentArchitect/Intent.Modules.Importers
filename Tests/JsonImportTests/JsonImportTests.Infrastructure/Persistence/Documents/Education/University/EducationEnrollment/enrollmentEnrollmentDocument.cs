using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class enrollmentEnrollmentDocument : IenrollmentEnrollmentDocument, ICosmosDBDocument<enrollmentEnrollment, enrollmentEnrollmentDocument>
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
        public Guid StudentId { get; set; }
        public Guid CourseOfferingId { get; set; }
        public Guid SemesterId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string EnrollmentStatus { get; set; } = default!;
        public string EnrollmentType { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; } = default!;
        public decimal Version { get; set; }
        public bool IsActive { get; set; }
        public WithdrawalInfoDocument WithdrawalInfo { get; set; } = default!;
        IWithdrawalInfoDocument IenrollmentEnrollmentDocument.WithdrawalInfo => WithdrawalInfo;
        public EnrollmentStudentDocument Student { get; set; } = default!;
        IEnrollmentStudentDocument IenrollmentEnrollmentDocument.Student => Student;
        public SpecialCircumstanceDocument SpecialCircumstances { get; set; } = default!;
        ISpecialCircumstanceDocument IenrollmentEnrollmentDocument.SpecialCircumstances => SpecialCircumstances;
        public SemesterDocument Semester { get; set; } = default!;
        ISemesterDocument IenrollmentEnrollmentDocument.Semester => Semester;
        public EnrollmentPrerequisiteDocument Prerequisites { get; set; } = default!;
        IEnrollmentPrerequisiteDocument IenrollmentEnrollmentDocument.Prerequisites => Prerequisites;
        public ParticipationDocument Participation { get; set; } = default!;
        IParticipationDocument IenrollmentEnrollmentDocument.Participation => Participation;
        public EnrollmentInstructorDocument Instructor { get; set; } = default!;
        IEnrollmentInstructorDocument IenrollmentEnrollmentDocument.Instructor => Instructor;
        public GradingInfoDocument GradingInfo { get; set; } = default!;
        IGradingInfoDocument IenrollmentEnrollmentDocument.GradingInfo => GradingInfo;
        public EnrollmentFinancialInfoDocument FinancialInfo { get; set; } = default!;
        IEnrollmentFinancialInfoDocument IenrollmentEnrollmentDocument.FinancialInfo => FinancialInfo;
        public EnrollmentCourseDocument Course { get; set; } = default!;
        IEnrollmentCourseDocument IenrollmentEnrollmentDocument.Course => Course;
        public AttendanceDocument Attendance { get; set; } = default!;
        IAttendanceDocument IenrollmentEnrollmentDocument.Attendance => Attendance;
        public List<AssessmentDocument> Assessments { get; set; } = default!;
        IReadOnlyList<IAssessmentDocument> IenrollmentEnrollmentDocument.Assessments => Assessments;

        public enrollmentEnrollment ToEntity(enrollmentEnrollment? entity = default)
        {
            entity ??= new enrollmentEnrollment();

            entity.Id = Guid.Parse(Id);
            entity.StudentId = StudentId;
            entity.CourseOfferingId = CourseOfferingId;
            entity.SemesterId = SemesterId;
            entity.EnrollmentDate = EnrollmentDate;
            entity.EnrollmentStatus = EnrollmentStatus ?? throw new Exception($"{nameof(entity.EnrollmentStatus)} is null");
            entity.EnrollmentType = EnrollmentType ?? throw new Exception($"{nameof(entity.EnrollmentType)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.ModifiedBy = ModifiedBy ?? throw new Exception($"{nameof(entity.ModifiedBy)} is null");
            entity.Version = Version;
            entity.IsActive = IsActive;
            entity.WithdrawalInfo = WithdrawalInfo.ToEntity() ?? throw new Exception($"{nameof(entity.WithdrawalInfo)} is null");
            entity.Student = Student.ToEntity() ?? throw new Exception($"{nameof(entity.Student)} is null");
            entity.SpecialCircumstances = SpecialCircumstances.ToEntity() ?? throw new Exception($"{nameof(entity.SpecialCircumstances)} is null");
            entity.Semester = Semester.ToEntity() ?? throw new Exception($"{nameof(entity.Semester)} is null");
            entity.Prerequisites = Prerequisites.ToEntity() ?? throw new Exception($"{nameof(entity.Prerequisites)} is null");
            entity.Participation = Participation.ToEntity() ?? throw new Exception($"{nameof(entity.Participation)} is null");
            entity.Instructor = Instructor.ToEntity() ?? throw new Exception($"{nameof(entity.Instructor)} is null");
            entity.GradingInfo = GradingInfo.ToEntity() ?? throw new Exception($"{nameof(entity.GradingInfo)} is null");
            entity.FinancialInfo = FinancialInfo.ToEntity() ?? throw new Exception($"{nameof(entity.FinancialInfo)} is null");
            entity.Course = Course.ToEntity() ?? throw new Exception($"{nameof(entity.Course)} is null");
            entity.Attendance = Attendance.ToEntity() ?? throw new Exception($"{nameof(entity.Attendance)} is null");
            entity.Assessments = Assessments.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public enrollmentEnrollmentDocument PopulateFromEntity(enrollmentEnrollment entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            StudentId = entity.StudentId;
            CourseOfferingId = entity.CourseOfferingId;
            SemesterId = entity.SemesterId;
            EnrollmentDate = entity.EnrollmentDate;
            EnrollmentStatus = entity.EnrollmentStatus;
            EnrollmentType = entity.EnrollmentType;
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            ModifiedBy = entity.ModifiedBy;
            Version = entity.Version;
            IsActive = entity.IsActive;
            WithdrawalInfo = WithdrawalInfoDocument.FromEntity(entity.WithdrawalInfo)!;
            Student = EnrollmentStudentDocument.FromEntity(entity.Student)!;
            SpecialCircumstances = SpecialCircumstanceDocument.FromEntity(entity.SpecialCircumstances)!;
            Semester = SemesterDocument.FromEntity(entity.Semester)!;
            Prerequisites = EnrollmentPrerequisiteDocument.FromEntity(entity.Prerequisites)!;
            Participation = ParticipationDocument.FromEntity(entity.Participation)!;
            Instructor = EnrollmentInstructorDocument.FromEntity(entity.Instructor)!;
            GradingInfo = GradingInfoDocument.FromEntity(entity.GradingInfo)!;
            FinancialInfo = EnrollmentFinancialInfoDocument.FromEntity(entity.FinancialInfo)!;
            Course = EnrollmentCourseDocument.FromEntity(entity.Course)!;
            Attendance = AttendanceDocument.FromEntity(entity.Attendance)!;
            Assessments = entity.Assessments.Select(x => AssessmentDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static enrollmentEnrollmentDocument? FromEntity(enrollmentEnrollment? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new enrollmentEnrollmentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}