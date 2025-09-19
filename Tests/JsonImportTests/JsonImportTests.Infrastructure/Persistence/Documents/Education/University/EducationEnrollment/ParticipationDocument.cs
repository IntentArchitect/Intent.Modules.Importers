using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class ParticipationDocument : IParticipationDocument
    {
        public string ParticipationGrade { get; set; } = default!;
        public decimal ParticipationPoints { get; set; }
        public decimal MaxParticipationPoints { get; set; }
        public string ParticipationComments { get; set; } = default!;
        public string EngagementLevel { get; set; } = default!;

        public Participation ToEntity(Participation? entity = default)
        {
            entity ??= new Participation();

            entity.ParticipationGrade = ParticipationGrade ?? throw new Exception($"{nameof(entity.ParticipationGrade)} is null");
            entity.ParticipationPoints = ParticipationPoints;
            entity.MaxParticipationPoints = MaxParticipationPoints;
            entity.ParticipationComments = ParticipationComments ?? throw new Exception($"{nameof(entity.ParticipationComments)} is null");
            entity.EngagementLevel = EngagementLevel ?? throw new Exception($"{nameof(entity.EngagementLevel)} is null");

            return entity;
        }

        public ParticipationDocument PopulateFromEntity(Participation entity)
        {
            ParticipationGrade = entity.ParticipationGrade;
            ParticipationPoints = entity.ParticipationPoints;
            MaxParticipationPoints = entity.MaxParticipationPoints;
            ParticipationComments = entity.ParticipationComments;
            EngagementLevel = entity.EngagementLevel;

            return this;
        }

        public static ParticipationDocument? FromEntity(Participation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ParticipationDocument().PopulateFromEntity(entity);
        }
    }
}