using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class MajorProgramDocument : IMajorProgramDocument
    {
        public string Id { get; set; } = default!;
        public Guid ProgramId { get; set; }
        public string ProgramName { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public string Major { get; set; } = default!;
        public string Minor { get; set; } = default!;
        public string Concentration { get; set; } = default!;
        public DateTime DeclaredDate { get; set; }

        public MajorProgram ToEntity(MajorProgram? entity = default)
        {
            entity ??= new MajorProgram();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ProgramId = ProgramId;
            entity.ProgramName = ProgramName ?? throw new Exception($"{nameof(entity.ProgramName)} is null");
            entity.DegreeType = DegreeType ?? throw new Exception($"{nameof(entity.DegreeType)} is null");
            entity.Major = Major ?? throw new Exception($"{nameof(entity.Major)} is null");
            entity.Minor = Minor ?? throw new Exception($"{nameof(entity.Minor)} is null");
            entity.Concentration = Concentration ?? throw new Exception($"{nameof(entity.Concentration)} is null");
            entity.DeclaredDate = DeclaredDate;

            return entity;
        }

        public MajorProgramDocument PopulateFromEntity(MajorProgram entity)
        {
            Id = entity.Id;
            ProgramId = entity.ProgramId;
            ProgramName = entity.ProgramName;
            DegreeType = entity.DegreeType;
            Major = entity.Major;
            Minor = entity.Minor;
            Concentration = entity.Concentration;
            DeclaredDate = entity.DeclaredDate;

            return this;
        }

        public static MajorProgramDocument? FromEntity(MajorProgram? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MajorProgramDocument().PopulateFromEntity(entity);
        }
    }
}