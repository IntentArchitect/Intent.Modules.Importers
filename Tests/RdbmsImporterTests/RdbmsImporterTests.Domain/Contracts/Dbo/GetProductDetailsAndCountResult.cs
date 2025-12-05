using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Dbo
{
    public record GetProductDetailsAndCountResult
    {
        public GetProductDetailsAndCountResult(IEnumerable<GetProductDetailsAndCountResponse> results, int? priceCount)
        {
            Results = results;
            PriceCount = priceCount;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected GetProductDetailsAndCountResult()
        {
            Results = null!;
        }

        public IEnumerable<GetProductDetailsAndCountResponse> Results { get; init; }
        public int? PriceCount { get; init; }
    }
}