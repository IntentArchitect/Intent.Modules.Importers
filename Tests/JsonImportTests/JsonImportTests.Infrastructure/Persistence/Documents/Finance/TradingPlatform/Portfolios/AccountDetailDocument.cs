using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class AccountDetailDocument : IAccountDetailDocument
    {
        public string AccountNumber { get; set; } = default!;
        public string AccountType { get; set; } = default!;
        public string TaxStatus { get; set; } = default!;
        public string BaseCurrency { get; set; } = default!;
        public DateTime OpenDate { get; set; }
        public string Status { get; set; } = default!;

        public AccountDetail ToEntity(AccountDetail? entity = default)
        {
            entity ??= new AccountDetail();

            entity.AccountNumber = AccountNumber ?? throw new Exception($"{nameof(entity.AccountNumber)} is null");
            entity.AccountType = AccountType ?? throw new Exception($"{nameof(entity.AccountType)} is null");
            entity.TaxStatus = TaxStatus ?? throw new Exception($"{nameof(entity.TaxStatus)} is null");
            entity.BaseCurrency = BaseCurrency ?? throw new Exception($"{nameof(entity.BaseCurrency)} is null");
            entity.OpenDate = OpenDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");

            return entity;
        }

        public AccountDetailDocument PopulateFromEntity(AccountDetail entity)
        {
            AccountNumber = entity.AccountNumber;
            AccountType = entity.AccountType;
            TaxStatus = entity.TaxStatus;
            BaseCurrency = entity.BaseCurrency;
            OpenDate = entity.OpenDate;
            Status = entity.Status;

            return this;
        }

        public static AccountDetailDocument? FromEntity(AccountDetail? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AccountDetailDocument().PopulateFromEntity(entity);
        }
    }
}