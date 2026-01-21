using Intent.Persistence;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Composes test scenarios for C# code analysis
/// </summary>
public static class CodeScenarioComposer
{
    public class Scenario
    {
        public required string[] CodeFiles { get; init; }
        public required string[] FolderPaths { get; init; }
    }

    /// <summary>
    /// Creates a scenario from inline C# code strings
    /// </summary>
    public static Scenario FromCode(string code)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"csharp-importer-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempPath);
        
        var filePath = Path.Combine(tempPath, "TestCode.cs");
        File.WriteAllText(filePath, code);
        
        return new Scenario
        {
            CodeFiles = [filePath],
            FolderPaths = [tempPath]
        };
    }

    /// <summary>
    /// Creates a scenario from multiple code files
    /// </summary>
    public static Scenario FromMultipleFiles(params (string filename, string code)[] files)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"csharp-importer-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempPath);
        
        var filePaths = new List<string>();
        foreach (var (filename, code) in files)
        {
            var filePath = Path.Combine(tempPath, filename);
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            File.WriteAllText(filePath, code);
            filePaths.Add(filePath);
        }
        
        return new Scenario
        {
            CodeFiles = filePaths.ToArray(),
            FolderPaths = [tempPath]
        };
    }

    /// <summary>
    /// Creates a scenario from the TestDataGenerator Domain project
    /// </summary>
    public static Scenario FromTestDataGeneratorDomain()
    {
        var domainPath = Path.Combine(
            Path.GetDirectoryName(typeof(CodeScenarioComposer).Assembly.Location) ?? "",
            "..", "..", "..", "..", "..", "Tests", "CSharpImportTests", "TestDataGenerator", "TestDataGenerator.Domain");
        
        if (!Directory.Exists(domainPath))
        {
            throw new InvalidOperationException($"TestDataGenerator.Domain folder not found at: {domainPath}");
        }

        var csharpFiles = Directory.GetFiles(domainPath, "*.cs", SearchOption.AllDirectories);
        
        return new Scenario
        {
            CodeFiles = csharpFiles,
            FolderPaths = [domainPath]
        };
    }

    /// <summary>
    /// Creates a scenario from the TestDataGenerator Application project
    /// </summary>
    public static Scenario FromTestDataGeneratorApplication()
    {
        var appPath = Path.Combine(
            Path.GetDirectoryName(typeof(CodeScenarioComposer).Assembly.Location) ?? "",
            "..", "..", "..", "..", "..", "Tests", "CSharpImportTests", "TestDataGenerator", "TestDataGenerator.Application");
        
        if (!Directory.Exists(appPath))
        {
            throw new InvalidOperationException($"TestDataGenerator.Application folder not found at: {appPath}");
        }

        var csharpFiles = Directory.GetFiles(appPath, "*.cs", SearchOption.AllDirectories);
        
        return new Scenario
        {
            CodeFiles = csharpFiles,
            FolderPaths = [appPath]
        };
    }

    /// <summary>
    /// Cleanup temporary files created for a scenario
    /// </summary>
    public static void CleanupScenario(Scenario scenario)
    {
        foreach (var folder in scenario.FolderPaths)
        {
            if (Directory.Exists(folder) && folder.Contains("csharp-importer-test-"))
            {
                try
                {
                    Directory.Delete(folder, recursive: true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
