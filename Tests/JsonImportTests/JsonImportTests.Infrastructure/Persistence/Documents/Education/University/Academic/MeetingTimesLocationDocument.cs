using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class MeetingTimesLocationDocument : IMeetingTimesLocationDocument
    {
        public string Building { get; set; } = default!;
        public string Room { get; set; } = default!;
        public decimal Capacity { get; set; }
        public List<string> Equipment { get; set; } = default!;
        IReadOnlyList<string> IMeetingTimesLocationDocument.Equipment => Equipment;

        public MeetingTimesLocation ToEntity(MeetingTimesLocation? entity = default)
        {
            entity ??= new MeetingTimesLocation();

            entity.Building = Building ?? throw new Exception($"{nameof(entity.Building)} is null");
            entity.Room = Room ?? throw new Exception($"{nameof(entity.Room)} is null");
            entity.Capacity = Capacity;
            entity.Equipment = Equipment ?? throw new Exception($"{nameof(entity.Equipment)} is null");

            return entity;
        }

        public MeetingTimesLocationDocument PopulateFromEntity(MeetingTimesLocation entity)
        {
            Building = entity.Building;
            Room = entity.Room;
            Capacity = entity.Capacity;
            Equipment = entity.Equipment.ToList();

            return this;
        }

        public static MeetingTimesLocationDocument? FromEntity(MeetingTimesLocation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MeetingTimesLocationDocument().PopulateFromEntity(entity);
        }
    }
}