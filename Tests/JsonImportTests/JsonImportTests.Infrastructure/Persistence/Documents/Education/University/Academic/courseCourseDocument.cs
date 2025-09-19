using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class courseCourseDocument : IcourseCourseDocument, ICosmosDBDocument<courseCourse, courseCourseDocument>
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
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string CourseTitle { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Credits { get; set; }
        public string CourseLevel { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string CreatedBy { get; set; } = default!;
        public bool IsActive { get; set; }
        public bool IsOfferedOnline { get; set; }
        public bool IsOfferedInPerson { get; set; }
        public List<CoursePrerequisiteDocument> Prerequisites { get; set; } = default!;
        IReadOnlyList<ICoursePrerequisiteDocument> IcourseCourseDocument.Prerequisites => Prerequisites;
        public List<OfferingDocument> Offerings { get; set; } = default!;
        IReadOnlyList<IOfferingDocument> IcourseCourseDocument.Offerings => Offerings;
        public List<LearningOutcomeDocument> LearningOutcomes { get; set; } = default!;
        IReadOnlyList<ILearningOutcomeDocument> IcourseCourseDocument.LearningOutcomes => LearningOutcomes;
        public DepartmentDocument Department { get; set; } = default!;
        IDepartmentDocument IcourseCourseDocument.Department => Department;
        public CourseContentDocument CourseContent { get; set; } = default!;
        ICourseContentDocument IcourseCourseDocument.CourseContent => CourseContent;
        public List<CorequisiteDocument> Corequisites { get; set; } = default!;
        IReadOnlyList<ICorequisiteDocument> IcourseCourseDocument.Corequisites => Corequisites;
        public AccreditationDocument Accreditation { get; set; } = default!;
        IAccreditationDocument IcourseCourseDocument.Accreditation => Accreditation;

        public courseCourse ToEntity(courseCourse? entity = default)
        {
            entity ??= new courseCourse();

            entity.Id = Guid.Parse(Id);
            entity.CourseCode = CourseCode ?? throw new Exception($"{nameof(entity.CourseCode)} is null");
            entity.CourseName = CourseName ?? throw new Exception($"{nameof(entity.CourseName)} is null");
            entity.CourseTitle = CourseTitle ?? throw new Exception($"{nameof(entity.CourseTitle)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Credits = Credits;
            entity.CourseLevel = CourseLevel ?? throw new Exception($"{nameof(entity.CourseLevel)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.IsActive = IsActive;
            entity.IsOfferedOnline = IsOfferedOnline;
            entity.IsOfferedInPerson = IsOfferedInPerson;
            entity.Prerequisites = Prerequisites.Select(x => x.ToEntity()).ToList();
            entity.Offerings = Offerings.Select(x => x.ToEntity()).ToList();
            entity.LearningOutcomes = LearningOutcomes.Select(x => x.ToEntity()).ToList();
            entity.Department = Department.ToEntity() ?? throw new Exception($"{nameof(entity.Department)} is null");
            entity.CourseContent = CourseContent.ToEntity() ?? throw new Exception($"{nameof(entity.CourseContent)} is null");
            entity.Corequisites = Corequisites.Select(x => x.ToEntity()).ToList();
            entity.Accreditation = Accreditation.ToEntity() ?? throw new Exception($"{nameof(entity.Accreditation)} is null");

            return entity;
        }

        public courseCourseDocument PopulateFromEntity(courseCourse entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            CourseCode = entity.CourseCode;
            CourseName = entity.CourseName;
            CourseTitle = entity.CourseTitle;
            Description = entity.Description;
            Credits = entity.Credits;
            CourseLevel = entity.CourseLevel;
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            CreatedBy = entity.CreatedBy;
            IsActive = entity.IsActive;
            IsOfferedOnline = entity.IsOfferedOnline;
            IsOfferedInPerson = entity.IsOfferedInPerson;
            Prerequisites = entity.Prerequisites.Select(x => CoursePrerequisiteDocument.FromEntity(x)!).ToList();
            Offerings = entity.Offerings.Select(x => OfferingDocument.FromEntity(x)!).ToList();
            LearningOutcomes = entity.LearningOutcomes.Select(x => LearningOutcomeDocument.FromEntity(x)!).ToList();
            Department = DepartmentDocument.FromEntity(entity.Department)!;
            CourseContent = CourseContentDocument.FromEntity(entity.CourseContent)!;
            Corequisites = entity.Corequisites.Select(x => CorequisiteDocument.FromEntity(x)!).ToList();
            Accreditation = AccreditationDocument.FromEntity(entity.Accreditation)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static courseCourseDocument? FromEntity(courseCourse? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new courseCourseDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}