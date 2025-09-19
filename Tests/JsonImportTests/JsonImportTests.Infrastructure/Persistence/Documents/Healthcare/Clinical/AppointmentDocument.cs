using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class AppointmentDocument : IAppointmentDocument, ICosmosDBDocument<Appointment, AppointmentDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string AppointmentNumber { get; set; } = default!;
        public Guid PatientId { get; set; }
        public Guid PractitionerId { get; set; }
        public string AppointmentType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Priority { get; set; } = default!;
        public DateTime ScheduledDateTime { get; set; }
        public decimal EstimatedDuration { get; set; }
        public DateTime ActualStartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public string ReasonForVisit { get; set; } = default!;
        public string ChiefComplaint { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public VisitDetailDocument VisitDetails { get; set; } = default!;
        IVisitDetailDocument IAppointmentDocument.VisitDetails => VisitDetails;
        public AppointmentLocationDocument Location { get; set; } = default!;
        IAppointmentLocationDocument IAppointmentDocument.Location => Location;

        public Appointment ToEntity(Appointment? entity = default)
        {
            entity ??= new Appointment();

            entity.Id = Guid.Parse(Id);
            entity.AppointmentNumber = AppointmentNumber ?? throw new Exception($"{nameof(entity.AppointmentNumber)} is null");
            entity.PatientId = PatientId;
            entity.PractitionerId = PractitionerId;
            entity.AppointmentType = AppointmentType ?? throw new Exception($"{nameof(entity.AppointmentType)} is null");
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.Priority = Priority ?? throw new Exception($"{nameof(entity.Priority)} is null");
            entity.ScheduledDateTime = ScheduledDateTime;
            entity.EstimatedDuration = EstimatedDuration;
            entity.ActualStartTime = ActualStartTime;
            entity.ActualEndTime = ActualEndTime;
            entity.ReasonForVisit = ReasonForVisit ?? throw new Exception($"{nameof(entity.ReasonForVisit)} is null");
            entity.ChiefComplaint = ChiefComplaint ?? throw new Exception($"{nameof(entity.ChiefComplaint)} is null");
            entity.Notes = Notes ?? throw new Exception($"{nameof(entity.Notes)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.VisitDetails = VisitDetails.ToEntity() ?? throw new Exception($"{nameof(entity.VisitDetails)} is null");
            entity.Location = Location.ToEntity() ?? throw new Exception($"{nameof(entity.Location)} is null");

            return entity;
        }

        public AppointmentDocument PopulateFromEntity(Appointment entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            AppointmentNumber = entity.AppointmentNumber;
            PatientId = entity.PatientId;
            PractitionerId = entity.PractitionerId;
            AppointmentType = entity.AppointmentType;
            Status = entity.Status;
            Priority = entity.Priority;
            ScheduledDateTime = entity.ScheduledDateTime;
            EstimatedDuration = entity.EstimatedDuration;
            ActualStartTime = entity.ActualStartTime;
            ActualEndTime = entity.ActualEndTime;
            ReasonForVisit = entity.ReasonForVisit;
            ChiefComplaint = entity.ChiefComplaint;
            Notes = entity.Notes;
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            VisitDetails = VisitDetailDocument.FromEntity(entity.VisitDetails)!;
            Location = AppointmentLocationDocument.FromEntity(entity.Location)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static AppointmentDocument? FromEntity(Appointment? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new AppointmentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}