using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.EdgeCases.ComplexTypes;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.EdgeCases.ComplexTypes
{
    internal class ComplexEntityCosmosDBRepository : CosmosDBRepositoryBase<ComplexEntity, ComplexEntityDocument, IComplexEntityDocument>, IComplexEntityRepository
    {
        public ComplexEntityCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<ComplexEntityDocument> cosmosRepository,
            ICosmosContainerProvider<ComplexEntityDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<ComplexEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<ComplexEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}