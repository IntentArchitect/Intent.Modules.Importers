using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class MeetingTimeDocument : IMeetingTimeDocument
    {
        public string Id { get; set; } = default!;
        public string Day { get; set; } = default!;
        public string StartTime { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public MeetingTimesLocationDocument Location { get; set; } = default!;
        IMeetingTimesLocationDocument IMeetingTimeDocument.Location => Location;

        public MeetingTime ToEntity(MeetingTime? entity = default)
        {
            entity ??= new MeetingTime();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Day = Day ?? throw new Exception($"{nameof(entity.Day)} is null");
            entity.StartTime = StartTime ?? throw new Exception($"{nameof(entity.StartTime)} is null");
            entity.EndTime = EndTime ?? throw new Exception($"{nameof(entity.EndTime)} is null");
            entity.Location = Location.ToEntity() ?? throw new Exception($"{nameof(entity.Location)} is null");

            return entity;
        }

        public MeetingTimeDocument PopulateFromEntity(MeetingTime entity)
        {
            Id = entity.Id;
            Day = entity.Day;
            StartTime = entity.StartTime;
            EndTime = entity.EndTime;
            Location = MeetingTimesLocationDocument.FromEntity(entity.Location)!;

            return this;
        }

        public static MeetingTimeDocument? FromEntity(MeetingTime? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MeetingTimeDocument().PopulateFromEntity(entity);
        }
    }
}