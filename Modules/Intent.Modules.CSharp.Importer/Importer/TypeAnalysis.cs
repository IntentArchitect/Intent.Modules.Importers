using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.CSharp.Importer.Importer;

public static class TypeAnalysis
{
    private static readonly IReadOnlyList<string> CollectionTypes = new[]
        {
            typeof(IAsyncEnumerable<>).FullName!,
            typeof(IEnumerable<>).FullName!,
            typeof(ICollection<>).FullName!,
            typeof(IList<>).FullName!,
            typeof(List<>).FullName!,
            typeof(HashSet<>).FullName!,
            typeof(LinkedList<>).FullName!,
            typeof(Queue<>).FullName!,
            typeof(Stack<>).FullName!,
            typeof(Dictionary<,>).FullName!
        }
        .Select(s => s.Remove(s.Length - 2, 2))
        .ToArray();
    
    private static readonly IReadOnlyDictionary<string, string> SimplifiedTypeNames = new Dictionary<string, string>
    {
        { "System.Boolean", "bool" },
        { "System.Byte", "byte" },
        { "System.SByte", "sbyte" },
        { "System.Char", "char" },
        { "System.Decimal", "decimal" },
        { "System.Double", "double" },
        { "System.Single", "float" },
        { "System.Int32", "int" },
        { "System.UInt32", "uint" },
        { "System.Int64", "long" },
        { "System.UInt64", "ulong" },
        { "System.Object", "object" },
        { "System.Int16", "short" },
        { "System.UInt16", "ushort" },
        { "System.String", "string" },
        { "System.DateTime", "datetime" },
        { "System.TimeSpan", "timespan" },
        { "System.DateTimeOffset", "datetimeoffset" },
        { "System.Void", "void" },
        { "System.Guid", "guid" },
        { "Boolean", "bool" },
        { "Byte", "byte" },
        { "SByte", "sbyte" },
        { "Char", "char" },
        { "Decimal", "decimal" },
        { "Double", "double" },
        { "Single", "float" },
        { "Int32", "int" },
        { "UInt32", "uint" },
        { "Int64", "long" },
        { "UInt64", "ulong" },
        { "Object", "object" },
        { "Int16", "short" },
        { "UInt16", "ushort" },
        { "String", "string" },
        { "DateTime", "datetime" },
        { "TimeSpan", "timespan" },
        { "DateTimeOffset", "datetimeoffset" },
        { "Void", "void" },
        { "Guid", "guid" },
        { "byte[]", "binary" }
    };

    public static bool IsCollection(ITypeSymbol? typeSymbol, CSharpCompilation compilation)
    {
        if (typeSymbol is IArrayTypeSymbol)
        {
            return true;
        }

        var internalTypeSymbol = !IsNullable(typeSymbol) ? typeSymbol : GetBaseTypeSymbol(typeSymbol, compilation);

        if (internalTypeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        var fullTypeName = namedTypeSymbol.ConstructedFrom.ToDisplayString();

        if (fullTypeName.EndsWith("<T>"))
        {
            fullTypeName = fullTypeName.Replace("<T>", "");
        }
        if (fullTypeName.EndsWith("<>"))
        {
            fullTypeName = fullTypeName.Replace("<>", "");
        }
        return CollectionTypes.Any(name => name.Contains(fullTypeName));
    }

    public static bool IsNullable(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        return typeSymbol.NullableAnnotation == NullableAnnotation.Annotated ||
               typeSymbol is INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
    }

    public static bool IsAsyncMethod(MethodDeclarationSyntax method)
    {
        var res = method switch
        {
            _ when method.ReturnType.IsKind(SyntaxKind.GenericName) => ((GenericNameSyntax)method.ReturnType).Identifier.ValueText.Contains("Task"),
            _ when method.ReturnType.IsKind(SyntaxKind.IdentifierName) => ((IdentifierNameSyntax)method.ReturnType).Identifier.ValueText.Contains("Task"),
            _ => false
        };
        return res;
    }

    public static string? GetFullTypeName(ITypeSymbol? typeSymbol, CSharpCompilation compilation)
    {
        if (typeSymbol is null)
        {
            return null;
        }

        if (IsNullable(typeSymbol) && IsCollection(typeSymbol, compilation))
        {
            typeSymbol = GetBaseTypeSymbol(GetBaseTypeSymbol(typeSymbol, compilation), compilation);
        }
        else
        {
            typeSymbol = GetBaseTypeSymbol(typeSymbol, compilation);
        }
        
        var displayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
        );

        var typeName = typeSymbol.ToDisplayString(displayFormat);
        typeName = typeName.Replace("global::", string.Empty);

        if (SimplifiedTypeNames.TryGetValue(typeName, out var simpleName))
        {
            return simpleName;
        }

        return typeName;
    }

    private static ITypeSymbol GetBaseTypeSymbol(ITypeSymbol typeSymbol, CSharpCompilation compilation)
    {
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType;
        }

        if (typeSymbol is not INamedTypeSymbol namedType)
        {
            return typeSymbol;
        }

        if (namedType.IsGenericType == false)
        {
            if (namedType.Name.Equals("Task") || namedType.Name.Equals("System.Threading.Tasks.Task"))
            {
                return compilation.GetSpecialType(SpecialType.System_Void);
            }
            return typeSymbol;
        }
        
        if ((namedType.ConstructedFrom.Name.Equals("Task") || namedType.ConstructedFrom.Name.Equals("System.Threading.Tasks.Task")) && namedType.IsGenericType)
        {
            typeSymbol = namedType.TypeArguments.First();
        }
        else if ((namedType.ConstructedFrom.Name.Equals("Task") || namedType.ConstructedFrom.Name.Equals("System.Threading.Tasks.Task")) && !namedType.IsGenericType)
        {
            typeSymbol = compilation.GetSpecialType(SpecialType.System_Void);
        }
        
        if (typeSymbol is not INamedTypeSymbol { IsGenericType: true } namedType2)
        {
            return typeSymbol;
        }
        
        if (namedType2.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T ||
            IsCollection(namedType2, compilation))
        {
            typeSymbol = namedType2.TypeArguments.First();
        }

        return typeSymbol;
    }

    public static IReadOnlyList<string> GetGenericParameters(IMethodSymbol methodSymbol)
    {
        return methodSymbol.TypeParameters.Select(s => s.Name).ToArray();
    }
}