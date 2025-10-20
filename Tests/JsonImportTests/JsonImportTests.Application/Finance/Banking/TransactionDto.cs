using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Finance.Banking
{
    public class TransactionDto
    {
        public TransactionDto()
        {
            TransactionType = null!;
            Currency = null!;
            Status = null!;
            Description = null!;
            SourceAccount = null!;
            DestinationAccount = null!;
            Metadata = null!;
        }

        public Guid TransactionId { get; set; }
        public Guid AccountId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public SourceAccountDto SourceAccount { get; set; }
        public DestinationAccountDto DestinationAccount { get; set; }
        public MetadataDto Metadata { get; set; }

        public static TransactionDto Create(
            Guid transactionId,
            Guid accountId,
            string transactionType,
            decimal amount,
            string currency,
            DateTime transactionDate,
            string status,
            string description,
            SourceAccountDto sourceAccount,
            DestinationAccountDto destinationAccount,
            MetadataDto metadata)
        {
            return new TransactionDto
            {
                TransactionId = transactionId,
                AccountId = accountId,
                TransactionType = transactionType,
                Amount = amount,
                Currency = currency,
                TransactionDate = transactionDate,
                Status = status,
                Description = description,
                SourceAccount = sourceAccount,
                DestinationAccount = destinationAccount,
                Metadata = metadata
            };
        }
    }
}