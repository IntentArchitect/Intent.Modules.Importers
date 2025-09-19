using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class SportDocument : ISportDocument
    {
        public Guid SportId { get; set; }
        public string SportName { get; set; } = default!;
        public string Position { get; set; } = default!;
        public string Season { get; set; } = default!;
        public decimal Year { get; set; }
        public decimal ScholarshipAmount { get; set; }

        public Sport ToEntity(Sport? entity = default)
        {
            entity ??= new Sport();

            entity.SportId = SportId;
            entity.SportName = SportName ?? throw new Exception($"{nameof(entity.SportName)} is null");
            entity.Position = Position ?? throw new Exception($"{nameof(entity.Position)} is null");
            entity.Season = Season ?? throw new Exception($"{nameof(entity.Season)} is null");
            entity.Year = Year;
            entity.ScholarshipAmount = ScholarshipAmount;

            return entity;
        }

        public SportDocument PopulateFromEntity(Sport entity)
        {
            SportId = entity.SportId;
            SportName = entity.SportName;
            Position = entity.Position;
            Season = entity.Season;
            Year = entity.Year;
            ScholarshipAmount = entity.ScholarshipAmount;

            return this;
        }

        public static SportDocument? FromEntity(Sport? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SportDocument().PopulateFromEntity(entity);
        }
    }
}