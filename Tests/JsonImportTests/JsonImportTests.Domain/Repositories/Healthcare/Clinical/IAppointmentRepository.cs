using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Healthcare.Clinical
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAppointmentRepository : ICosmosDBRepository<Appointment, IAppointmentDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Appointment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Appointment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}