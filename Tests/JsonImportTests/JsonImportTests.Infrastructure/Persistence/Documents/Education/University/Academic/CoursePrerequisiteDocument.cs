using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class CoursePrerequisiteDocument : ICoursePrerequisiteDocument
    {
        public string Id { get; set; } = default!;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string MinimumGrade { get; set; } = default!;
        public bool IsRequired { get; set; }

        public CoursePrerequisite ToEntity(CoursePrerequisite? entity = default)
        {
            entity ??= new CoursePrerequisite();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CourseId = CourseId;
            entity.CourseCode = CourseCode ?? throw new Exception($"{nameof(entity.CourseCode)} is null");
            entity.CourseName = CourseName ?? throw new Exception($"{nameof(entity.CourseName)} is null");
            entity.MinimumGrade = MinimumGrade ?? throw new Exception($"{nameof(entity.MinimumGrade)} is null");
            entity.IsRequired = IsRequired;

            return entity;
        }

        public CoursePrerequisiteDocument PopulateFromEntity(CoursePrerequisite entity)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            CourseCode = entity.CourseCode;
            CourseName = entity.CourseName;
            MinimumGrade = entity.MinimumGrade;
            IsRequired = entity.IsRequired;

            return this;
        }

        public static CoursePrerequisiteDocument? FromEntity(CoursePrerequisite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CoursePrerequisiteDocument().PopulateFromEntity(entity);
        }
    }
}