using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace TestDataGenerator.Domain.Contracts.RepoTest
{
    public record SpParameterWithName
    {
        public SpParameterWithName(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected SpParameterWithName()
        {
            Attribute = null!;
        }

        public string Attribute { get; init; }
    }
}