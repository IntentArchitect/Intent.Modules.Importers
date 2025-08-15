using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class TestConnection : ModuleTaskBase<TestConnectionInputModel>
{
    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.TestConnection";
    public override string TaskTypeName => "SqlServer Database Connection Tester";

    protected override ValidationResult ValidateInputModel(TestConnectionInputModel inputModel)
    {
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(TestConnectionInputModel importModel)
    {
        var input = new ConnectionTestRequest
        {
            ConnectionString = importModel.ConnectionString,
            DatabaseType = importModel.DatabaseType
        };
        var executionResult = new ExecuteResult();
        var result = ImporterTool.Run<ConnectionTestResult>("test-connection", input);
        executionResult.Errors.AddRange(result.Errors);
        executionResult.Warnings.AddRange(result.Warnings);
        return executionResult;
    }
}