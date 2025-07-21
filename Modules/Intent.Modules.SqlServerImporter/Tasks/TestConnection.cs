using System.Diagnostics;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class TestConnection : ModuleTaskSingleInputBase<TestConnectionInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.TestConnection";
    public override string TaskTypeName => "SqlServer Database Connection Tester";

    protected override ValidationResult ValidateInputModel(TestConnectionInputModel inputModel)
    {
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(TestConnectionInputModel importModel)
    {
        var input = new ConnectionTestRequest
        {
            ConnectionString = importModel.ConnectionString
        };
        var executionResult = new ExecuteResult();
        var result = ImporterTool.Run<ConnectionTestResult>("test-connection", input);
        executionResult.Errors.AddRange(result.Errors);
        executionResult.Warnings.AddRange(result.Warnings);
        return executionResult;
    }
}