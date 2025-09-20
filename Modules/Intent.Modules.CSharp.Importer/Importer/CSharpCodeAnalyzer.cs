using Intent.MetadataSynchronizer.CSharp.Importer;
using Intent.Modules.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.CSharp.Importer.Importer;

internal static class CSharpCodeAnalyzer
{
    public static async Task<CoreTypesData> ImportMetadataFromFolder(string folder)
    {
        IEnumerable<string> csharpFiles = [];
        if (!string.IsNullOrWhiteSpace(folder))
        {
            csharpFiles = Directory.EnumerateFiles(folder, "*.cs", new EnumerationOptions
            {
                RecurseSubdirectories = true
            });
        }

        return await ImportMetadataFromFiles(csharpFiles.ToArray());
    }

    public static async Task<CoreTypesData> ImportMetadataFromFiles(string[] files)
    {
        var syntaxTrees = await ReadSyntaxTreesFromFolder(files);
        var compilation = CreateCompilationForFullTypeResolution(syntaxTrees);
        var coreTypes = AnalyzeCSharpCoreTypesInSyntaxTrees(compilation, syntaxTrees);
        return coreTypes;
    }

    private static async Task<IReadOnlyList<SyntaxTree>> ReadSyntaxTreesFromFolder(string[] files)
    {

        var syntaxTrees = new List<SyntaxTree>();
        var visited = new HashSet<string>();


        syntaxTrees.AddRange(await ReadDomainEntities(files, visited));

        return syntaxTrees;

        static async Task<List<SyntaxTree>> ReadDomainEntities(string[] csharpFiles, HashSet<string> visited)
        {
            var syntaxTrees = new List<SyntaxTree>();

            foreach (var file in csharpFiles)
            {
                if (visited.Contains(file.NormalizePath()))
                {
                    continue;
                }
                var text = await File.ReadAllTextAsync(file);
                var syntaxTree = CSharpSyntaxTree.ParseText(text, path: file.NormalizePath());

                visited.Add(file.NormalizePath());
                syntaxTrees.Add(syntaxTree);
            }

            return syntaxTrees;
        }
    }

    private static bool IsOnlyEnum(SyntaxTree syntaxTree)
    {
        var rootSyntax = syntaxTree.GetCompilationUnitRoot().DescendantNodes()
            .Where(p => p.Parent is BaseNamespaceDeclarationSyntax);

        // if there is no syntax items which have a parent as namespace
        // in other words, there was no namespace specified
        if (!rootSyntax.Any())
        {
            // check if all of the items at the root of the syntax tree are enums
            rootSyntax = syntaxTree.GetCompilationUnitRoot().ChildNodes();

            return rootSyntax
               .All(p => p is EnumDeclarationSyntax);
        }

        // the "skip" 1 is to cater for the fact that the first item returned is
        // the name of the namespace
        return rootSyntax
            .Skip(1)
            .All(p => p is EnumDeclarationSyntax);
    }

    private static CSharpCompilation CreateCompilationForFullTypeResolution(IReadOnlyList<SyntaxTree> syntaxTrees)
    {
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        return CSharpCompilation.Create("CodeAnalysis", syntaxTrees, references, compilationOptions);
    }

    private static CoreTypesData AnalyzeCSharpCoreTypesInSyntaxTrees(
        CSharpCompilation compilation,
        IReadOnlyList<SyntaxTree> syntaxTrees)
    {
        var data = new CoreTypesData();

        ExtractClassesFromSyntaxTrees(compilation, syntaxTrees, data);
        ExtractRecordsFromSyntaxTrees(compilation, syntaxTrees, data);
        ExtractEnumsFromSyntaxTrees(compilation, syntaxTrees, data);

        return data;
    }

    private static void ExtractClassesFromSyntaxTrees(
        CSharpCompilation compilation,
        IReadOnlyList<SyntaxTree> syntaxTrees,
        CoreTypesData data)
    {
        var classDeclarations = syntaxTrees
            .SelectMany(tree => tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            .Where(clazz => clazz.Parent is BaseNamespaceDeclarationSyntax && // Filter out nested classes
                            !clazz.Modifiers.Any(SyntaxKind.StaticKeyword) && // No static classes
                            (clazz.TypeParameterList is null || clazz.TypeParameterList.Parameters.Count == 0) // No generic parameters
            )
            .ToList();

        ExtractPartialClassesFromSyntaxTrees(compilation, classDeclarations, data);
        ExtractNonPartialClassesFromSyntaxTrees(compilation, classDeclarations, data);

        return;

        static void ExtractPartialClassesFromSyntaxTrees(
            CSharpCompilation compilation,
            List<ClassDeclarationSyntax> classDeclarations,
            CoreTypesData data)
        {
            var groupedClassDeclarations = classDeclarations
                .Where(c => c.Modifiers.Any(SyntaxKind.PartialKeyword))
                .GroupBy(c => c.Identifier.Text);

            foreach (var group in groupedClassDeclarations)
            {
                var firstClassDeclaration = group.First();
                var namespaceDeclaration = firstClassDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

                var newClassDeclaration = SyntaxFactory.ClassDeclaration(group.Key)
                    .WithModifiers(firstClassDeclaration.Modifiers)
                    .WithTypeParameterList(firstClassDeclaration.TypeParameterList)
                    .WithAttributeLists(firstClassDeclaration.AttributeLists)
                    .WithBaseList(firstClassDeclaration.BaseList)
                    .WithConstraintClauses(firstClassDeclaration.ConstraintClauses)
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>());

                foreach (var classDeclaration in group)
                {
                    newClassDeclaration = newClassDeclaration.AddMembers(classDeclaration.Members.ToArray());
                }

                var newRoot = SyntaxFactory.CompilationUnit()
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newClassDeclaration));

                if (namespaceDeclaration != null)
                {
                    newRoot = newRoot.WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            namespaceDeclaration.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newClassDeclaration))));
                }

                var newTree = newRoot.SyntaxTree.WithFilePath(firstClassDeclaration.SyntaxTree.FilePath);

                var newCompilation = compilation.AddSyntaxTrees(newTree);

                newClassDeclaration = newTree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .First(p => p.Identifier.ValueText == newClassDeclaration.Identifier.ValueText);

                var classData = ExtractClassFromSyntaxTree(newCompilation, newTree, newClassDeclaration);

                ApplyDataForClassesAndRecords(firstClassDeclaration, data, classData);
            }
        }

        static void ExtractNonPartialClassesFromSyntaxTrees(
            CSharpCompilation compilation,
            List<ClassDeclarationSyntax> classDeclarations,
            CoreTypesData data)
        {
            var normalClassDeclarations = classDeclarations.Where(c => !c.Modifiers.Any(SyntaxKind.PartialKeyword));
            foreach (var declarationSyntax in normalClassDeclarations)
            {
                var classData = ExtractClassFromSyntaxTree(compilation, declarationSyntax.SyntaxTree, declarationSyntax);
                ApplyDataForClassesAndRecords(declarationSyntax, data, classData);
            }
        }

        static ClassData ExtractClassFromSyntaxTree(
            CSharpCompilation compilation,
            SyntaxTree tree,
            ClassDeclarationSyntax classDeclaration)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            return SymbolExtractor.GetClassData(semanticModel, classDeclaration, compilation);
        }
    }

    private static void ExtractRecordsFromSyntaxTrees(CSharpCompilation compilation, IReadOnlyList<SyntaxTree> syntaxTrees, CoreTypesData data)
    {
        var recordDeclarations = syntaxTrees
            .SelectMany(s => s.GetCompilationUnitRoot().DescendantNodes().OfType<RecordDeclarationSyntax>())
            .Where(p => p.Parent is BaseNamespaceDeclarationSyntax)
            .ToArray();
        foreach (var declarationSyntax in recordDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(declarationSyntax.SyntaxTree);
            var recordData = SymbolExtractor.GetRecordData(semanticModel, declarationSyntax, compilation);

            ApplyDataForClassesAndRecords(declarationSyntax, data, recordData);
        }
    }

    private static void ApplyDataForClassesAndRecords(BaseTypeDeclarationSyntax typeDeclaration, CoreTypesData data, ClassData classData)
    {
        data.Classes.Add(classData);
    }

    private static void ExtractEnumsFromSyntaxTrees(
        CSharpCompilation compilation,
        IReadOnlyList<SyntaxTree> syntaxTrees,
        CoreTypesData data)
    {
        var enumDeclarations = syntaxTrees
            .SelectMany(s => s.GetCompilationUnitRoot().DescendantNodes().OfType<EnumDeclarationSyntax>())
            .Where(p => p.Parent is BaseNamespaceDeclarationSyntax)
            .ToArray();
        foreach (var enumDeclaration in enumDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(enumDeclaration.SyntaxTree);
            var @enum = SymbolExtractor.GetEnumData(semanticModel, enumDeclaration);
            data.Enums.Add(@enum);
            //switch (hintHelper.GetHint(enumDeclaration))
            //{
            //    case HintHelper.HintDomainEnums:
            //        data.DomainEnums.Add(@enum);
            //        break;
            //    case HintHelper.HintServiceEnums:
            //        data.ServiceEnums.Add(@enum);
            //        break;
            //}
        }
    }
}
