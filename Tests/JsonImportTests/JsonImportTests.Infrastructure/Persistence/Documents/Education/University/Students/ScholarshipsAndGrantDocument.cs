using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class ScholarshipsAndGrantDocument : IScholarshipsAndGrantDocument
    {
        public string Id { get; set; } = default!;
        public Guid AwardId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Semester { get; set; } = default!;
        public decimal Year { get; set; }
        public bool Renewable { get; set; }
        public string Requirements { get; set; } = default!;

        public ScholarshipsAndGrant ToEntity(ScholarshipsAndGrant? entity = default)
        {
            entity ??= new ScholarshipsAndGrant();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.AwardId = AwardId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Amount = Amount;
            entity.Semester = Semester ?? throw new Exception($"{nameof(entity.Semester)} is null");
            entity.Year = Year;
            entity.Renewable = Renewable;
            entity.Requirements = Requirements ?? throw new Exception($"{nameof(entity.Requirements)} is null");

            return entity;
        }

        public ScholarshipsAndGrantDocument PopulateFromEntity(ScholarshipsAndGrant entity)
        {
            Id = entity.Id;
            AwardId = entity.AwardId;
            Name = entity.Name;
            Type = entity.Type;
            Amount = entity.Amount;
            Semester = entity.Semester;
            Year = entity.Year;
            Renewable = entity.Renewable;
            Requirements = entity.Requirements;

            return this;
        }

        public static ScholarshipsAndGrantDocument? FromEntity(ScholarshipsAndGrant? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ScholarshipsAndGrantDocument().PopulateFromEntity(entity);
        }
    }
}