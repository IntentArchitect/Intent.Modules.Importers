using Intent.Modules.CSharp.Importer.Importer;
using Shouldly;
using Xunit;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Tests for C# importer configuration and data models
/// </summary>
public class CSharpImporterConfigTests
{
    [Fact]
    public void CSharpConfig_CanBeCreated_WithImportProfile()
    {
        // Arrange
        var profile = new ImportProfileConfig
        {
            Identifier = "test-profile"
        };

        // Act
        var config = new CSharpConfig
        {
            ImportProfile = profile,
            TargetFolderId = null
        };

        // Assert
        config.ImportProfile.Identifier.ShouldBe("test-profile");
    }

    [Fact]
    public void ImportProfileConfig_HasMappingProperties()
    {
        // Act
        var profile = new ImportProfileConfig
        {
            Identifier = "domain-classes",
            MapClassesTo = null,
            MapPropertiesTo = null,
            MapEnumsTo = null
        };

        // Assert
        profile.Identifier.ShouldBe("domain-classes");
    }

    [Fact]
    public void CoreTypesData_CanHoldMultipleTypes()
    {
        // Arrange
        var coreTypes = new CoreTypesData();

        // Act
        var enumData = new EnumData
        {
            Name = "Status",
            Namespace = "TestNamespace",
            FilePath = "/Test/Status.cs",
            Literals = new[] { 
                new EnumLiteralData { Name = "Active", Value = "0" }
            }
        };
        
        ((List<EnumData>)coreTypes.Enums).Add(enumData);

        // Assert
        coreTypes.Enums.Count.ShouldBe(1);
        coreTypes.Enums[0].Name.ShouldBe("Status");
    }
}
