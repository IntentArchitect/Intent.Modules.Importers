using Intent.Modules.CSharp.Importer.Importer;
using Shouldly;
using Xunit;

namespace Intent.Modules.CSharp.Importer.Tests;

/// <summary>
/// Tests for C# data model classes
/// </summary>
public class CSharpDataModelTests
{
    [Fact]
    public void ClassData_CanSetBasicProperties()
    {
        // Arrange & Act
        var classData = new ClassData
        {
            Namespace = "MyApp.Domain",
            Name = "User",
            BaseType = null,
            Interfaces = Array.Empty<string>(),
            Attributes = Array.Empty<string>(),
            Properties = Array.Empty<PropertyData>(),
            Methods = Array.Empty<MethodData>(),
            Constructors = Array.Empty<ConstructorData>()
        };

        // Assert
        classData.Namespace.ShouldBe("MyApp.Domain");
        classData.Name.ShouldBe("User");
        classData.BaseType.ShouldBeNull();
    }

    [Fact]
    public void ClassData_GeneratesIdentifier()
    {
        // Arrange
        var classData = new ClassData
        {
            Namespace = "MyApp",
            Name = "Product",
            Interfaces = Array.Empty<string>(),
            Attributes = Array.Empty<string>(),
            Properties = Array.Empty<PropertyData>(),
            Methods = Array.Empty<MethodData>(),
            Constructors = Array.Empty<ConstructorData>()
        };

        // Act
        var identifier = classData.GetIdentifier();

        // Assert
        identifier.ShouldBe("MyApp.Product");
    }

    [Fact]
    public void EnumData_CanSetProperties()
    {
        // Arrange & Act
        var enumData = new EnumData
        {
            Namespace = "MyApp.Models",
            Name = "OrderStatus",
            Literals = new[] 
            { 
                new EnumLiteralData { Name = "Pending" },
                new EnumLiteralData { Name = "Completed" },
                new EnumLiteralData { Name = "Cancelled" }
            }
        };

        // Assert
        enumData.Name.ShouldBe("OrderStatus");
        enumData.Literals.Count.ShouldBe(3);
    }

    [Fact]
    public void PropertyData_CanSetProperties()
    {
        // Arrange & Act
        var property = new PropertyData
        {
            Name = "Id",
            Type = "int",
            IsNullable = false,
            IsCollection = false,
            Attributes = new List<string>()
        };

        // Assert
        property.Name.ShouldBe("Id");
        property.Type.ShouldBe("int");
        property.IsNullable.ShouldBeFalse();
        property.IsCollection.ShouldBeFalse();
    }

    [Fact]
    public void InterfaceData_CanSetProperties()
    {
        // Arrange & Act
        var interfaceData = new InterfaceData
        {
            Namespace = "MyApp.Contracts",
            Name = "IRepository",
            Methods = Array.Empty<MethodData>(),
            Attributes = Array.Empty<string>(),
            Properties = Array.Empty<PropertyData>(),
            Interfaces = Array.Empty<string>()
        };

        // Assert
        interfaceData.Name.ShouldBe("IRepository");
        interfaceData.Namespace.ShouldBe("MyApp.Contracts");
        interfaceData.GetIdentifier().ShouldBe("MyApp.Contracts.IRepository");
    }

    [Fact]
    public void MethodData_CanSetSignature()
    {
        // Arrange & Act
        var method = new MethodData
        {
            Name = "GetById",
            IsAsync = true,
            Parameters = Array.Empty<ParameterData>(),
            ReturnType = "Task<User>",
            GenericParameters = Array.Empty<string>(),
            IsNullable = false,
            ReturnsCollection = false
        };

        // Assert
        method.Name.ShouldBe("GetById");
        method.IsAsync.ShouldBeTrue();
        method.Parameters.ShouldBeEmpty();
        method.ReturnType.ShouldBe("Task<User>");
    }

    [Fact]
    public void ConstructorData_CanSetParameters()
    {
        // Arrange & Act
        var constructor = new ConstructorData
        {
            Parameters = new[]
            {
                new ParameterData { Name = "id", Type = "int" }
            }
        };

        // Assert
        constructor.Parameters.Count.ShouldBe(1);
        constructor.Parameters[0].Name.ShouldBe("id");
        constructor.Parameters[0].Type.ShouldBe("int");
    }

    [Fact]
    public void EnumLiteralData_GeneratesIdentifier()
    {
        // Arrange
        var literal = new EnumLiteralData { Name = "Pending", Value = "0" };

        // Act
        var identifier = literal.GetIdentifier();

        // Assert
        identifier.ShouldBe("Pending");
    }

    [Fact]
    public void PropertyData_GeneratesIdentifier()
    {
        // Arrange
        var property = new PropertyData
        {
            Name = "UserId",
            Type = "string",
            Attributes = new List<string>()
        };

        // Act
        var identifier = property.GetIdentifier();

        // Assert
        identifier.ShouldBe("UserId");
    }
}
