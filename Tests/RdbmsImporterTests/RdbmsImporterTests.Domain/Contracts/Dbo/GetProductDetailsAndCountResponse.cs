using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Dbo
{
    public record GetProductDetailsAndCountResponse
    {
        public GetProductDetailsAndCountResponse(Guid id,
            string name,
            string description,
            bool isActive,
            decimal? currentPrice,
            int? totalPriceRecords)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CurrentPrice = currentPrice;
            TotalPriceRecords = totalPriceRecords;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected GetProductDetailsAndCountResponse()
        {
            Name = null!;
            Description = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsActive { get; init; }
        public decimal? CurrentPrice { get; init; }
        public int? TotalPriceRecords { get; init; }
    }
}