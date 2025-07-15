using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.OpenApi.CLI
{
    internal class Program
    {
        private static string OptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";

        static int Main(string[] args)
        {
            var rootCommand = new RootCommand(
                "The Intent JSON Metadata Synchronizer CLI tool can be used to synchronize an object " +
                "graph in a JSON file into an Intent Architect Document DB Domain Package.")
            {
                new Option<FileInfo>(
                    name: OptionName(nameof(ImportConfig.ConfigFile)),
                    description: "Path to a JSON formatted file containing options to use for execution of " +
                                 "this tool as an alternative to using command line options. The " +
                                 $"{OptionName(nameof(ImportConfig.GenerateConfigFile))} option can be used to " +
                                 "generate a file with all the possible fields populated with null."),
                new Option<bool>(
                    name: OptionName(nameof(ImportConfig.GenerateConfigFile)),
                    description: $"Scaffolds into the current working directory a \"config.json\" for use with the " +
                                 $"{OptionName(nameof(ImportConfig.ConfigFile))} option."),
                new Option<FileInfo>(
                    name: OptionName(nameof(ImportConfig.OpenApiSpecificationFile)),
                    description: "The name of the OpenApi (3.*) file to parse and synchronize into the Intent Architect Package. This can be a Json or Xaml file. "),
                new Option<FileInfo>(
                    name: OptionName(nameof(ImportConfig.IslnFile)),
                    description: "The Intent Architect solution (.isln) file containing the Intent Architect Application " +
                                 "into which to synchronize the metadata."),
                new Option<string>(
                    name: OptionName(nameof(ImportConfig.ApplicationName)),
                    description: "The name of the Intent Architect Application (as per the Application Settings view) " +
                                 "containing the Intent Architect Package into which to synchronize the metadata."),
                new Option<string>(
                    name: OptionName(nameof(ImportConfig.PackageId)),
                    description: "The id of the Intent Architect Package containing the Intent Architect Package into " +
                                 "which to synchronize the metadata."),
                new Option<string>(
                    name: OptionName(nameof(ImportConfig.TargetFolderId)),
                    description: "The target folder within the Intent Architect package into which to synchronize the " +
                                 "metadata. If unspecified then the metadata will be synchronized into the root of the " +
                                 "Intent Architect package."),
                new Option<ServiceType?>(
                    name: OptionName(nameof(ImportConfig.ServiceType)),
                    description: "What paradigm of Service Model would you like. Options are CQRS or Service."),
                new Option<bool?>(
                    name: OptionName(nameof(ImportConfig.IsAzureFunctions)),
                    description: "Are these services exposed as AzureFunctions."),
                new Option<bool?>(
                    name: OptionName(nameof(ImportConfig.AllowRemoval)),
                    description: "Remove previously imported data which is no longer being imported?"),
                new Option<string?>(
                    name: OptionName(nameof(ImportConfig.SerializedConfig)),
                    description: "JSON string representing a serialized configuration file."),
            };

            rootCommand.SetHandler(
                handle: (
                    FileInfo? configFilePath,
                    bool generateConfigFile,
                    FileInfo? sourceFile,
                    FileInfo? islnFile,
                    string? applicationName,
                    string? packageId,
                    string? targetFolderId,
                    ServiceType? serviceType,
                    bool? isAzureFunctions,
                    bool? allowRemoval,
                    string? serializedConfig
                    ) =>
                {
                    try
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
                            File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(new ImportConfig(), serializerOptions));
                            Console.WriteLine("Done.");
                            return Task.FromResult(0);
                        }

                        var debug = true;
                        var createAttributesWithUnknownTypes = true;
                        var stereotypeManagementMode = StereotypeManagementMode.Merge;

                        ImportConfig configFile;
                        if (serializedConfig != null)
                        {
                            configFile = JsonSerializer.Deserialize<ImportConfig>(serializedConfig, serializerOptions)
                                         ?? throw new Exception("Parsing of serialized-config returned null.");
                        }
                        else if (configFilePath != null)
                        {
                            configFile = JsonSerializer.Deserialize<ImportConfig>(File.ReadAllText(configFilePath.FullName), serializerOptions)
                                         ?? throw new Exception($"Parsing of \"{configFilePath.FullName}\" returned null.");

                            if (string.IsNullOrWhiteSpace(configFile.OpenApiSpecificationFile) || !IsValidPath(configFile.OpenApiSpecificationFile))
                                throw new Exception($"Config file field {nameof(configFile.OpenApiSpecificationFile)} must be a valid file path.");

                            if (string.IsNullOrWhiteSpace(configFile.IslnFile) || !File.Exists(configFile.IslnFile))
                                throw new Exception($"Config file field {nameof(configFile.IslnFile)} must be a valid file path.");

                            if (string.IsNullOrWhiteSpace(configFile.ApplicationName))
                                throw new Exception($"Config file field {nameof(configFile.ApplicationName)} must be specified.");
                        }
                        else
                        {
                            configFile = new ImportConfig();
                        }

                        if (sourceFile != null)
                            configFile.OpenApiSpecificationFile = sourceFile.FullName;
                        if (islnFile != null)
                            configFile.IslnFile = islnFile.FullName;
                        if (!string.IsNullOrWhiteSpace(applicationName))
                            configFile.ApplicationName = applicationName;
                        if (!string.IsNullOrWhiteSpace(packageId))
                            configFile.PackageId = packageId;
                        if (!string.IsNullOrWhiteSpace(targetFolderId))
                            configFile.TargetFolderId = targetFolderId;
                        if (serviceType != null)
                            configFile.ServiceType = serviceType.Value;
                        if (isAzureFunctions != null)
                            configFile.IsAzureFunctions = isAzureFunctions.Value;
                        if (allowRemoval != null)
                            configFile.AllowRemoval = allowRemoval.Value;


                        if (string.IsNullOrEmpty(configFile.OpenApiSpecificationFile))
                            throw new Exception($"{OptionName(nameof(ImportConfig.OpenApiSpecificationFile))} is mandatory.");

                        if (string.IsNullOrEmpty(configFile.IslnFile))
                            throw new Exception($"{OptionName(nameof(ImportConfig.IslnFile))} mandatory.");

                        if (string.IsNullOrEmpty(configFile.ApplicationName))
                            throw new Exception($"{OptionName(nameof(ImportConfig.ApplicationName))} mandatory.");

                        var factory = new OpenApiPersistableFactory();
                        Helpers.Execute(
                            intentSolutionPath: configFile.IslnFile,
                            applicationName: configFile.ApplicationName,
                            designerName: "Services",
                            packageId: configFile.PackageId,
                            targetFolderId: configFile.TargetFolderId,
                            deleteExtra: configFile.AllowRemoval,
                            debug: debug,
                            createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                            stereotypeManagementMode: stereotypeManagementMode,
                            additionalPreconditionChecks: null,
                            getPersistables: packages => factory.GetPersistables(configFile, packages),
                            persistAdditionalMetadata: package => PersistSettings(package, configFile),
                            packageTypeId: "df45eaf6-9202-4c25-8dd5-677e9ba1e906");

                        if (configFile.ReverseEngineerImplementation)
                        {
                            Helpers.Execute(
                                intentSolutionPath: configFile.IslnFile,
                                applicationName: configFile.ApplicationName,
                                designerName: "Domain",
                                packageId: configFile.DomainPackageId,
                                targetFolderId: null,
                                deleteExtra: false,
                                debug: debug,
                                createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                                stereotypeManagementMode: stereotypeManagementMode,
                                additionalPreconditionChecks: null,
                                getPersistables: packages => factory.GetDomainPersistables(),
                                persistAdditionalMetadata: null,
                                packageTypeId: "1a824508-4623-45d9-accc-f572091ade5a");
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.LogError($"{exception.Message}");
                        return Task.FromResult(1);
                    }

                    return Task.FromResult(0);
                },
                symbols: Enumerable.Empty<IValueDescriptor>()
                    .Concat(rootCommand.Options)
                    .ToArray());

            Console.WriteLine($"{rootCommand.Name} version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

            return new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build()
                .Invoke(args);
        }

        private static bool IsValidPath(string file)
        {
            return File.Exists(file) || (file.StartsWith("http") && DoesUrlExistAsync(file));
        }

        private static bool DoesUrlExistAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void PersistSettings(PackageModelPersistable package, ImportConfig config)
        {
            if (config.SettingPersistence != SettingPersistence.None)
            {
                package.AddMetadata("open-api-import:open-api-file", config.OpenApiSpecificationFile);
                package.AddMetadata("open-api-import:add-postfixes", config.AddPostFixes.ToString().ToLower());
                package.AddMetadata("open-api-import:allow-removal", config.AllowRemoval.ToString().ToLower());
                package.AddMetadata("open-api-import:service-type", config.ServiceType.ToString());
                package.AddMetadata("open-api-import:setting-persistence", config.SettingPersistence.ToString());
            }
            else
            {
                package.RemoveMetadata("open-api-import:open-api-file");
                package.RemoveMetadata("open-api-import:add-postfixes");
                package.RemoveMetadata("open-api-import:allow-removal");
                package.RemoveMetadata("open-api-import:service-type");
                package.RemoveMetadata("open-api-import:setting-persistence");
            }
        }
    }
}