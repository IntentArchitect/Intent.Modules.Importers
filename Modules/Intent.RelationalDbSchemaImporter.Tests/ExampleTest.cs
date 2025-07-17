using Intent.Engine;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;
using Shouldly;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class ExampleTest
{
    public ExampleTest()
    {
        Logging.SetTracing(new DummyTracer());
        ImporterTool.SetToolDirectory(Path.GetDirectoryName(typeof(ExampleTest).Assembly.Location));
    }
    
    [Fact]
    public void TestRun()
    {
        var result = ImporterTool.Run<ConnectionTestResult>("test-connection", new ConnectionTestRequest
        {
            // This needs to use the connection string from test containers
            ConnectionString = "Server=.;Initial Catalog=Intent;Integrated Security=true;MultipleActiveResultSets=True;TrustServerCertificate=true;"
        });

        Assert.NotNull(result);
        result.Result!.IsSuccessful.ShouldBeTrue();
    }
}

public class DummyTracer : ITracing
{
    public void Debug(string message)
    {
    }

    public void Failure(Exception exception)
    {
    }

    public void Failure(string exceptionMessage)
    {
    }

    public void Info(string message)
    {
    }

    public void Warning(string message)
    {
    }
}