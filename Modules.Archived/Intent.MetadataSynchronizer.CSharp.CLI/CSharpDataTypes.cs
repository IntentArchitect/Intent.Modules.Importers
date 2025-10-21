namespace Intent.MetadataSynchronizer.CSharp.Importer;

class CoreTypesData
{
    // These are populated based on folder settings supplied by the CSharpConfig Folder properties.
    public IList<ClassData> DomainClasses { get; } = new List<ClassData>();
    public IList<EnumData> DomainEnums { get; } = new List<EnumData>();
    public IList<ClassData> DomainServices { get; } = new List<ClassData>();
    public IList<ClassData> DomainRepositories { get; } = new List<ClassData>();
    public IList<ClassData> DomainDataContracts { get; } = new List<ClassData>();
    public IList<EnumData> ServiceEnums { get; } = new List<EnumData>();
    public IList<ClassData> ServiceDTOs { get; } = new List<ClassData>();
    public IList<ClassData> ValueObjects { get; } = new List<ClassData>();
    public IList<ClassData> EventMessages { get; } = new List<ClassData>();
}

record ClassData
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
}

record EnumData
{
    public string? FilePath { get; init; }
    public string Namespace { get; init; }
    public string Name { get; init; }
    public IReadOnlyList<EnumLiteralData> Literals { get; init; }
}

record EnumLiteralData
{
    public string Name { get; init; }
    public string? Value { get; init; }
}

record ConstructorData
{
    public IReadOnlyList<ParameterData> Parameters { get; init; }
}

record PropertyData
{
    public string Name { get; init; }
    public string? Type { get; init; }
    public bool IsCollection { get; init; }
    public bool IsNullable { get; init; }
    public List<string> Attributes { get; init; } = [];
}

record MethodData
{
    public string Name { get; init; }
    public string? ReturnType { get; init; }
    public IReadOnlyList<ParameterData> Parameters { get; init; }
    public bool IsAsync { get; init; }
    public bool ReturnsCollection { get; init; }
    public bool IsNullable { get; init; }
    public IReadOnlyList<string> GenericParameters { get; init; }
}

record ParameterData
{
    public string Name { get; init; }
    public string? Type { get; init; }
    public bool IsCollection { get; init; }
    public bool IsNullable { get; init; }
}
