using System.Diagnostics;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.MetadataSynchronizer.CSharp.Importer;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.MetadataSynchronizer.CSharp.Importer.Program;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.CSharp.Importer.Tasks
{
    public class ImportCSharpFilesTask : ModuleTaskBase<ImportCSharpFileInputModel, int>
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public ImportCSharpFilesTask(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        public override string TaskTypeId => "Intent.CSharp.Importer.ImportCSharpFilesTask";
        public override string TaskTypeName => "Import C# Files";

        protected override ValidationResult ValidateInputModel(ImportCSharpFileInputModel inputModel)
        {
            return ValidationResult.SuccessResult();
        }

        protected override ExecuteResult<int> ExecuteModuleTask(ImportCSharpFileInputModel importModel)
        {
            Debugger.Launch();
            var importedTypes = CSharpCodeAnalyzer.ImportMetadataFromFiles(importModel.SelectedFiles).GetAwaiter().GetResult();

            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(importModel.DesignerId);
            var targetPackage = designer.GetPackage(importModel.PackageId);

            var profile = GetProfile(importModel.ImportProfileId, designer);

            targetPackage.ImportCSharpTypes(importedTypes, new CSharpConfig()
            {
                ImportProfile = profile,
                TargetFolder = importModel.SourceFolder,
                TargetFolderId = importModel.TargetFolderId
            });

            targetPackage.Save();

            return new ExecuteResult<int>();
        }

        private ImportProfileConfig GetProfile(string identifier, IApplicationDesignerPersistable designer)
        {
            var profiles = new[] {
                new ImportProfileConfig()
                {
                    Identifier = "domain-classes",
                    MapClassesTo = designer.GetElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10"),
                    MapPropertiesTo = designer.GetElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf"),
                    MapAssociationsTo = designer.GetAssociationSettings("eaf9ed4e-0b61-4ac1-ba88-09f912c12087"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "domain-enums",
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo= designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "domain-events",
                    MapClassesTo = designer.GetElementSettings("0814e459-fb9b-47db-b7eb-32ce30397e8a"),
                    MapPropertiesTo = designer.GetElementSettings("b4d69073-5abb-4968-b41b-545b2f7408ed"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "domain-contracts",
                    MapClassesTo = designer.GetElementSettings("4464fabe-c59e-4d90-81fc-c9245bdd1afd"),
                    MapPropertiesTo = designer.GetElementSettings("8ebd48a9-eae7-451b-92de-22b6c8ee838c"),
                },
            };

            return profiles.SingleOrDefault(x => x.Identifier == identifier) ?? throw new Exception($"No profile with id '{identifier ?? "null"}' exists.");
        }
    }
}