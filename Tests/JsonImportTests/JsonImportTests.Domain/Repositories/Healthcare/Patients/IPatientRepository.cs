using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Healthcare.Patients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPatientRepository : ICosmosDBRepository<Patient, IPatientDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Patient?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Patient>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}