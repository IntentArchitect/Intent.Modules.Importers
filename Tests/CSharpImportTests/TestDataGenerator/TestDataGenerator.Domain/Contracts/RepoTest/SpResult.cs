using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace TestDataGenerator.Domain.Contracts.RepoTest
{
    public record SpResult
    {
        public SpResult(string data)
        {
            Data = data;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected SpResult()
        {
            Data = null!;
        }

        public string Data { get; init; }
    }
}