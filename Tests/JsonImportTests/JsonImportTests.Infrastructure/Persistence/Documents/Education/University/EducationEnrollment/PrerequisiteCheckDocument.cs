using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class PrerequisiteCheckDocument : IPrerequisiteCheckDocument
    {
        public string Id { get; set; } = default!;
        public Guid RequiredCourseId { get; set; }
        public string RequiredCourse { get; set; } = default!;
        public string MinimumGrade { get; set; } = default!;
        public string StudentGrade { get; set; } = default!;
        public bool IsMet { get; set; }
        public DateTime CheckedDate { get; set; }

        public PrerequisiteCheck ToEntity(PrerequisiteCheck? entity = default)
        {
            entity ??= new PrerequisiteCheck();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.RequiredCourseId = RequiredCourseId;
            entity.RequiredCourse = RequiredCourse ?? throw new Exception($"{nameof(entity.RequiredCourse)} is null");
            entity.MinimumGrade = MinimumGrade ?? throw new Exception($"{nameof(entity.MinimumGrade)} is null");
            entity.StudentGrade = StudentGrade ?? throw new Exception($"{nameof(entity.StudentGrade)} is null");
            entity.IsMet = IsMet;
            entity.CheckedDate = CheckedDate;

            return entity;
        }

        public PrerequisiteCheckDocument PopulateFromEntity(PrerequisiteCheck entity)
        {
            Id = entity.Id;
            RequiredCourseId = entity.RequiredCourseId;
            RequiredCourse = entity.RequiredCourse;
            MinimumGrade = entity.MinimumGrade;
            StudentGrade = entity.StudentGrade;
            IsMet = entity.IsMet;
            CheckedDate = entity.CheckedDate;

            return this;
        }

        public static PrerequisiteCheckDocument? FromEntity(PrerequisiteCheck? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PrerequisiteCheckDocument().PopulateFromEntity(entity);
        }
    }
}