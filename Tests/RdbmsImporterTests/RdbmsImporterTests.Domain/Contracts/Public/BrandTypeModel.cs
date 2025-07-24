using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record BrandTypeModel
    {
        public BrandTypeModel(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected BrandTypeModel()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public bool IsActive { get; init; }
    }
}