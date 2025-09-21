using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI
{
    internal class Program
    {
        private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";

        static void Main(string[] args)
        {
            var rootCommand = new RootCommand(
                "The Intent JSON Metadata Synchronizer CLI tool can be used to synchronize an object " +
                "graph in JSON files into an Intent Architect package within any supported designer.")
            {
                new Option<FileInfo>(
                    name: GetOptionName(nameof(JsonConfig.ConfigFile)),
                    description: "Path to a JSON formatted file containing options to use for execution of " +
                                 "this tool as an alternative to using command line options. The " +
                                 $"{GetOptionName(nameof(JsonConfig.GenerateConfigFile))} option can be used to " +
                                 "generate a file with all the possible fields populated with null."),
                new Option<bool>(
                    name: GetOptionName(nameof(JsonConfig.GenerateConfigFile)),
                    description: $"Scaffolds into the current working directory a \"config.json\" for use with the " +
                                 $"{GetOptionName(nameof(JsonConfig.ConfigFile))} option."),
                new Option<DirectoryInfo>(
                    name: GetOptionName(nameof(JsonConfig.SourceJsonFolder)),
                    description: "The path to the folder containing JSON files to parse and synchronize into the Intent Architect Package."),
                new Option<FileInfo>(
                    name: GetOptionName(nameof(JsonConfig.IslnFile)),
                    description: "The Intent Architect solution (.isln) file containing the Intent Architect Application " +
                                 "into which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.ApplicationName)),
                    description: "The name of the Intent Architect Application (as per the Application Settings view) " +
                                 "containing the Intent Architect Package into which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.PackageId)),
                    description: "The id of the Intent Architect Package containing the Intent Architect Package into " +
                                 "which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.TargetFolderId)),
                    description: "The target folder within the Intent Architect package into which to synchronize the " +
                                 "metadata. If unspecified then the metadata will be synchronized into the root of the " +
                                 "Intent Architect package."),
                new Option<CasingConvention?>(
                    name: GetOptionName(nameof(JsonConfig.CasingConvention)),
                    description: "Casing convention to be applied on imported elements. Options are PascalCase or AsIs."),
                new Option<ImportProfile?>(
                    name: GetOptionName(nameof(JsonConfig.Profile)),
                    description: "Import profile to use. Options are DomainDocumentDB or EventingMessages.")
            };

            rootCommand.SetHandler(
                handle: (
                    FileInfo? configFilePath,
                    bool generateConfigFile,
                    DirectoryInfo? sourceJsonFolder,
                    FileInfo? islnFile,
                    string? applicationName,
                    string? packageId,
                    string? targetFolderId,
                    CasingConvention? casingConvention,
                    ImportProfile? profile) =>
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() },
                        WriteIndented = true
                    };

                    if (generateConfigFile)
                    {
                        var path = Path.Join(Environment.CurrentDirectory, "config.json");
                        Console.WriteLine($"Writing {path}...");
                        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(new JsonConfig(), serializerOptions));
                        Console.WriteLine("Done.");
                        return;
                    }

                    var deleteExtra = false;
                    var debug = true;
                    var createAttributesWithUnknownTypes = true;
                    var stereotypeManagementMode = StereotypeManagementMode.Merge;

                    var configFileOptionName = GetOptionName(nameof(JsonConfig.ConfigFile));

                    JsonConfig configFile;
                    if (configFilePath != null)
                    {
                        if (sourceJsonFolder != null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.SourceJsonFolder))} not allowed when {configFileOptionName} specified.");

                        if (islnFile != null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.IslnFile))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(applicationName))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.ApplicationName))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(packageId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.PackageId))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(targetFolderId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.TargetFolderId))} not allowed when {configFileOptionName} specified.");

                        configFile = JsonSerializer.Deserialize<JsonConfig>(File.ReadAllText(configFilePath.FullName), serializerOptions)
                                     ?? throw new Exception($"Parsing of \"{configFilePath.FullName}\" returned null.");

                        if (string.IsNullOrWhiteSpace(configFile.SourceJsonFolder) || !Directory.Exists(configFile.SourceJsonFolder))
                            throw new Exception($"Config file field {nameof(configFile.SourceJsonFolder)} must be a valid directory path.");

                        if (string.IsNullOrWhiteSpace(configFile.IslnFile) || !File.Exists(configFile.IslnFile))
                            throw new Exception($"Config file field {nameof(configFile.IslnFile)} must be a valid file path.");

                        if (string.IsNullOrWhiteSpace(configFile.ApplicationName))
                            throw new Exception($"Config file field {nameof(configFile.ApplicationName)} must be specified.");

                        if (string.IsNullOrWhiteSpace(configFile.PackageId))
                            throw new Exception($"Config file field {nameof(configFile.PackageId)} must be specified.");
                    }
                    else
                    {
                        if (sourceJsonFolder == null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.SourceJsonFolder))} mandatory when {configFileOptionName} not specified.");

                        if (islnFile == null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.IslnFile))} mandatory when {configFileOptionName} not specified.");

                        if (string.IsNullOrWhiteSpace(applicationName))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.ApplicationName))} mandatory when {configFileOptionName} not specified.");

                        if (string.IsNullOrWhiteSpace(packageId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.PackageId))} mandatory when {configFileOptionName} not specified.");

                        configFile = new JsonConfig
                        {
                            SourceJsonFolder = sourceJsonFolder.FullName,
                            IslnFile = islnFile.FullName,
                            ApplicationName = applicationName,
                            PackageId = packageId,
                            TargetFolderId = !string.IsNullOrWhiteSpace(targetFolderId) ? targetFolderId : null
                        };
                        if (casingConvention != null)
                        {
                            configFile.CasingConvention = casingConvention.Value;
                        }
                        if (profile != null)
                        {
                            configFile.Profile = profile.Value;
                        }
                    }

                    // Resolve settings from profile
                    var visitor = ProfileFactory.GetVisitorForProfile(configFile.Profile);

                    Helpers.Execute(
                        intentSolutionPath: configFile.IslnFile,
                        applicationName: configFile.ApplicationName,
                        designerName: visitor.DesignerName,
                        packageId: configFile.PackageId,
                        targetFolderId: configFile.TargetFolderId,
                        deleteExtra: deleteExtra,
                        debug: debug,
                        createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                        stereotypeManagementMode: stereotypeManagementMode,
                        additionalPreconditionChecks: () => { },
                        getPersistables: packages => JsonPersistableFactory.GetPersistables(configFile, packages));
                },
                symbols: Enumerable.Empty<IValueDescriptor>()
                    .Concat(rootCommand.Options)
                    .ToArray());

            Console.WriteLine($"{rootCommand.Name} version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

            new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build()
                .Invoke(args);
        }
    }
}