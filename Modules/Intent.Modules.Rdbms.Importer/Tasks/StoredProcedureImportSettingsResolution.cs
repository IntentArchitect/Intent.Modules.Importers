using System;
using Intent.Engine;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class StoredProcedureImportSettingsResolution : ModuleTaskBase<DatabaseImportSettingsResolutionInput>
{
    private readonly IMetadataManager _metadataManager;

    public StoredProcedureImportSettingsResolution(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.StoredProcedureImportSettingsResolution";
    public override string TaskTypeName => "Stored Procedure Import Settings Resolution";
    public override int Order => 0;

    protected override ValidationResult ValidateInputModel(DatabaseImportSettingsResolutionInput inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ApplicationId))
        {
            return ValidationResult.ErrorResult("Application Id is required.");
        }

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
        {
            return ValidationResult.ErrorResult("Package Id is required.");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseImportSettingsResolutionInput inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out var errorMessage))
        {
            throw new Exception(errorMessage);
        }

        var resolution = SettingsHelper.ResolveRepositoryImportSettings(package.FileLocation);

        // The dialog's "Inherit Database Settings" mode reads the database import's resolved values,
        // returned here so the dialog only needs the single round-trip.
        var databaseResolution = SettingsHelper.ResolveDatabaseImportSettings(package.FileLocation);

        var executionResult = new ExecuteResult();
        executionResult.Result = new RepositoryImportSettingsResolutionResult
        {
            ConnectionString = resolution.Settings.ConnectionString,
            DatabaseType = resolution.Settings.DatabaseType,
            StoredProcedureType = resolution.Settings.StoredProcedureType,
            SettingPersistence = resolution.Settings.SettingPersistence.ToString(),
            Source = resolution.Source.ToString(),
            UserLocalSettingsPath = SettingsHelper.GetRepositoryUserLocalSettingsPath(package.FileLocation),
            InheritedConnectionString = databaseResolution.Settings.ConnectionString,
            InheritedDatabaseType = databaseResolution.Settings.DatabaseType
        };

        return executionResult;
    }
}
