using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace TestDataGenerator.Domain.Enums
{
    public enum ProductType
    {
        Personal = 1,
        Commercial = 2
    }
}