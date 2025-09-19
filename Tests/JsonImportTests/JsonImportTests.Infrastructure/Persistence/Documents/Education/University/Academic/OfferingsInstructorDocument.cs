using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class OfferingsInstructorDocument : IOfferingsInstructorDocument
    {
        public Guid InstructorId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Department { get; set; } = default!;

        public OfferingsInstructor ToEntity(OfferingsInstructor? entity = default)
        {
            entity ??= new OfferingsInstructor();

            entity.InstructorId = InstructorId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Department = Department ?? throw new Exception($"{nameof(entity.Department)} is null");

            return entity;
        }

        public OfferingsInstructorDocument PopulateFromEntity(OfferingsInstructor entity)
        {
            InstructorId = entity.InstructorId;
            Name = entity.Name;
            Email = entity.Email;
            Title = entity.Title;
            Department = entity.Department;

            return this;
        }

        public static OfferingsInstructorDocument? FromEntity(OfferingsInstructor? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OfferingsInstructorDocument().PopulateFromEntity(entity);
        }
    }
}