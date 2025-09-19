using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Education.University.Academic;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.Education.University.Academic
{
    internal class courseCourseCosmosDBRepository : CosmosDBRepositoryBase<courseCourse, courseCourseDocument, IcourseCourseDocument>, ICourseCourseRepository
    {
        public courseCourseCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<courseCourseDocument> cosmosRepository,
            ICosmosContainerProvider<courseCourseDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<courseCourse?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<courseCourse>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}