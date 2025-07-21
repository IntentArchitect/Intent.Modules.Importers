using System.Collections.Generic;
using System.Linq;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class StoredProcList : ModuleTaskSingleInputBase<StoredProcListInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcList";
    public override string TaskTypeName => "SqlServer Stored Procedure List";

    protected override ValidationResult ValidateInputModel(StoredProcListInputModel inputModel)
    {
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