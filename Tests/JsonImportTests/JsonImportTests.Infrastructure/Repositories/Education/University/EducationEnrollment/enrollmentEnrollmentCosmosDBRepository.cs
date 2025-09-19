using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Education.University.EducationEnrollment;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.Education.University.EducationEnrollment
{
    internal class enrollmentEnrollmentCosmosDBRepository : CosmosDBRepositoryBase<enrollmentEnrollment, enrollmentEnrollmentDocument, IenrollmentEnrollmentDocument>, IEnrollmentEnrollmentRepository
    {
        public enrollmentEnrollmentCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<enrollmentEnrollmentDocument> cosmosRepository,
            ICosmosContainerProvider<enrollmentEnrollmentDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<enrollmentEnrollment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<enrollmentEnrollment>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}