using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.CSharp.Importer;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.CSharp.Importer.Tests.TestData;
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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.TwoClasses);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassWithMethods);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainInterfacesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleInterface);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainInterfacesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleEnum);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainEnumsProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassWithInheritance);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.ClassImplementingInterface);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.Record);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.SimpleClass);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TypeDefinitionsOnlyProfile();

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
        var coreTypes = await AnalyzeCodeInMemory(CSharpCodeSamples.MixedTypes);
        var package = PackageModels.Empty();
        var config = ImportConfigurations.DomainClassesProfile();

        // Act
        package.ImportCSharpTypes(coreTypes, config);

        // Assert
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("mixed-types");
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
