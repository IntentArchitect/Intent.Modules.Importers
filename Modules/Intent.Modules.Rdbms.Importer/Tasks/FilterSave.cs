using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Engine;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class FilterSave : ModuleTaskBase<FilterSaveInputModel>
{
    private readonly IMetadataManager _metadataManager;

    public FilterSave(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }
    
    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.FilterSave";
    public override string TaskTypeName => "SqlServer Filter Save";

    protected override ValidationResult ValidateInputModel(FilterSaveInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ImportFilterFilePath))
        {
            return ValidationResult.ErrorResult("Import filter file path is required");
        }
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        if (inputModel.FilterData == null)
        {
            return ValidationResult.ErrorResult("Filter data is required");
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(FilterSaveInputModel inputModel)
    {
        var executionResult = new ExecuteResult();

        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return executionResult;
        }
        
        try
        {
            var filePath = inputModel.ImportFilterFilePath;
            
            // Handle relative paths relative to package directory
            if (!string.IsNullOrWhiteSpace(filePath) && !Path.IsPathRooted(filePath))
            {
                var packageDirectory = Path.GetDirectoryName(package.FileLocation);
                if (!string.IsNullOrWhiteSpace(packageDirectory))
                {
                    filePath = Path.GetFullPath(Path.Combine(packageDirectory, filePath));
                }
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var jsonContent = JsonSerializer.Serialize(inputModel.FilterData, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            });

            File.WriteAllText(filePath, jsonContent);
            
            executionResult.Result = new { Success = true, FilePath = filePath };
        }
        catch (Exception ex)
        {
            executionResult.Errors.Add($"Error saving filter file: {ex.ToString()}");
            executionResult.Result = new { Success = false };
        }

        return executionResult;
    }
} 