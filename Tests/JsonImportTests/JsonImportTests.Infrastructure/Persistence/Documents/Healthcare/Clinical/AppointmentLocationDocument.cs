using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class AppointmentLocationDocument : IAppointmentLocationDocument
    {
        public string Building { get; set; } = default!;
        public string Floor { get; set; } = default!;
        public string RoomNumber { get; set; } = default!;
        public Guid FacilityId { get; set; }

        public AppointmentLocation ToEntity(AppointmentLocation? entity = default)
        {
            entity ??= new AppointmentLocation();

            entity.Building = Building ?? throw new Exception($"{nameof(entity.Building)} is null");
            entity.Floor = Floor ?? throw new Exception($"{nameof(entity.Floor)} is null");
            entity.RoomNumber = RoomNumber ?? throw new Exception($"{nameof(entity.RoomNumber)} is null");
            entity.FacilityId = FacilityId;

            return entity;
        }

        public AppointmentLocationDocument PopulateFromEntity(AppointmentLocation entity)
        {
            Building = entity.Building;
            Floor = entity.Floor;
            RoomNumber = entity.RoomNumber;
            FacilityId = entity.FacilityId;

            return this;
        }

        public static AppointmentLocationDocument? FromEntity(AppointmentLocation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AppointmentLocationDocument().PopulateFromEntity(entity);
        }
    }
}