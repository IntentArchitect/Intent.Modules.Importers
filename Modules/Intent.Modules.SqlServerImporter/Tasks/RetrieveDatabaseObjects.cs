using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class RetrieveDatabaseObjects : ModuleTaskSingleInputBase<DatabaseMetadataInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.RetrieveDatabaseObjects";
    public override string TaskTypeName => "Retrieve Database Objects";

    protected override ValidationResult ValidateInputModel(DatabaseMetadataInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ConnectionString))
        {
            return ValidationResult.ErrorResult("Connection string is required");
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseMetadataInputModel importModel)
    {
        var executionResult = new ExecuteResult();
        
        var result = ImporterTool.Run<DatabaseObjectsResult>("retrieve-database-objects", importModel);
        if (result.Errors.Count == 0)
        {
            if (result.Result is null)
            {
                throw new InvalidOperationException("Result was expected");
            }
            
            executionResult.Result = new DatabaseMetadataResultModel
            {
                Tables = GetGroupedSchemaElements(result.Result.Tables),
                StoredProcedures = GetGroupedSchemaElements(result.Result.StoredProcedures),
                Views = GetGroupedSchemaElements(result.Result.Views)
            };
        }
        executionResult.Errors.AddRange(result.Errors);
        executionResult.Warnings.AddRange(result.Warnings);

        return executionResult;
    }

    private static Dictionary<string, string[]> GetGroupedSchemaElements(List<string> schemaElements)
    {
        return schemaElements.Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(s =>
            {
                var parts = s.Split(".");
                var schema = parts[0];
                var name = parts[1];
                return new { Schema = schema, Name = name };
            })
            .GroupBy(k => k.Schema, v => v.Name)
            .ToDictionary(k => k.Key, v => v.ToArray());
    }
} 