using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.CSharp.Importer.Tasks
{
    public class ImportCSharpFilesTask : IModuleTask
    {
        public ImportCSharpFilesTask()
        {
        }

        public string TaskTypeId { get; set; } = "Intent.CSharp.Importer.ImportCSharpFilesTask";
        public string TaskTypeName { get; set; } = "Import c sharp files";
        public int Order { get; set; } = 0;

        public string Execute(params string[] args)
        {
            // TODO: Implement ImportCSharpFilesTask.Execute(...) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }

    }
}