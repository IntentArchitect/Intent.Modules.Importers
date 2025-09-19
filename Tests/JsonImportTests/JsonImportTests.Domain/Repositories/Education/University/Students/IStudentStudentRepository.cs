using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Education.University.Students
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStudentStudentRepository : ICosmosDBRepository<studentStudent, IstudentStudentDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<studentStudent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<studentStudent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}