using System;
using Intent.Engine;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class VerifyFilePath : ModuleTaskBase<VerifyFilePathInputModel>
{
    private readonly IMetadataManager _metadataManager;

    public VerifyFilePath(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.VerifyFilePath";
    public override string TaskTypeName => "Verify File Path";

    protected override ValidationResult ValidateInputModel(VerifyFilePathInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.PathToFile))
        {
            return ValidationResult.ErrorResult("PathToFile is required");
        }

        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(VerifyFilePathInputModel inputModel)
    {
        var executionResult = new ExecuteResult();

        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return executionResult;
        }
        
        try
        {
            var targetPathToFile = inputModel.PathToFile;
            
            if (!string.IsNullOrWhiteSpace(inputModel.PathToFile) &&
                !Path.IsPathRooted(inputModel.PathToFile))
            {
                targetPathToFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(package.FileLocation)!, inputModel.PathToFile));
            }
            
            var access = FileAccessChecks.CanCreateOrWriteToFile(targetPathToFile);
            if (!access.Allowed)
            {
                executionResult.Errors.Add(access.Reason ?? "Access denied to path");
            }
            else
            {
                executionResult.Result = new { Verified = true };
            }
        }
        catch (Exception ex)
        {
            executionResult.Errors.Add($"Unexpected error verifying path: {ex.Message}");
        }

        return executionResult;
    }
}
