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
            //Debugger.Launch();
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
                //----------------- UNIVERSAL -----------------//
                new ImportProfileConfig()
                {
                    Identifier = "type-definition",
                    MapClassesTo = designer.GetElementSettings("d4e577cd-ad05-4180-9a2e-fff4ddea0e1e"),
                    MapInterfacesTo = designer.GetElementSettings("d4e577cd-ad05-4180-9a2e-fff4ddea0e1e"),
                },

                //----------------- DOMAIN -----------------//
                new ImportProfileConfig()
                {
                    Identifier = "domain-classes",
                    MapClassesTo = designer.GetElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10"),
                    MapInheritanceTo = designer.GetAssociationSettings("5de35973-3ac7-4e65-b48c-385605aec561"),
                    MapPropertiesTo = designer.GetElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf"),
                    MapAssociationsTo = designer.GetAssociationSettings("eaf9ed4e-0b61-4ac1-ba88-09f912c12087"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo= designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
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
                    MapInheritanceTo = designer.GetAssociationSettings("fa57ec52-536d-46a8-8aa0-4589812665c1"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo= designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "domain-contracts",
                    MapClassesTo = designer.GetElementSettings("4464fabe-c59e-4d90-81fc-c9245bdd1afd"),
                    MapPropertiesTo = designer.GetElementSettings("8ebd48a9-eae7-451b-92de-22b6c8ee838c"),
                    MapInheritanceTo = designer.GetAssociationSettings("4199ae15-0ecc-4086-82f3-bfa885c9d3e8"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo= designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },

                //----------------- SERVICES -----------------//
                new ImportProfileConfig()
                {
                    Identifier = "services-services",
                    MapClassesTo = designer.GetElementSettings("b16578a5-27b1-4047-a8df-f0b783d706bd"),
                    MapInterfacesTo = designer.GetElementSettings("b16578a5-27b1-4047-a8df-f0b783d706bd"),
                    MapMethodsTo = designer.GetElementSettings("e030c97a-e066-40a7-8188-808c275df3cb"),
                    MapMethodParametersTo = designer.GetElementSettings("00208d20-469d-41cb-8501-768fd5eb796b"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "services-commands",
                    MapClassesTo = designer.GetElementSettings("ccf14eb6-3a55-4d81-b5b9-d27311c70cb9"),
                    MapPropertiesTo = designer.GetElementSettings("7baed1fd-469b-4980-8fd9-4cefb8331eb2"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "services-queries",
                    MapClassesTo = designer.GetElementSettings("e71b0662-e29d-4db2-868b-8a12464b25d0"),
                    MapPropertiesTo = designer.GetElementSettings("7baed1fd-469b-4980-8fd9-4cefb8331eb2"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "services-dtos",
                    MapClassesTo = designer.GetElementSettings("fee0edca-4aa0-4f77-a524-6bbd84e78734"),
                    MapPropertiesTo = designer.GetElementSettings("7baed1fd-469b-4980-8fd9-4cefb8331eb2"),
                    MapInheritanceTo = designer.GetAssociationSettings("5ba12bbf-122f-4c3e-af3c-4a88dc554597"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "services-enums",
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },

                //----------------- EVENTING -----------------//
                new ImportProfileConfig()
                {
                    Identifier = "eventing-integration-messages",
                    MapClassesTo = designer.GetElementSettings("cbe970af-5bad-4d92-a3ed-a24b9fdaa23e"),
                    MapPropertiesTo = designer.GetElementSettings("bde29850-5fb9-4f47-9941-b9e182fd9bdc"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "eventing-integration-commands",
                    MapClassesTo = designer.GetElementSettings("7f01ca8e-0e3c-4735-ae23-a45169f71625"),
                    MapPropertiesTo = designer.GetElementSettings("bde29850-5fb9-4f47-9941-b9e182fd9bdc"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
                new ImportProfileConfig()
                {
                    Identifier = "eventing-dtos",
                    MapClassesTo = designer.GetElementSettings("544f1d57-27ce-4985-a4ec-cc01568d72b0"),
                    MapPropertiesTo = designer.GetElementSettings("93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7"),
                    MapInheritanceTo = designer.GetAssociationSettings("ccf59371-009d-44dd-9417-a907b463b223"),
                    MapEnumsTo = designer.GetElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb"),
                    MapEnumLiteralsTo = designer.GetElementSettings("4215f417-25d2-4509-9309-5076a1452eaa"),
                },
            }.ToDictionary(x => x.Identifier);

            profiles["domain-events"].DependencyProfile = profiles["domain-contracts"];
            profiles["services-commands"].DependencyProfile = profiles["services-dtos"];
            profiles["services-queries"].DependencyProfile = profiles["services-dtos"];
            profiles["services-services"].DependencyProfile = profiles["services-dtos"];
            profiles["eventing-integration-messages"].DependencyProfile = profiles["eventing-dtos"];
            profiles["eventing-integration-commands"].DependencyProfile = profiles["eventing-dtos"];

            return profiles.TryGetValue(identifier, out var result) ? result : throw new Exception($"No profile with id '{identifier ?? "null"}' exists.");
        }
    }
}