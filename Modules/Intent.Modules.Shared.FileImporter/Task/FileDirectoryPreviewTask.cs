using System;
using System.IO;
using System.Linq;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Intent.Modules.Json.Importer.Tasks
{
    public class FileDirectoryPreviewTask : ModuleTaskBase<FilePreviewInputModel, DirectoryPreview>
    {
        public override string TaskTypeId => "Intent.Modules.Importer.FileDirectoryPreviewTask";
        public override string TaskTypeName => "File Preview";

        protected override ValidationResult ValidateInputModel(FilePreviewInputModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.SourceFolder))
                return ValidationResult.ErrorResult("Source folder is required.");

            if (!Directory.Exists(inputModel.SourceFolder))
                return ValidationResult.ErrorResult($"Source folder does not exist: {inputModel.SourceFolder}");

            return ValidationResult.SuccessResult();
        }

        protected override ExecuteResult<DirectoryPreview> ExecuteModuleTask(FilePreviewInputModel inputModel)
        {
            var executionResult = new ExecuteResult<DirectoryPreview>();
            try
            {
                var root = Path.GetFullPath(inputModel.SourceFolder);
                var pattern = !string.IsNullOrWhiteSpace(inputModel.Pattern) ? inputModel.Pattern! : throw new Exception("File matching pattern is required.");

                var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
                matcher.AddInclude(pattern);

                // Use Execute to get relative paths from the selected root directory
                var di = new DirectoryInfo(root);
                var matchResult = matcher.Execute(new DirectoryInfoWrapper(di));
                var rootPath = root.Replace('\\', '/').TrimEnd('/');
                var rootName = rootPath.Split('/').LastOrDefault() ?? rootPath;
                var files = matchResult.Files
                    .Select(x => new FilePreview(Path.GetFileName(x.Path), x.Path.Replace('\\', '/'), Path.Combine(root, x.Path).Replace('\\', '/')))
                    .ToArray();

                executionResult.Result = new DirectoryPreview(rootPath, rootName, files);
            }
            catch (Exception ex)
            {
                executionResult.Errors.Add($"Failed to list JSON files: {ex.Message}");
            }

            return executionResult;
        }
    }
}
