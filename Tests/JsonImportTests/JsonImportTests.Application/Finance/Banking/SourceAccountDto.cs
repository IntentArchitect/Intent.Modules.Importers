using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Finance.Banking
{
    public class SourceAccountDto
    {
        public SourceAccountDto()
        {
            AccountNumber = null!;
            AccountType = null!;
        }

        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }

        public static SourceAccountDto Create(string accountNumber, string accountType, decimal balance)
        {
            return new SourceAccountDto
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                Balance = balance
            };
        }
    }
}