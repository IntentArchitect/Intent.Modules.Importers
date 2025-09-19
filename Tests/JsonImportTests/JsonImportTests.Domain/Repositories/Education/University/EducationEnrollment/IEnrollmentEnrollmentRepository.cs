using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Education.University.EducationEnrollment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEnrollmentEnrollmentRepository : ICosmosDBRepository<enrollmentEnrollment, IenrollmentEnrollmentDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<enrollmentEnrollment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<enrollmentEnrollment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}