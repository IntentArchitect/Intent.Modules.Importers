using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class SafetyDocument : ISafetyDocument
    {
        public decimal AirbagsCount { get; set; }
        public bool HasABS { get; set; }
        public bool HasESC { get; set; }
        public decimal CrashRating { get; set; }
        public List<string> OptionalSafetyFeatures { get; set; } = default!;
        IReadOnlyList<string> ISafetyDocument.OptionalSafetyFeatures => OptionalSafetyFeatures;

        public Safety ToEntity(Safety? entity = default)
        {
            entity ??= new Safety();

            entity.AirbagsCount = AirbagsCount;
            entity.HasABS = HasABS;
            entity.HasESC = HasESC;
            entity.CrashRating = CrashRating;
            entity.OptionalSafetyFeatures = OptionalSafetyFeatures ?? throw new Exception($"{nameof(entity.OptionalSafetyFeatures)} is null");

            return entity;
        }

        public SafetyDocument PopulateFromEntity(Safety entity)
        {
            AirbagsCount = entity.AirbagsCount;
            HasABS = entity.HasABS;
            HasESC = entity.HasESC;
            CrashRating = entity.CrashRating;
            OptionalSafetyFeatures = entity.OptionalSafetyFeatures.ToList();

            return this;
        }

        public static SafetyDocument? FromEntity(Safety? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SafetyDocument().PopulateFromEntity(entity);
        }
    }
}