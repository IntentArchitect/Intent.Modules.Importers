using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class CorequisiteDocument : ICorequisiteDocument
    {
        public string Id { get; set; } = default!;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public bool CanBeTakenConcurrently { get; set; }

        public Corequisite ToEntity(Corequisite? entity = default)
        {
            entity ??= new Corequisite();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CourseId = CourseId;
            entity.CourseCode = CourseCode ?? throw new Exception($"{nameof(entity.CourseCode)} is null");
            entity.CourseName = CourseName ?? throw new Exception($"{nameof(entity.CourseName)} is null");
            entity.CanBeTakenConcurrently = CanBeTakenConcurrently;

            return entity;
        }

        public CorequisiteDocument PopulateFromEntity(Corequisite entity)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            CourseCode = entity.CourseCode;
            CourseName = entity.CourseName;
            CanBeTakenConcurrently = entity.CanBeTakenConcurrently;

            return this;
        }

        public static CorequisiteDocument? FromEntity(Corequisite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CorequisiteDocument().PopulateFromEntity(entity);
        }
    }
}