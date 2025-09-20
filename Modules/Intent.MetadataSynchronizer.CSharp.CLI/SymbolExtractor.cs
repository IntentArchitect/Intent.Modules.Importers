using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.MetadataSynchronizer.CSharp.Importer;

internal static class SymbolExtractor
{
    public static ClassData GetClassData(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration, CSharpCompilation compilation)
    {
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration)!;
        return new ClassData
        {
            FilePath = classDeclaration.SyntaxTree.FilePath,
            Namespace = classSymbol.ContainingNamespace?.ToString() ?? "UnknownNamespace",
            Name = classSymbol.Name,
            BaseType = GetBaseType(classSymbol, compilation),
            Interfaces = GetInterfaces(classSymbol, compilation),
            Attributes = GetAttributes(classSymbol),
            Constructors = GetConstructors(semanticModel, classDeclaration.Members, compilation),
            Properties = GetProperties(semanticModel, classDeclaration.Members, compilation),
            Methods = GetMethods(semanticModel, classDeclaration.Members, compilation)
        };
    }

    public static ClassData GetRecordData(SemanticModel semanticModel, RecordDeclarationSyntax recordDeclaration, CSharpCompilation compilation)
    {
        var classSymbol = semanticModel.GetDeclaredSymbol(recordDeclaration)!;
        return new ClassData
        {
            FilePath = recordDeclaration.SyntaxTree.FilePath,
            Namespace = classSymbol.ContainingNamespace?.ToString() ?? "UnknownNamespace",
            Name = classSymbol.Name,
            BaseType = GetBaseType(classSymbol, compilation),
            Interfaces = GetInterfaces(classSymbol, compilation),
            Attributes = GetAttributes(classSymbol),
            Constructors = GetConstructors(semanticModel, recordDeclaration.Members, compilation),
            Properties = GetProperties(semanticModel, recordDeclaration.Members, compilation),
            Methods = GetMethods(semanticModel, recordDeclaration.Members, compilation)
        };
    }
    
    public static EnumData GetEnumData(SemanticModel semanticModel, EnumDeclarationSyntax enumDeclaration)
    {
        var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration)!;
        return new EnumData
        {
            FilePath = enumDeclaration.SyntaxTree.FilePath,
            Namespace = enumSymbol.ContainingNamespace?.ToString() ?? "UnknownNamespace",
            Name = enumSymbol.Name,
            Literals = GetEnumLiterals(enumSymbol, enumDeclaration)
        };
    }

    private static IReadOnlyList<EnumLiteralData> GetEnumLiterals(INamedTypeSymbol enumSymbol, EnumDeclarationSyntax enumDeclaration)
    {
        var literals = new List<EnumLiteralData>();

        foreach (var member in enumSymbol.GetMembers().Where(m => m.Kind == SymbolKind.Field))
        {
            var fieldSymbol = (IFieldSymbol)member;
            var value = fieldSymbol.ConstantValue != null ? fieldSymbol.ConstantValue.ToString() : null;

            literals.Add(new EnumLiteralData
            {
                Name = fieldSymbol.Name,
                Value = value
            });
        }

        return literals;
    }

    private static IReadOnlyList<ConstructorData> GetConstructors(SemanticModel semanticModel, SyntaxList<MemberDeclarationSyntax> memberDeclarations, CSharpCompilation compilation)
    {
        return memberDeclarations
            .OfType<ConstructorDeclarationSyntax>()
            .Select(ctor => new ConstructorData
            {
                Parameters = MapParameterListSyntaxToParameterData(semanticModel, ctor.ParameterList, compilation)
            })
            .ToList();
    }

    
    private static IReadOnlyList<PropertyData> GetProperties(SemanticModel semanticModel, SyntaxList<MemberDeclarationSyntax> memberDeclarations, CSharpCompilation compilation)
    {
        return memberDeclarations
            .OfType<PropertyDeclarationSyntax>()
            .Where(property => property.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
            .Select(property => MapPropertyDeclarationToPropertyData(semanticModel, property, compilation))
            .ToList();
    }

    private static IReadOnlyList<MethodData> GetMethods(SemanticModel semanticModel, SyntaxList<MemberDeclarationSyntax> memberDeclarations, CSharpCompilation compilation)
    {
        return memberDeclarations
            .OfType<MethodDeclarationSyntax>()
            .Where(method => method.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
            .Select(method => MapMethodDeclarationToMethodData(semanticModel, method, compilation))
            .ToList();
    }
    
    private static PropertyData MapPropertyDeclarationToPropertyData(SemanticModel semanticModel, PropertyDeclarationSyntax propertyDeclaration, CSharpCompilation compilation)
    {
        var typeInfo = semanticModel.GetTypeInfo(propertyDeclaration.Type);
        return new PropertyData
        {
            Name = propertyDeclaration.Identifier.ValueText,
            Type = TypeAnalysis.GetFullTypeName(typeInfo.Type, compilation),
            IsCollection = TypeAnalysis.IsCollection(typeInfo.Type, compilation),
            IsNullable = TypeAnalysis.IsNullable(typeInfo.Type) || propertyDeclaration.Type is NullableTypeSyntax,
            Attributes = propertyDeclaration.AttributeLists.SelectMany(a => a.Attributes.Select(t => t.Name.ToString())).ToList()
        };
    }

    private static MethodData MapMethodDeclarationToMethodData(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CSharpCompilation compilation)
    {
        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
        if (methodSymbol is not null)
        {
            var resolvedReturnType = TypeAnalysis.GetFullTypeName(methodSymbol.ReturnType, compilation);
            if (resolvedReturnType == "void")
            {
                resolvedReturnType = null;
            }
            
            return new MethodData
            {
                Name = methodDeclaration.Identifier.ValueText,
                ReturnType = resolvedReturnType,
                Parameters = MapParameterListSyntaxToParameterData(semanticModel, methodDeclaration.ParameterList, compilation),
                IsAsync = TypeAnalysis.IsAsyncMethod(methodDeclaration),
                ReturnsCollection = TypeAnalysis.IsCollection(methodSymbol.ReturnType, compilation),
                IsNullable = TypeAnalysis.IsNullable(methodSymbol.ReturnType),
                GenericParameters = TypeAnalysis.GetGenericParameters(methodSymbol)
            };
        }

        throw new InvalidOperationException("Could not get method symbol information");
    }

    private static IReadOnlyList<ParameterData> MapParameterListSyntaxToParameterData(SemanticModel semanticModel, ParameterListSyntax parameterList, CSharpCompilation compilation)
    {
        return parameterList.Parameters
            .Select(parameter =>
            {
                var param = semanticModel.GetDeclaredSymbol(parameter);
                return new ParameterData
                {
                    Name = parameter.Identifier.ValueText,
                    Type = TypeAnalysis.GetFullTypeName(param!.Type, compilation),
                    IsCollection = TypeAnalysis.IsCollection(param.Type, compilation),
                    IsNullable = TypeAnalysis.IsNullable(param.Type)
                };
            })
            .ToList();
    }

    private static string? GetBaseType(INamedTypeSymbol classSymbol, CSharpCompilation compilation)
    {
        return classSymbol.BaseType != null && classSymbol.BaseType.SpecialType != SpecialType.System_Object
            ? TypeAnalysis.GetFullTypeName(classSymbol.BaseType, compilation)
            : null;
    }

    private static List<string> GetInterfaces(INamedTypeSymbol classSymbol, CSharpCompilation compilation)
    {
        return classSymbol.Interfaces
            .Select(s => TypeAnalysis.GetFullTypeName(s, compilation))
            .Where(p => p is not null)
            .Cast<string>()
            .ToList();
    }

    private static IReadOnlyList<string> GetAttributes(ISymbol symbol)
    {
        return symbol.GetAttributes()
            .Select(attr => attr.AttributeClass)
            .Where(attrClass => attrClass is not null)
            .Select(attrClass => attrClass!.ToDisplayString())
            .ToList();
    }
}