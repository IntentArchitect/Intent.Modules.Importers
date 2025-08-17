using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class RetrieveDatabaseObjects : ModuleTaskBase<DatabaseMetadataInputModel>
{
    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.RetrieveDatabaseObjects";
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

            var tables = GetGroupedSchemaElements(result.Result.Tables);
            var storedProcedures = GetGroupedSchemaElements(result.Result.StoredProcedures);
            var views = GetGroupedSchemaElements(result.Result.Views);

            var schemas = new Dictionary<string, DatabaseMetadataSchema>();
            foreach (var table in tables)
            {
                schemas.TryAdd(table.Key, new DatabaseMetadataSchema() { SchemaName = table.Key });
                schemas[table.Key].Tables.AddRange(table.Value);
            }
            foreach (var table in storedProcedures)
            {
                schemas.TryAdd(table.Key, new DatabaseMetadataSchema() { SchemaName = table.Key });
                schemas[table.Key].StoredProcedures.AddRange(table.Value);
            }

            foreach (var table in views)
            {
                schemas.TryAdd(table.Key, new DatabaseMetadataSchema() { SchemaName = table.Key });
                schemas[table.Key].Views.AddRange(table.Value);
            }

            executionResult.Result = new DatabaseMetadataResultModel
            {
                Schemas = schemas.Values.ToArray()
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