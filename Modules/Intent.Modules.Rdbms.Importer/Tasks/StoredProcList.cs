using System.Linq;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class StoredProcList : ModuleTaskBase<StoredProcListInputModel>
{
    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.StoredProcList";
    public override string TaskTypeName => "SqlServer Stored Procedure List";

    protected override ValidationResult ValidateInputModel(StoredProcListInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ConnectionString))
        {
            return ValidationResult.ErrorResult("Connection string could not be determined. Please enter a connection string before browsing stored procedures, or rerun the Database Import process and ensure the connection string is persisted.");
        }


        if (!System.Enum.IsDefined(inputModel.DatabaseType))
        {
            return ValidationResult.ErrorResult("Database Type is required to browse stored procedures. Configure the package-level Database Import Database Type first if this repository inherits settings, or select an explicit repository Database Type.");
        }

        return ValidationResult.SuccessResult();
    }


    protected override ExecuteResult ExecuteModuleTask(StoredProcListInputModel importModel)
    {
        var executionResult = new ExecuteResult();

        var input = new StoredProceduresListRequest
        {
            ConnectionString = importModel.ConnectionString,
            DatabaseType = importModel.DatabaseType
        };
        
        var result = ImporterTool.Run<StoredProceduresListResult>("list-stored-procedures", input);

        if (executionResult.Errors.Count == 0)
        {
            var resultModel = new StoredProcListResultModel
            {
                StoredProcs = result.Result!.StoredProcedures
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(s =>
                    {
                        var parts = s.Split(".");
                        var schema = parts[0];
                        var name = parts[1];
                        return new { Schema = schema, Name = name };
                    })
                    .GroupBy(k => k.Schema, v => v.Name)
                    .ToDictionary(k => k.Key, v => v.ToArray())
            };
            executionResult.Result = resultModel;
        }

        return executionResult;
    }
}
