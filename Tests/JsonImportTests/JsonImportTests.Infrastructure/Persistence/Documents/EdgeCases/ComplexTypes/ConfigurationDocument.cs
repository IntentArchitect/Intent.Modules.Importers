using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class ConfigurationDocument : IConfigurationDocument
    {
        public SettingDocument Settings { get; set; } = default!;
        ISettingDocument IConfigurationDocument.Settings => Settings;
        public List<PreferenceDocument> Preferences { get; set; } = default!;
        IReadOnlyList<IPreferenceDocument> IConfigurationDocument.Preferences => Preferences;

        public Domain.Entities.EdgeCases.ComplexTypes.Configuration ToEntity(Domain.Entities.EdgeCases.ComplexTypes.Configuration? entity = default)
        {
            entity ??= new Domain.Entities.EdgeCases.ComplexTypes.Configuration();
            entity.Settings = Settings.ToEntity() ?? throw new Exception($"{nameof(entity.Settings)} is null");
            entity.Preferences = Preferences.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ConfigurationDocument PopulateFromEntity(Domain.Entities.EdgeCases.ComplexTypes.Configuration entity)
        {
            Settings = SettingDocument.FromEntity(entity.Settings)!;
            Preferences = entity.Preferences.Select(x => PreferenceDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ConfigurationDocument? FromEntity(Domain.Entities.EdgeCases.ComplexTypes.Configuration? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ConfigurationDocument().PopulateFromEntity(entity);
        }
    }
}