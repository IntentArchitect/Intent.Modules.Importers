using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.ECommerce.Catalog.Categories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICategoryCategoryRepository : ICosmosDBRepository<categoryCategory, IcategoryCategoryDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<categoryCategory?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<categoryCategory>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}