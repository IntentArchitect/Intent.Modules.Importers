using Intent.Modules.CSharp.Importer.Importer;
using Shouldly;
using Xunit;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Edge case and error handling tests
/// </summary>
public class CSharpImporterEdgeCaseTests
{
    [Fact]
    public void MethodData_WithoutParameters_GeneratesCorrectIdentifier()
    {
        // Arrange
        var method = new MethodData
        {
            Name = "Save",
            IsAsync = false,
            Parameters = Array.Empty<ParameterData>(),
            ReturnType = "void",
            GenericParameters = Array.Empty<string>(),
            IsNullable = false,
            ReturnsCollection = false
        };

        // Act
        var identifier = method.GetIdentifier();

        // Assert
        identifier.ShouldBe("Save()");
    }

    [Fact]
    public void MethodData_WithMultipleParameters_GeneratesCorrectIdentifier()
    {
        // Arrange
        var method = new MethodData
        {
            Name = "Update",
            IsAsync = false,
            Parameters = new[]
            {
                new ParameterData { Name = "id", Type = "int" },
                new ParameterData { Name = "name", Type = "string" }
            },
            ReturnType = "void",
            GenericParameters = Array.Empty<string>(),
            IsNullable = false,
            ReturnsCollection = false
        };

        // Act
        var identifier = method.GetIdentifier();

        // Assert
        identifier.ShouldBe("Update(int,string)");
    }

    [Fact]
    public void EnumData_WithNoLiterals_IsCreatable()
    {
        // Arrange & Act
        var enumData = new EnumData
        {
            Namespace = "MyApp.Models",
            Name = "EmptyEnum",
            Literals = Array.Empty<EnumLiteralData>()
        };

        // Assert
        enumData.Literals.Count.ShouldBe(0);
        enumData.GetIdentifier().ShouldBe("MyApp.Models.EmptyEnum");
    }

    [Fact]
    public void ClassData_WithoutProperties_IsCreatable()
    {
        // Arrange & Act
        var classData = new ClassData
        {
            Namespace = "MyApp",
            Name = "EmptyClass",
            Properties = Array.Empty<PropertyData>(),
            Methods = Array.Empty<MethodData>(),
            Interfaces = Array.Empty<string>(),
            Attributes = Array.Empty<string>(),
            Constructors = Array.Empty<ConstructorData>()
        };

        // Assert
        classData.Properties.Count.ShouldBe(0);
        classData.Methods.Count.ShouldBe(0);
    }

    [Fact]
    public void PropertyData_WithNullableAndCollection_HasBothFlags()
    {
        // Arrange & Act
        var property = new PropertyData
        {
            Name = "Items",
            Type = "List<string>",
            IsNullable = true,
            IsCollection = true,
            Attributes = new List<string>()
        };

        // Assert
        property.IsNullable.ShouldBeTrue();
        property.IsCollection.ShouldBeTrue();
    }
}
