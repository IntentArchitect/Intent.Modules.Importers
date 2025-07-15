using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;

namespace Intent.MetadataSynchronizer.Java;

internal class Program
{
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code")]
    public static void Main(string[] args)
    {
        static void WriteHelp(string configFileName)
        {
            Console.WriteLine($"Ensure that {configFileName} exists and that its fields are populated as follows:");
            Console.WriteLine();

            foreach (var property in typeof(Config).GetProperties())
            {
                Console.WriteLine($"- {property.Name}: {property.GetCustomAttribute<DescriptionAttribute>()!.Description}");
                Console.WriteLine();
            }
        }

        var configFileName = $"{Path.GetFileNameWithoutExtension(Environment.ProcessPath)}.json";
        if (args.Length >= 1 &&
            args[0].ToLowerInvariant() is "--help" or "-help" or "help" or "--?" or "-?" or "?")
        {
            WriteHelp(configFileName);

            return;
        }

        if (!File.Exists(configFileName))
        {
            Console.WriteLine($"Could not find {configFileName}, do you want to create one (Y/N)?");
            if ((Console.ReadLine() ?? string.Empty).Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                File.WriteAllText(configFileName, JsonSerializer.Serialize(
                    new Config(),
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    }));
                Console.WriteLine($"Created {configFileName}, please edit its values and then run the utility again.");
            }

            Console.WriteLine("Exiting.");
            return;
        }

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configFileName),
            new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            });
        if (config == null)
        {
            Console.WriteLine($"Deserializing {configFileName} resulted in null.");
            WriteHelp(configFileName);
            return;
        }

        Execute(
            sourcesPath: config.SourcesPath,
            intentSolutionPath: config.IntentSolutionPath,
            applicationName: config.ApplicationName,
            designerName: config.DesignerName,
            packageId: config.PackageId,
            targetParentId: config.TargetFolderId,
            deleteExtra: config.DeleteExtraElements,
            debug: config.Debug,
            createAttributesWithUnknownTypes: config.CreateAttributesWithUnknownTypes,
            stereotypeManagementMode: config.StereotypeManagementMode,
            applyJavaStereotypes: config.ApplyJavaStereotypes,
            ignores: config.Ignores ?? Array.Empty<string>());

        if (!config.SkipPressAnyKeyToExit)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static void Execute(string sourcesPath,
        string intentSolutionPath,
        string applicationName,
        string designerName,
        string packageId,
        string targetParentId,
        bool deleteExtra,
        bool debug,
        bool createAttributesWithUnknownTypes,
        StereotypeManagementMode stereotypeManagementMode,
        bool applyJavaStereotypes,
        IReadOnlyCollection<string> ignores)
    {
        Helpers.Execute(
            intentSolutionPath: intentSolutionPath,
            applicationName: applicationName,
            designerName: designerName,
            packageId: packageId,
            targetFolderId: targetParentId,
            deleteExtra: deleteExtra,
            debug: debug,
            createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
            stereotypeManagementMode: stereotypeManagementMode,
            additionalPreconditionChecks: AdditionalPreconditionChecks,
            getPersistables: GetPersistables);

        void AdditionalPreconditionChecks()
        {
            if (string.IsNullOrWhiteSpace(sourcesPath)) throw new ArgumentNullException(nameof(sourcesPath));
        }

        Persistables GetPersistables(IReadOnlyCollection<PackageModelPersistable> packages)
        {
            return JavaSource.GetPersistables(
                sourcesPath: sourcesPath,
                packages: packages,
                ignores: ignores,
                applyJavaStereotypes: applyJavaStereotypes);
        }
    }
}