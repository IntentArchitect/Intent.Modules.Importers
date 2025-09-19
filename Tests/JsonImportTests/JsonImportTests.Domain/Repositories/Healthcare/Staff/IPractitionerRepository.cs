using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Healthcare.Staff
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPractitionerRepository : ICosmosDBRepository<Practitioner, IPractitionerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Practitioner?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Practitioner>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}