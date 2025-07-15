using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Module;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.IArchitect.Common.Publishing;
using Intent.MetadataSynchronizer.Configuration;
using Serilog;

namespace Intent.MetadataSynchronizer;

public static class Helpers
{
    public delegate Persistables GetPersistables(IReadOnlyCollection<PackageModelPersistable> packages);

    private class DummyDomainPublisher : IDomainEventDispatcher
    {
        public Task<TResponse> Request<TResponse>(IDomainRequest<TResponse> request) => throw new NotSupportedException();
        public Task Publish(IDomainEvent @event) => Task.CompletedTask;
    }

    public static void Execute(
        string intentSolutionPath,
        string applicationName,
        string designerName,
        string packageId,
        string targetFolderId,
        bool deleteExtra,
        bool debug,
        bool createAttributesWithUnknownTypes,
        StereotypeManagementMode stereotypeManagementMode,
        Action additionalPreconditionChecks,
        GetPersistables getPersistables,
        Action<PackageModelPersistable> persistAdditionalMetadata = null,
        string packageTypeId = null)
    {
        try
        {
            DomainPublisher.Set(new DummyDomainPublisher());
            
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration = loggerConfiguration
                .WriteTo.Console()
                .Filter.ByExcluding(@event =>
                    @event.MessageTemplate.Text == "'\\' detected in path, please rather use platform agnostic '/': {path}");
            if (debug)
            {
                loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
            }
            Log.Logger = loggerConfiguration.CreateLogger();

            additionalPreconditionChecks?.Invoke();
            if (string.IsNullOrWhiteSpace(intentSolutionPath)) throw new ArgumentNullException(nameof(intentSolutionPath));
            if (string.IsNullOrWhiteSpace(applicationName)) throw new ArgumentNullException(nameof(applicationName));
            if (string.IsNullOrWhiteSpace(designerName)) throw new ArgumentNullException(nameof(designerName));
            if (!File.Exists(intentSolutionPath))
                throw new FileNotFoundException(
                    $"Could not find .isln file (absolute path {Path.GetFullPath(intentSolutionPath)})",
                    intentSolutionPath);

            Log.Information(Indentation.Get() + "Loading {@IslnFile} ({IslnAbsolutePath}) file", intentSolutionPath, Path.GetFullPath(intentSolutionPath));
            var solution = SolutionPersistable.Load(intentSolutionPath);
            if (solution == null) throw new Exception("Loaded isln file is null.");

            Log.Information(Indentation.Get() + "Loading application {@ApplicationId}", applicationName);
            var application = solution.GetApplications().SingleOrDefault(x => x.Name.Equals(applicationName, StringComparison.OrdinalIgnoreCase));
            if (application == null) throw new Exception("Application is null.");
            Log.Information(Indentation.Get() + "Loaded application {@ApplicationName}", application.Name);

            Log.Information(Indentation.Get() + "Loading designer {@DesignerId}", designerName);
            var designer = application.GetDesigners().SingleOrDefault(x => x.Name == designerName);
            if (designer == null) throw new Exception("Designer is null.");
            Log.Information(Indentation.Get() + "Loaded designer {@DesignerName}", designer.Name);

            if (string.IsNullOrWhiteSpace(packageId))
            {
                var designerPackages = designer
                    .GetPackages(includeExternal: false, packageFileOnly: true)
                    .Where(x => packageTypeId == null || x.SpecializationTypeId == packageTypeId)
                    .ToArray();
                if (designerPackages.Length != 1)
                {
                    throw new Exception($"{nameof(packageId)} must be specified when more than one package exists in the designer");
                }

                packageId = designerPackages[0].Id;
            }

            Log.Information(Indentation.Get() + "Loading package {@PackageId}", packageId);
            var package = designer.GetPackage(packageId);
            if (package == null) throw new Exception("Package is null.");

            if (!package.IsFullyLoaded)
            {
                package.Load();
            }

            Log.Information(Indentation.Get() + "Loaded package {@PackageName}", package.Name);

            Log.Debug(Indentation.Get() + "{Parameter} is {@Value}", nameof(targetFolderId), targetFolderId);
            if (string.IsNullOrWhiteSpace(targetFolderId))
            {
                Log.Information(Indentation.Get() + "Using package as root folder");
            }
            else
            {
                Log.Information(Indentation.Get() + "Searching for target root folder with Id {Id}", targetFolderId);
                var rootDestinationFolder = package.GetElementById(targetFolderId);
                if (rootDestinationFolder == null) throw new Exception($"Could not find target folder with Id {targetFolderId}.");
                Log.Information(Indentation.Get() + "Found {Name}", ((ElementPersistable)rootDestinationFolder).Name);
            }

            Log.Information(Indentation.Get() + "Loading referenced packages");
            var referencedPackages = GetReferencedPackages(solution, application, package);

            var packages = Enumerable.Empty<PackageModelPersistable>()
                .Append(package)
                .Concat(referencedPackages)
                .ToArray();

            Log.Information(Indentation.Get() + "Acquiring metadata to synchronize from");
            Persistables persistables;
            using (new Indentation())
            {
                persistables = getPersistables(packages);
            }

            Log.Information(Indentation.Get() + "Synchronizing metadata");
            using (new Indentation())
            {
                Synchronizer.Execute(
                    targetPackage: package,
                    parentFolderId: targetFolderId ?? packageId,
                    persistables: persistables,
                    deleteExtra: deleteExtra,
                    createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                    stereotypeManagementMode: stereotypeManagementMode);
            }

            if (persistAdditionalMetadata != null) 
            {
				Log.Information(Indentation.Get() + "Saving Metadata");
				persistAdditionalMetadata(package);
			}
            Log.Information(Indentation.Get() + "Saving package");
            package.Save(MetadataNamingConvention.UseElementName);
            Log.Information(Indentation.Get() + "Package saved successfully.");
        }
        catch (Exception e)
        {
			Console.WriteLine("Error: " + e.Message.ToString());
            Console.WriteLine(".");
			if (Log.Logger != null)
            {
                Log.Fatal(e, "Exception");
                return;
            }
			Console.WriteLine(e.ToString());
		}
	}

    private static IEnumerable<PackageModelPersistable> GetReferencedPackages(
        SolutionPersistable solution,
        ApplicationPersistable application,
        PackageModelPersistable package)
    {
        InstalledModules installedModules = null;

        return package.References
            .Select(reference =>
            {
                if (reference.Module == null)
                {
                    return PackageModelPersistable.Load(reference.AbsolutePath);
                }

                installedModules ??= InstalledModules.Load(Path.Combine(application.DirectoryPath, "modules.config"));
                var installedModule = installedModules.Modules
                    .Single(x => x.ModuleId.Equals(reference.Module, StringComparison.OrdinalIgnoreCase));
                var moduleDirectory = Path.Combine(
                    path1: solution.ModulesCacheAbsolutePath,
                    path2: $"{installedModule.ModuleId}.{installedModule.Version}");
                var moduleConfiguration = XmlSerializationHelper
                    .LoadFromDirectory<ModuleConfigurationPersistable>(
                        directory: moduleDirectory,
                        filenamePattern: $"*.{ModuleConfigurationPersistable.FILE_EXTENSION}")
                    .SingleOrDefault();
                if (moduleConfiguration == null) throw new Exception($"Could not find .{ModuleConfigurationPersistable.FILE_EXTENSION} file, have Intent modules been restored? Tried looking at: {moduleDirectory}");

                return moduleConfiguration.GetPackage(reference.PackageId);
            });
    }
}