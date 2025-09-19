using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class EducationBackgroundDocument : IEducationBackgroundDocument
    {
        public string Id { get; set; } = default!;
        public string Institution { get; set; } = default!;
        public string Degree { get; set; } = default!;
        public string FieldOfStudy { get; set; } = default!;
        public decimal GraduationYear { get; set; }

        public EducationBackground ToEntity(EducationBackground? entity = default)
        {
            entity ??= new EducationBackground();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Institution = Institution ?? throw new Exception($"{nameof(entity.Institution)} is null");
            entity.Degree = Degree ?? throw new Exception($"{nameof(entity.Degree)} is null");
            entity.FieldOfStudy = FieldOfStudy ?? throw new Exception($"{nameof(entity.FieldOfStudy)} is null");
            entity.GraduationYear = GraduationYear;

            return entity;
        }

        public EducationBackgroundDocument PopulateFromEntity(EducationBackground entity)
        {
            Id = entity.Id;
            Institution = entity.Institution;
            Degree = entity.Degree;
            FieldOfStudy = entity.FieldOfStudy;
            GraduationYear = entity.GraduationYear;

            return this;
        }

        public static EducationBackgroundDocument? FromEntity(EducationBackground? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EducationBackgroundDocument().PopulateFromEntity(entity);
        }
    }
}