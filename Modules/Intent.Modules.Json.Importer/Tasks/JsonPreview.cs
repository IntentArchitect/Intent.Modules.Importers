using System;
using System.IO;
using System.Linq;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Intent.Modules.Json.Importer.Tasks
{
    public class JsonPreview : ModuleTaskBase<JsonPreviewInputModel>
    {
        public override string TaskTypeId => "Intent.Modules.Json.Importer.Tasks.JsonPreview";
        public override string TaskTypeName => "JSON Preview";

        protected override ValidationResult ValidateInputModel(JsonPreviewInputModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.SourceFolder))
                return ValidationResult.ErrorResult("Source folder is required.");

            if (!Directory.Exists(inputModel.SourceFolder))
                return ValidationResult.ErrorResult($"Source folder does not exist: {inputModel.SourceFolder}");

            return ValidationResult.SuccessResult();
        }

        protected override ExecuteResult ExecuteModuleTask(JsonPreviewInputModel inputModel)
        {
            var executionResult = new ExecuteResult();
            try
            {
                var root = Path.GetFullPath(inputModel.SourceFolder);
                var pattern = string.IsNullOrWhiteSpace(inputModel.Pattern) ? "**/*.json" : inputModel.Pattern!;

                var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
                matcher.AddInclude(pattern);

                // Use Execute to get relative paths from the selected root directory
                var di = new DirectoryInfo(root);
                var matchResult = matcher.Execute(new DirectoryInfoWrapper(di));
                var rootPath = root.Replace('\\', '/').TrimEnd('/');
                var rootName = rootPath.Split('/').LastOrDefault() ?? rootPath;
                var files = matchResult.Files
                    .Select(x => new
                    {
                        name = Path.GetFileName(x.Path),
                        relativePath = x.Path.Replace('\\', '/'),
                        fullPath = Path.Combine(root, x.Path).Replace('\\', '/')
                    })
                    .ToList();

                executionResult.Result = new { rootPath, rootName, files };
            }
            catch (Exception ex)
            {
                executionResult.Errors.Add($"Failed to list JSON files: {ex.Message}");
            }

            return executionResult;
        }
    }
}
