using Intent.Engine;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class DatabaseImport : ModuleTaskSingleInputBase<DatabaseImportModel>
{
    private readonly IMetadataManager _metadataManager;

    public DatabaseImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseImport";
    public override string TaskTypeName => "SqlServer Database Import";

    protected override ValidationResult ValidateInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseImportModel importModel)
    {
        PrepareInputModel(importModel);

        var executionResult = new ExecuteResult();
        
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importModel);

        if (result.Errors.Count == 0)
        {
            SettingsHelper.PersistSettings(importModel);
        }
        executionResult.Errors.AddRange(result.Errors);
        executionResult.Warnings.AddRange(result.Warnings);

        return executionResult;
    }

    private void PrepareInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return;
        }

        // Making required changes for the underlying CLI tool
        
        inputModel.PackageFileName = package.FileLocation;

        if (string.IsNullOrWhiteSpace(inputModel.StoredProcedureType))
        {
            inputModel.StoredProcedureType = "Default";
        }
    }
}