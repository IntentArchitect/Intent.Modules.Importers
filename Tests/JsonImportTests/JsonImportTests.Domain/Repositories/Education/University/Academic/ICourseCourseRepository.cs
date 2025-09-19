using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Education.University.Academic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICourseCourseRepository : ICosmosDBRepository<courseCourse, IcourseCourseDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<courseCourse?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<courseCourse>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}