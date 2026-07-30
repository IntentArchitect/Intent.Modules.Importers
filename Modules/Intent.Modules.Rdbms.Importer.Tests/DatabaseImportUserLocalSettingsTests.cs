using System.Text.Json;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

public class DatabaseImportUserLocalSettingsTests
{
    [Fact]
    public void UserLocalSettings_SerializedForStorageThenRead_RoundTripsEveryValue()
    {
        // Arrange
        var settings = new DatabaseImportLocalSettings
        {
            EntityNameConvention = "SingularEntity",
            AttributeNameConvention = "Default",
            TableStereotypes = "WhenDifferent",
            TypesToExport = "Table;View;StoredProcedure",
            ImportFilterFilePath = "db-import-filter.json",
            ConnectionString = "Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;",
            StoredProcedureType = "RepositoryOperationMapping",
            SettingPersistence = "UserLocal",
            DatabaseType = "SqlServer",
            FilterType = "include",
            AllowDeletions = "true",
            PreserveAttributeTypes = "true"
        };

        // Act
        var json = JsonSerializer.Serialize(settings, SerializationHelper.IndentedSerializerOptions);
        var reloaded = JsonSerializer.Deserialize<DatabaseImportLocalSettings>(json, SerializationHelper.SerializerOptions);

        // Assert
        reloaded.ShouldNotBeNull();
        reloaded.EntityNameConvention.ShouldBe(settings.EntityNameConvention);
        reloaded.AttributeNameConvention.ShouldBe(settings.AttributeNameConvention);
        reloaded.TableStereotypes.ShouldBe(settings.TableStereotypes);
        reloaded.TypesToExport.ShouldBe(settings.TypesToExport);
        reloaded.ImportFilterFilePath.ShouldBe(settings.ImportFilterFilePath);
        reloaded.ConnectionString.ShouldBe(settings.ConnectionString);
        reloaded.StoredProcedureType.ShouldBe(settings.StoredProcedureType);
        reloaded.SettingPersistence.ShouldBe(settings.SettingPersistence);
        reloaded.DatabaseType.ShouldBe(settings.DatabaseType);
        reloaded.FilterType.ShouldBe(settings.FilterType);
        reloaded.AllowDeletions.ShouldBe(settings.AllowDeletions);
        reloaded.PreserveAttributeTypes.ShouldBe(settings.PreserveAttributeTypes);
    }

    [Fact]
    public void UserLocalSettings_PascalCasedLegacyFileContent_StillDeserializes()
    {
        // Settings files written by earlier versions of the module used the default (PascalCase)
        // naming policy, so reads must remain backwards compatible.
        const string legacyJson = """
                                  {
                                    "EntityNameConvention": "SingularEntity",
                                    "AttributeNameConvention": "Default",
                                    "TableStereotypes": "WhenDifferent",
                                    "TypesToExport": "Table;View;StoredProcedure",
                                    "ImportFilterFilePath": "db-import-filter.json",
                                    "ConnectionString": "Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;",
                                    "StoredProcedureType": "RepositoryOperationMapping",
                                    "SettingPersistence": "UserLocal",
                                    "DatabaseType": "SqlServer",
                                    "FilterType": "include",
                                    "AllowDeletions": "true",
                                    "PreserveAttributeTypes": "true"
                                  }
                                  """;

        // Act
        var settings = JsonSerializer.Deserialize<DatabaseImportLocalSettings>(legacyJson, SerializationHelper.SerializerOptions);

        // Assert
        settings.ShouldNotBeNull();
        settings.ConnectionString.ShouldBe("Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;");
        settings.StoredProcedureType.ShouldBe("RepositoryOperationMapping");
        settings.SettingPersistence.ShouldBe("UserLocal");
        settings.TypesToExport.ShouldBe("Table;View;StoredProcedure");
    }

    [Fact]
    public void RepositoryUserLocalSettings_SerializedForStorageThenRead_RoundTripsEveryValue()
    {
        // Arrange
        var settings = new RepositoryImportLocalSettings
        {
            ConnectionString = "Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;",
            DatabaseType = "SqlServer",
            StoredProcedureType = "RepositoryOperationMapping",
            SettingPersistence = "UserLocal"
        };

        // Act
        var json = JsonSerializer.Serialize(settings, SerializationHelper.IndentedSerializerOptions);
        var reloaded = JsonSerializer.Deserialize<RepositoryImportLocalSettings>(json, SerializationHelper.SerializerOptions);

        // Assert
        reloaded.ShouldNotBeNull();
        reloaded.ConnectionString.ShouldBe(settings.ConnectionString);
        reloaded.DatabaseType.ShouldBe(settings.DatabaseType);
        reloaded.StoredProcedureType.ShouldBe(settings.StoredProcedureType);
        reloaded.SettingPersistence.ShouldBe(settings.SettingPersistence);
    }

    [Fact]
    public void RepositoryUserLocalSettings_PascalCasedLegacyFileContent_StillDeserializes()
    {
        // Settings files written by earlier versions of the module used the default (PascalCase)
        // naming policy, so reads must remain backwards compatible.
        const string legacyJson = """
                                  {
                                    "ConnectionString": "Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;",
                                    "DatabaseType": "SqlServer",
                                    "StoredProcedureType": "RepositoryOperationMapping",
                                    "SettingPersistence": "UserLocal"
                                  }
                                  """;

        // Act
        var settings = JsonSerializer.Deserialize<RepositoryImportLocalSettings>(legacyJson, SerializationHelper.SerializerOptions);

        // Assert
        settings.ShouldNotBeNull();
        settings.ConnectionString.ShouldBe("Data Source=localhost;Database=ImportDemo;Integrated Security=sspi;TrustServerCertificate=true;");
        settings.DatabaseType.ShouldBe("SqlServer");
        settings.StoredProcedureType.ShouldBe("RepositoryOperationMapping");
        settings.SettingPersistence.ShouldBe("UserLocal");
    }

    [Fact]
    public void RepositoryUserLocalSettings_PartiallyWrittenFileContent_LeavesMissingValuesNull()
    {
        // A truncated or hand-edited settings file must degrade to defaults rather than throw.
        const string partialJson = """
                                   {
                                     "connectionString": "Data Source=localhost;Database=ImportDemo;"
                                   }
                                   """;

        // Act
        var settings = JsonSerializer.Deserialize<RepositoryImportLocalSettings>(partialJson, SerializationHelper.SerializerOptions);

        // Assert
        settings.ShouldNotBeNull();
        settings.ConnectionString.ShouldBe("Data Source=localhost;Database=ImportDemo;");
        settings.DatabaseType.ShouldBeNull();
        settings.StoredProcedureType.ShouldBeNull();
        settings.SettingPersistence.ShouldBeNull();
    }

    [Theory]
    [InlineData(RepositorySettingPersistence.All, RepositorySettingPersistence.SharedMetadata)]
    [InlineData(RepositorySettingPersistence.AllSanitisedConnectionString, RepositorySettingPersistence.SharedMetadataSanitisedConnectionString)]
    [InlineData(RepositorySettingPersistence.AllWithoutConnectionString, RepositorySettingPersistence.SharedMetadataWithoutConnectionString)]
    public void NormalizeRepositorySettingPersistence_LegacyMetadataValue_MapsToSharedMetadataEquivalent(
        RepositorySettingPersistence legacyValue,
        RepositorySettingPersistence expected)
    {
        SettingsHelper.NormalizeRepositorySettingPersistence(legacyValue).ShouldBe(expected);
    }

    [Theory]
    [InlineData(RepositorySettingPersistence.None)]
    [InlineData(RepositorySettingPersistence.InheritDb)]
    [InlineData(RepositorySettingPersistence.UserLocal)]
    [InlineData(RepositorySettingPersistence.SharedMetadata)]
    [InlineData(RepositorySettingPersistence.SharedMetadataSanitisedConnectionString)]
    [InlineData(RepositorySettingPersistence.SharedMetadataWithoutConnectionString)]
    public void NormalizeRepositorySettingPersistence_CurrentValue_IsUnchanged(RepositorySettingPersistence value)
    {
        SettingsHelper.NormalizeRepositorySettingPersistence(value).ShouldBe(value);
    }
}
