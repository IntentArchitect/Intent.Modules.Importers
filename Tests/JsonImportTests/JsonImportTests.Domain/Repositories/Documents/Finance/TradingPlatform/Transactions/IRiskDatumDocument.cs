using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IRiskDatumDocument
    {
        decimal VaRImpact { get; }
        decimal DeltaEquivalent { get; }
        decimal NotionalAmount { get; }
        string RiskClass { get; }
        decimal ConcentrationLimit { get; }
        decimal PositionLimit { get; }
    }
}