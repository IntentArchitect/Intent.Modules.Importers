using System.Xml.Linq;

namespace Intent.MetadataSynchronizer.CSharp.CLI;

public class CoreTypesData
{
    public IList<ClassData> Classes { get; } = new List<ClassData>();
    public IList<EnumData> Enums { get; } = new List<EnumData>();
}

public record ClassData
{
    public string? FilePath { get; init; }
    public string Namespace { get; init; }
    public string Name { get; init; }
    public string? BaseType { get; init; }
    public IReadOnlyList<string> Interfaces { get; init; }
    public IReadOnlyList<string> Attributes { get; init; }
    public IReadOnlyList<ConstructorData> Constructors { get; init; }
    public IReadOnlyList<PropertyData> Properties { get; init; }
    public IReadOnlyList<MethodData> Methods { get; init; }

    public string GetIdentifier()
    {
        return $"{Namespace}.{Name}";
    }
}

public record EnumData
{
    public string? FilePath { get; init; }
    public string Namespace { get; init; }
    public string Name { get; init; }
    public IReadOnlyList<EnumLiteralData> Literals { get; init; }
}

public record EnumLiteralData
{
    public string Name { get; init; }
    public string? Value { get; init; }
}

public record ConstructorData
{
    public IReadOnlyList<ParameterData> Parameters { get; init; }

    public string GetIdentifier()
    {
        return $"constructor({string.Join(",", Parameters.Select(x => x.Type))})";
    }
}

public record PropertyData
{
    public string Name { get; init; }
    public string? Type { get; init; }
    public bool IsCollection { get; init; }
    public bool IsNullable { get; init; }
    public List<string> Attributes { get; init; } = [];

    public string GetIdentifier()
    {
        return $"{Name}";
    }
}

public record MethodData
{
    public string Name { get; init; }
    public string? ReturnType { get; init; }
    public IReadOnlyList<ParameterData> Parameters { get; init; }
    public bool IsAsync { get; init; }
    public bool ReturnsCollection { get; init; }
    public bool IsNullable { get; init; }
    public IReadOnlyList<string> GenericParameters { get; init; }

    public string GetIdentifier()
    {
        return $"{Name}({string.Join(",", Parameters.Select(x => x.Type))})";
    }
}

public record ParameterData
{
    public string Name { get; init; }
    public string? Type { get; init; }
    public bool IsCollection { get; init; }
    public bool IsNullable { get; init; }

    public string GetIdentifier()
    {
        return Name;
    }
}
