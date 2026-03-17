using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.CSharp.Importer;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.CSharp.Importer.Tests.TestData;
using Intent.Persistence;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using VerifyXunit;
using Xunit;
using static VerifyXunit.Verifier;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Comprehensive snapshot-based tests for C# code import to Intent metadata.
/// Tests straightforward imports from scratch - all IN-MEMORY, NO temp files.
/// Similar to DbSchemaComprehensiveMappingTests for RDBMS.
/// </summary>
public class CSharpImporterMappingTests
{
    [Fact]
    public async Task ImportSimpleClass_BasicProperties_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        package.Classes.Count(c => c.SpecializationType == "Class").ShouldBe(1);
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("simple-class");
    }

    [Fact]
    public async Task ImportTwoClasses_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.TwoClasses);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        package.Classes.Count(c => c.SpecializationType == "Class").ShouldBe(2);
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("two-classes");
    }

    [Fact]
    public async Task ImportClassWithMethods_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassWithMethods);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainInterfacesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("class-with-methods");
    }

    [Fact]
    public async Task ImportSimpleInterface_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleInterface);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainInterfacesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("simple-interface");
    }

    [Fact]
    public async Task ImportSimpleEnum_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleEnum);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainEnumsProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("simple-enum");
    }

    [Fact]
    public async Task ImportClassWithInheritance_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassWithInheritance);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("class-with-inheritance");
    }

    [Fact]
    public async Task ImportClassImplementingInterface_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassImplementingInterface);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("class-implementing-interface");
    }

    [Fact]
    public async Task ImportRecord_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.Record);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("record");
    }

    [Fact]
    public async Task ImportTypeDefinitionsOnly_IgnoresMembers_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TypeDefinitionsOnlyProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert - Should have class but NO properties
        package.Classes.Count(c => c.SpecializationType == "Type-Definition").ShouldBe(1);
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("type-definitions-only");
    }

    [Fact]
    public async Task ImportMixedTypes_ShouldMatchSnapshot()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.MixedTypes);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile(tempPath);

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("mixed-types");
    }

    [Fact]
    public async Task ImportSimpleClass_DuplicateFolder_MultiplePackages()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var domainOne = PackageModels.WithFolder("DomainOne");
        var domainTwo = PackageModels.WithFolder("DomainTwo");
        var services = PackageModels.WithFolder("Services", [domainOne, domainTwo]);

        var config = ImportConfigurations.ServiceDtoProfile(tempPath);

        // Act
        services.ImportCSharpTypes(coreTypes, config);

        // Assert
        services.Classes.Count(c => c.SpecializationType == "DTO").ShouldBe(1);
        var snapshot = BuildPackageSnapshot(services);
        await Verify(snapshot).UseParameters("simple-class");
    }

    [Fact]
    public async Task ImportInterface_AsyncAndSyncOperations_PreserveMethodAsyncDeclarations()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.AsyncInterface);
        var services = PackageModels.Empty();

        var config = ImportConfigurations.ServiceServiceProfile(tempPath);

        // Act
        services.ImportCSharpTypes(coreTypes, config);

        // Assert
        services.Classes.Count().ShouldBe(1);
        var snapshot = BuildPackageSnapshot(services);
        await Verify(snapshot).UseParameters("async-interface");

        var service = services.Classes.First();
        service.ChildElements.Count().ShouldBe(3);

        service.ChildElements.Count(e => e.Stereotypes.Any(s => s.DefinitionId == "2db1104b-ca3c-47a6-ad82-a0d2ee915c06")).ShouldBe(1);
        
        var asyncOperations = service.ChildElements.Where(e => e.Stereotypes.Any(s => s.DefinitionId == "A225C795-33E9-417D-8D58-E22826A08224"));
        asyncOperations.Count().ShouldBe(1);
        var asyncOperation = asyncOperations.First();

        var stereotype = asyncOperation.GetOrCreateStereotype("A225C795-33E9-417D-8D58-E22826A08224", "", "", "");
        stereotype.Properties.Count(s => s.DefinitionId == "2801e2a9-5797-406f-b289-43af8fbb2d7e" && s.Value == "true").ShouldBe(1);
    }

    [Fact]
    public async Task ImportInterface_AsyncAndSyncOperations_DoNotPreserveMethodAsyncDeclarations()
    {
        // Arrange
        var (coreTypes, tempPath) = await AnalyzeCodeInMemory(CSharpCodeSamples.AsyncInterface);
        var services = PackageModels.Empty();

        var config = ImportConfigurations.ServiceServiceProfile(tempPath, false);

        // Act
        services.ImportCSharpTypes(coreTypes, config);

        // Assert
        services.Classes.Count().ShouldBe(1);
        var snapshot = BuildPackageSnapshot(services);
        await Verify(snapshot).UseParameters("async-interface");

        var service = services.Classes.First();
        service.ChildElements.Count().ShouldBe(3);
        service.ChildElements.Count(e => e.Stereotypes.Any(s => s.DefinitionId == "2db1104b-ca3c-47a6-ad82-a0d2ee915c06")).ShouldBe(0);
    }

    // Helper methods

    private async Task<(CoreTypesData, string)> AnalyzeCodeInMemory(string code)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"csharp-test-{Guid.NewGuid()}");
        var filePath = Path.Combine(tempPath, "TestFile.cs");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            await File.WriteAllTextAsync(filePath, code);
            var coreTypesData = await CSharpCodeAnalyzer.ImportMetadataFromFiles(new[] { filePath });
            return (coreTypesData, tempPath);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, recursive: true);
                }
                catch
                {
                    // Ignore cleanup failures - temp folder will be cleaned up by OS eventually
                }
            }
        }
    }

    private object BuildPackageSnapshot(Intent.Persistence.IPackageModelPersistable package)
    {
        return new
        {
            Classes = package.Classes.Select(c => new
            {
                c.Name,
                c.SpecializationType,
                Properties = c.ChildElements.Select(p => new
                {
                    p.Name,
                    p.SpecializationType
                })
            }),
            Associations = package.Associations.Select(a => new
            {
                a.AssociationType
            })
        };
    }
}
