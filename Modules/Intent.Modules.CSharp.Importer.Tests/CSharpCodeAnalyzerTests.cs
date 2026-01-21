using System.Threading.Tasks;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.CSharp.Importer.Tests.TestData;
using Shouldly;
using Xunit;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Test suite for C# code type extraction
/// </summary>
public class CSharpCodeAnalyzerTests
{
    [Fact]
    public void CoreTypesData_CanBeInstantiated_WithCollections()
    {
        // Act
        var coreTypes = new CoreTypesData();
        
        // Assert
        coreTypes.Classes.ShouldNotBeNull();
        coreTypes.Interfaces.ShouldNotBeNull();
        coreTypes.Enums.ShouldNotBeNull();
    }

    [Fact]
    public async Task ImportMetadataFromFiles_SimpleClass_ExtractsClass()
    {
        // Arrange
        var scenario = CodeScenarioComposer.FromCode(TestCodeFiles.SimpleClass);
        
        // Act
        var result = await CSharpCodeAnalyzer.ImportMetadataFromFiles(scenario.CodeFiles);
        
        // Assert
        result.Classes.Count.ShouldBeGreaterThan(0);
        var simpleEntity = result.Classes.First(c => c.Name == "SimpleEntity");
        simpleEntity.Properties.Count.ShouldBeGreaterThan(0);
        
        // Cleanup
        CodeScenarioComposer.CleanupScenario(scenario);
    }

    [Fact]
    public async Task ImportMetadataFromFiles_StaticClass_IsNotExtracted()
    {
        // Arrange
        var scenario = CodeScenarioComposer.FromCode(TestCodeFiles.StaticClass);
        
        // Act
        var result = await CSharpCodeAnalyzer.ImportMetadataFromFiles(scenario.CodeFiles);
        
        // Assert
        result.Classes.ShouldBeEmpty("Static classes should not be extracted");
        
        // Cleanup
        CodeScenarioComposer.CleanupScenario(scenario);
    }

    [Fact]
    public async Task ImportMetadataFromFiles_Enum_ExtractsEnumWithLiterals()
    {
        // Arrange
        var scenario = CodeScenarioComposer.FromCode(TestCodeFiles.Enum);
        
        // Act
        var result = await CSharpCodeAnalyzer.ImportMetadataFromFiles(scenario.CodeFiles);
        
        // Assert
        result.Enums.ShouldHaveSingleItem();
        var productType = result.Enums.Single();
        productType.Name.ShouldBe("ProductType");
        productType.Literals.Count.ShouldBe(3);
        
        // Cleanup
        CodeScenarioComposer.CleanupScenario(scenario);
    }

    [Fact]
    public async Task ImportMetadataFromFiles_Interface_ExtractsInterface()
    {
        // Arrange
        var scenario = CodeScenarioComposer.FromCode(TestCodeFiles.InterfaceWithMethods);
        
        // Act
        var result = await CSharpCodeAnalyzer.ImportMetadataFromFiles(scenario.CodeFiles);
        
        // Assert
        result.Interfaces.ShouldNotBeEmpty();
        var service = result.Interfaces.First(i => i.Name == "IOrderService");
        service.Methods.Count.ShouldBeGreaterThan(0);
        
        // Cleanup
        CodeScenarioComposer.CleanupScenario(scenario);
    }

    [Fact]
    public async Task ImportMetadataFromFiles_EmptyArray_ReturnsEmpty()
    {
        // Act
        var result = await CSharpCodeAnalyzer.ImportMetadataFromFiles(Array.Empty<string>());
        
        // Assert
        result.Classes.Count.ShouldBe(0);
        result.Interfaces.Count.ShouldBe(0);
        result.Enums.Count.ShouldBe(0);
    }
}
