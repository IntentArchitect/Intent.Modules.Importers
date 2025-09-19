using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class WorkScheduleDocument : IWorkScheduleDocument
    {
        public string ShiftType { get; set; } = default!;
        public decimal WeeklyHours { get; set; }
        public string StartTime { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public List<string> WorkDays { get; set; } = default!;
        IReadOnlyList<string> IWorkScheduleDocument.WorkDays => WorkDays;

        public WorkSchedule ToEntity(WorkSchedule? entity = default)
        {
            entity ??= new WorkSchedule();

            entity.ShiftType = ShiftType ?? throw new Exception($"{nameof(entity.ShiftType)} is null");
            entity.WeeklyHours = WeeklyHours;
            entity.StartTime = StartTime ?? throw new Exception($"{nameof(entity.StartTime)} is null");
            entity.EndTime = EndTime ?? throw new Exception($"{nameof(entity.EndTime)} is null");
            entity.WorkDays = WorkDays ?? throw new Exception($"{nameof(entity.WorkDays)} is null");

            return entity;
        }

        public WorkScheduleDocument PopulateFromEntity(WorkSchedule entity)
        {
            ShiftType = entity.ShiftType;
            WeeklyHours = entity.WeeklyHours;
            StartTime = entity.StartTime;
            EndTime = entity.EndTime;
            WorkDays = entity.WorkDays.ToList();

            return this;
        }

        public static WorkScheduleDocument? FromEntity(WorkSchedule? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new WorkScheduleDocument().PopulateFromEntity(entity);
        }
    }
}