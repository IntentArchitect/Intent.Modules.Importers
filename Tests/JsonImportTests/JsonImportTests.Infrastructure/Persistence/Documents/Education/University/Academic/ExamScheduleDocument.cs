using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class ExamScheduleDocument : IExamScheduleDocument
    {
        public DateTime MidtermDate { get; set; }
        public DateTime FinalExamDate { get; set; }
        public string Location { get; set; } = default!;

        public ExamSchedule ToEntity(ExamSchedule? entity = default)
        {
            entity ??= new ExamSchedule();

            entity.MidtermDate = MidtermDate;
            entity.FinalExamDate = FinalExamDate;
            entity.Location = Location ?? throw new Exception($"{nameof(entity.Location)} is null");

            return entity;
        }

        public ExamScheduleDocument PopulateFromEntity(ExamSchedule entity)
        {
            MidtermDate = entity.MidtermDate;
            FinalExamDate = entity.FinalExamDate;
            Location = entity.Location;

            return this;
        }

        public static ExamScheduleDocument? FromEntity(ExamSchedule? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ExamScheduleDocument().PopulateFromEntity(entity);
        }
    }
}