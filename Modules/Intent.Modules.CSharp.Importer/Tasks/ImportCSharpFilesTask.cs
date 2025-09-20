using System.Diagnostics;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.MetadataSynchronizer.CSharp.CLI;
using Intent.Modules.CSharp.Importer.Tasks.Model;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.MetadataSynchronizer.CSharp.CLI.Program;

[assembly: DefaultIntentManaged(Mode.Fully)]
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
            
            var persistables = PersistableFactory.GetPersistables(new CSharpConfig()
            {
                ImportProfile = new ImportProfileConfig()
                {
                    //MapClassesTo = designer.GetElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10"),
                    //MapPropertiesTo = designer.GetElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf"),
                    //MapAssociationsTo = designer.GetAssociationSettings("0a66489f-30aa-417b-a75d-b945863366fd"),
                    MapClassesTo = new ElementSetting("04e12b51-ed12-42a3-9667-a6aa81bb6d10", "Class"),
                    MapPropertiesTo = new ElementSetting("0090fb93-483e-41af-a11d-5ad2dc796adf", "Attribute"),
                    MapAssociationsTo = new AssociationSetting(specializationTypeId: "0a66489f-30aa-417b-a75d-b945863366fd",
                        specializationType: "Association",
                        sourceEnd: new AssociationEndSetting("8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58", "Association Source End"),
                        targetEnd: new AssociationEndSetting("eaf9ed4e-0b61-4ac1-ba88-09f912c12087", "Association Target End")),
                },
                TargetFolder = importModel.SourceFolder,
                TargetFolderId = importModel.TargetFolderId
            }, importedTypes, targetPackage);

            targetPackage.Save();

            return new ExecuteResult<int>();
        }
    }
}