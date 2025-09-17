using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories;
using JsonImportTests.Domain.Repositories.Documents;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories
{
    internal class invoiceCosmosDBRepository : CosmosDBRepositoryBase<invoice, invoiceDocument, IinvoiceDocument>, IInvoiceRepository
    {
        public invoiceCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<invoiceDocument> cosmosRepository,
            ICosmosContainerProvider<invoiceDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<invoice>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}