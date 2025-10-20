using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Finance.Banking
{
    public class DestinationAccountDto
    {
        public DestinationAccountDto()
        {
            AccountNumber = null!;
            AccountType = null!;
        }

        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }

        public static DestinationAccountDto Create(string accountNumber, string accountType, decimal balance)
        {
            return new DestinationAccountDto
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                Balance = balance
            };
        }
    }
}