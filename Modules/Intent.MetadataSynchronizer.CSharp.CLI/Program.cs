using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.CSharp.CLI;

class Program
{
    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";
    
    static async Task<int> Main(string[] args)
    {
        var configFileOption = new Option<FileInfo?>(
            name: GetOptionName(nameof(CSharpConfig.ConfigFile)),
            description: "Path to a JSON formatted file containing options to use for execution of " +
                         "this tool as an alternative to using command line options. The " +
                         $"{GetOptionName(nameof(CSharpConfig.GenerateConfigFile))} option can be used to " +
                         "generate a file with all the possible fields populated with null.");
        var generateConfigFileOption = new Option<bool>(
            name: GetOptionName(nameof(CSharpConfig.GenerateConfigFile)),
            description: $"Scaffolds into the current working directory a \"config.json\" for use with the " +
                         $"{GetOptionName(nameof(CSharpConfig.ConfigFile))} option.",
            getDefaultValue: () => false);

        var domainEntitiesFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.DomainEntitiesFolder)),
            description: "The physical folder path to scan for C# classes/records representing Domain Entities.");
        var domainEnumsFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.DomainEnumsFolder)),
            description: "The physical folder path to scan for C# enums representing Domain Enums.");
        var domainServicesFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.DomainServicesFolder)),
            description: "The physical folder path to scan for C# classes/records representing Domain Services.");
        var domainRepositoriesFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.DomainRepositoriesFolder)),
            description: "The physical folder path to scan for C# classes/records representing Domain Repositories.");
        var domainDataContractsFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.DomainDataContractsFolder)),
            description: "The physical folder path to scan for C# classes/records representing Domain Data Contracts.");
        var serviceEnumsFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.ServiceEnumsFolder)),
            description: "The physical folder path to scan for C# enums representing Service Enums.");
        var serviceDtosFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.ServiceDtosFolder)),
            description: "The physical folder path to scan for C# classes/records representing Service DTOs.");
        var valueObjectsFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.ValueObjectsFolder)),
            description: "The physical folder path to scan for C# classes/records representing Value Objects.");
        var eventMessagesFolderOption = new Option<DirectoryInfo?>(
            name: GetOptionName(nameof(CSharpConfig.EventMessagesFolder)),
            description: "The physical folder path to scan for C# classes/records representing Event Messages and Event DTOs.");

        var islnFileOption = new Option<FileInfo?>(
            name: GetOptionName(nameof(CSharpConfig.IslnFile)),
            description: "The Intent Architect solution (.isln) file containing the Intent Architect Application " +
                         "into which to synchronize the metadata.");
        var applicationNameOption = new Option<string?>(
            name: GetOptionName(nameof(CSharpConfig.ApplicationName)),
            description: "The name of the Intent Architect Application (as per the Application Settings view) " +
                         "containing the Intent Architect Package into which to synchronize the metadata.");
        var packageIdOption = new Option<string?>(
            name: GetOptionName(nameof(CSharpConfig.PackageId)),
            description: "The id of the Intent Architect Package containing the Intent Architect Package into " +
                         "which to synchronize the metadata.");
        var designerNameOption = new Option<string?>(
            name: GetOptionName(nameof(CSharpConfig.DesignerName)),
            description: @"The name of the Designer where the package is located. i.e. ""Domain"" or ""Services""");
        var targetFolderIdOption = new Option<string?>(
            name: GetOptionName(nameof(CSharpConfig.TargetFolderId)),
            description: "The target folder within the Intent Architect package into which to synchronize the " +
                         "metadata. If unspecified then the metadata will be synchronized into the root of the " +
                         "Intent Architect package.");

        var debugOption = new Option<bool>(
            name: GetOptionName(nameof(CSharpConfig.Debug)),
            description: "Verbose logging.",
            getDefaultValue: () => false);
        
        var rootCommand = new RootCommand("The Intent C# Metadata Synchronizer CLI tool can be used to synchronize a folder " +
                                          "of .cs Class files into an Intent Architect Document DB Domain Package.")
        {
            configFileOption, 
            generateConfigFileOption,
            domainEntitiesFolderOption,
            domainEnumsFolderOption,
            domainServicesFolderOption,
            domainRepositoriesFolderOption,
            domainDataContractsFolderOption,
            serviceEnumsFolderOption,
            serviceDtosFolderOption,
            valueObjectsFolderOption,
            eventMessagesFolderOption,
            islnFileOption,
            applicationNameOption,
            packageIdOption,
            designerNameOption,
            targetFolderIdOption,
            debugOption
        };

        rootCommand.SetHandler(async context =>
        {
            try
            {
                if (context.ParseResult.GetValueForOption(generateConfigFileOption))
                {
                    var path = Path.Join(Environment.CurrentDirectory, "config.json");
                    Console.WriteLine($"Writing {path}...");
                    await File.WriteAllBytesAsync(path, JsonSerializer.SerializeToUtf8Bytes(new CSharpConfig(), new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
                    Console.WriteLine("Done.");
                    return;
                }

                CSharpConfig csConfig;
                var configFile = context.ParseResult.GetValueForOption(configFileOption);
                if (configFile is not null)
                {
                    csConfig = JsonSerializer.Deserialize<CSharpConfig>(await File.ReadAllTextAsync(configFile.FullName),
                                   new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip })
                               ?? throw new Exception($"Parsing of \"{configFile.FullName}\" returned null.");
                }
                else
                {
                    csConfig = new CSharpConfig
                    {
                        DomainEntitiesFolder = context.ParseResult.GetValueForOption(domainEntitiesFolderOption)?.FullName.NormalizePath(),
                        DomainEnumsFolder = context.ParseResult.GetValueForOption(domainEnumsFolderOption)?.FullName.NormalizePath(),
                        DomainServicesFolder = context.ParseResult.GetValueForOption(domainServicesFolderOption)?.FullName.NormalizePath(),
                        DomainRepositoriesFolder = context.ParseResult.GetValueForOption(domainRepositoriesFolderOption)?.FullName.NormalizePath(),
                        DomainDataContractsFolder = context.ParseResult.GetValueForOption(domainDataContractsFolderOption)?.FullName.NormalizePath(),
                        ServiceEnumsFolder = context.ParseResult.GetValueForOption(serviceEnumsFolderOption)?.FullName.NormalizePath(),
                        ServiceDtosFolder = context.ParseResult.GetValueForOption(serviceDtosFolderOption)?.FullName.NormalizePath(),
                        ValueObjectsFolder = context.ParseResult.GetValueForOption(valueObjectsFolderOption)?.FullName.NormalizePath(),
                        EventMessagesFolder = context.ParseResult.GetValueForOption(eventMessagesFolderOption)?.FullName.NormalizePath(),
                        IslnFile = context.ParseResult.GetValueForOption(islnFileOption)!.FullName.NormalizePath()!,
                        ApplicationName = context.ParseResult.GetValueForOption(applicationNameOption)!,
                        PackageId = context.ParseResult.GetValueForOption(packageIdOption)!,
                        DesignerName = context.ParseResult.GetValueForOption(designerNameOption)!,
                        TargetFolderId = context.ParseResult.GetValueForOption(targetFolderIdOption),
                        Debug = context.ParseResult.GetValueForOption(debugOption)
                    };
                }

                csConfig.Validate();

                await ExecuteImportProcess(csConfig);
            }
            catch (ValidationException ex)
            {
                context.ExitCode = 1;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.ResetColor();
                context.HelpBuilder.Write(rootCommand, context.Console.Out.CreateTextWriter());
            }
        });

        return await new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build()
            .InvokeAsync(args);
    }

    private static async Task ExecuteImportProcess(CSharpConfig csConfig)
    {
        const bool createAttributesWithUnknownTypes = true;
        const StereotypeManagementMode stereotypeManagementMode = StereotypeManagementMode.Merge;
        var classDataElements = await CSharpCodeAnalyzer.ImportMetadataFromFolder(csConfig);

        Helpers.Execute(
            intentSolutionPath: csConfig.IslnFile,
            applicationName: csConfig.ApplicationName,
            designerName: csConfig.DesignerName,
            packageId: csConfig.PackageId,
            targetFolderId: csConfig.TargetFolderId,
            deleteExtra: false,
            debug: csConfig.Debug,
            createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
            stereotypeManagementMode: stereotypeManagementMode,
            additionalPreconditionChecks: null,
            getPersistables: packages => PersistableFactory.GetPersistables(csConfig, classDataElements, packages));
    }
}