using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace TestDataGenerator.Application.Products
{
    public enum ServiceProductEnum
    {
        Commercial = 2,
        Personal = 1
    }
}