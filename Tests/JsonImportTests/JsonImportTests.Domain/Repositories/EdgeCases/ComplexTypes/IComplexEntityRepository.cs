using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.EdgeCases.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IComplexEntityRepository : ICosmosDBRepository<ComplexEntity, IComplexEntityDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<ComplexEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ComplexEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}