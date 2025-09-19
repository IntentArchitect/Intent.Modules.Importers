using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class ScheduleDocument : IScheduleDocument
    {
        public List<MeetingTimeDocument> MeetingTimes { get; set; } = default!;
        IReadOnlyList<IMeetingTimeDocument> IScheduleDocument.MeetingTimes => MeetingTimes;
        public ExamScheduleDocument ExamSchedule { get; set; } = default!;
        IExamScheduleDocument IScheduleDocument.ExamSchedule => ExamSchedule;

        public Schedule ToEntity(Schedule? entity = default)
        {
            entity ??= new Schedule();
            entity.MeetingTimes = MeetingTimes.Select(x => x.ToEntity()).ToList();
            entity.ExamSchedule = ExamSchedule.ToEntity() ?? throw new Exception($"{nameof(entity.ExamSchedule)} is null");

            return entity;
        }

        public ScheduleDocument PopulateFromEntity(Schedule entity)
        {
            MeetingTimes = entity.MeetingTimes.Select(x => MeetingTimeDocument.FromEntity(x)!).ToList();
            ExamSchedule = ExamScheduleDocument.FromEntity(entity.ExamSchedule)!;

            return this;
        }

        public static ScheduleDocument? FromEntity(Schedule? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ScheduleDocument().PopulateFromEntity(entity);
        }
    }
}