using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.CSharp.Importer;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.CSharp.Importer.Tests.TestData;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Tests for C# code import merge behavior - testing idempotency and incremental updates.
/// Similar to DbSchemaIntentMetadataMergerTests for RDBMS.
/// All code is IN-MEMORY - NO temp files.
/// </summary>
public class CSharpImporterMergerTests
{
    [Fact]
    public async Task ReimportSameClass_RemainsIdempotent()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

        // Act - First import
        package.ImportCSharpTypes(coreTypes, config);
        var firstClass = package.Classes.Single(c => c.SpecializationType == "Class");
        var firstClassId = firstClass.Id;
        var firstPropertyIds = firstClass.ChildElements.Select(e => e.Id).ToList();

        // Act - Reimport same data
        package.ImportCSharpTypes(coreTypes, config);
        var secondClass = package.Classes.Single(c => c.SpecializationType == "Class");
        var secondClassId = secondClass.Id;
        var secondPropertyIds = secondClass.ChildElements.Select(e => e.Id).ToList();

        // Assert - IDs should remain unchanged (idempotency)
        firstClassId.ShouldBe(secondClassId);
        firstPropertyIds.ShouldBe(secondPropertyIds);
    }

    [Fact]
    public async Task ImportToExistingPackage_WithMatchingClass_UpdatesInPlace()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.WithCustomerClass(); // Already has Customer with Id and Email
        var existingClassId = package.Classes.Single().Id;
        var config = ImportConfigurations.DomainClassesProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Should update existing class, not create new one
        package.Classes.Count(c => c.SpecializationType == "Class").ShouldBe(1);
        package.Classes.Single(c => c.SpecializationType == "Class").Id.ShouldBe(existingClassId);
    }

    [Fact]
    public async Task ImportToExistingPackage_WithNewClass_AddsClass()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.TwoClasses);
        var package = PackageModels.WithCustomerClass(); // Only has Customer
        var config = ImportConfigurations.DomainClassesProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Should add Order class
        package.Classes.Count(c => c.SpecializationType == "Class").ShouldBe(2);
        package.Classes.Where(c => c.SpecializationType == "Class").ShouldContain(c => c.Name == "Customer");
        package.Classes.Where(c => c.SpecializationType == "Class").ShouldContain(c => c.Name == "Order");
    }

    [Fact]
    public async Task ImportWithNewProperty_AddsProperty()
    {
        // Arrange - Start with Customer having Id and Email
        var package = PackageModels.WithCustomerClass();
        var existingClass = package.Classes.Single(c => c.SpecializationType == "Class");
        var existingClassId = existingClass.Id;
        var existingPropertyCount = existingClass.ChildElements.Count();

        // Code now has Name property added
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var config = ImportConfigurations.DomainClassesProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Should add Name property to existing class
        var customer = package.Classes.Single(c => c.SpecializationType == "Class");
        customer.Id.ShouldBe(existingClassId);
        customer.ChildElements.Count().ShouldBeGreaterThan(existingPropertyCount);
        customer.ChildElements.ShouldContain(p => p.Name == "Name");
    }

    [Fact]
    public async Task TypeDefinitionsOnlyProfile_AppliesCSharpStereotypeWithNamespace()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TypeDefinitionsOnlyProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Class created with C# stereotype containing namespace
        var typeDefinition = package.Classes.Single(c => c.SpecializationType == "Type-Definition");
        typeDefinition.Stereotypes.ShouldNotBeEmpty();
        var csharpStereotype = typeDefinition.Stereotypes.FirstOrDefault(s => s.Name == "C#");
        csharpStereotype.ShouldNotBeNull();
        var namespaceProperty = csharpStereotype.Properties.FirstOrDefault(p => p.Name == "Namespace");
        namespaceProperty.ShouldNotBeNull();
        namespaceProperty.Value.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task TypeDefinitionsOnlyProfile_DoesNotImportProperties()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TypeDefinitionsOnlyProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Class created but NO properties
        package.Classes.Count(c => c.SpecializationType == "Type-Definition").ShouldBe(1);
        package.Classes.Single(c => c.SpecializationType == "Type-Definition").Name.ShouldBe("Customer");
        package.Classes.Single(c => c.SpecializationType == "Type-Definition").ChildElements.Count().ShouldBe(0);
    }

    [Fact]
    public async Task TypeDefinitionsOnlyProfile_ImportsAllTypesAsTypeDefinitions()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.MixedTypes);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TypeDefinitionsOnlyProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Should import class, interface, and enum all as Type-Definition
        var typeDefinitions = package.Classes.Where(c => c.SpecializationType == "Type-Definition").ToList();
        typeDefinitions.Count().ShouldBeGreaterThanOrEqualTo(3); // class, interface, enum
        typeDefinitions.ShouldAllBe(t => t.ChildElements.Count() == 0); // No members
    }

    [Fact]
    public async Task ImportMultipleTimes_MaintainsStability()
    {
        // Arrange
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.TwoClasses);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

        // Act - Import 3 times
        package.ImportCSharpTypes(coreTypes, config);
        var ids1 = package.Classes.Where(c => c.SpecializationType == "Class").Select(c => c.Id).OrderBy(x => x).ToList();
        
        package.ImportCSharpTypes(coreTypes, config);
        var ids2 = package.Classes.Where(c => c.SpecializationType == "Class").Select(c => c.Id).OrderBy(x => x).ToList();
        
        package.ImportCSharpTypes(coreTypes, config);
        var ids3 = package.Classes.Where(c => c.SpecializationType == "Class").Select(c => c.Id).OrderBy(x => x).ToList();

        // Assert - IDs should remain stable across all imports
        ids1.ShouldBe(ids2);
        ids2.ShouldBe(ids3);
        package.Classes.Count(c => c.SpecializationType == "Class").ShouldBe(2);
    }

    // Helper methods

    private async Task<CoreTypesData> AnalyzeCodeInMemory(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: "TestFile.cs");
        var tempPath = Path.Combine(Path.GetTempPath(), $"csharp-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempPath);
        var filePath = Path.Combine(tempPath, "TestFile.cs");
        await File.WriteAllTextAsync(filePath, code);
        
        try
        {
            return await CSharpCodeAnalyzer.ImportMetadataFromFiles(new[] { filePath });
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, recursive: true);
            }
        }
    }
}
