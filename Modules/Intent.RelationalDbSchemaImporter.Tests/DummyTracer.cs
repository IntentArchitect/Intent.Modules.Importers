using Intent.Engine;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class DummyTracer : ITracing
{
    private readonly ITestOutputHelper _outputHelper;

    public DummyTracer(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public void Debug(string message)
    {
        _outputHelper.WriteLine("DEBUG: " + message);
    }

    public void Failure(Exception exception)
    {
        _outputHelper.WriteLine("FAIL: " + exception);
    }

    public void Failure(string exceptionMessage)
    {
        _outputHelper.WriteLine("FAIL: " + exceptionMessage);
    }

    public void Info(string message)
    {
        _outputHelper.WriteLine("INFO: " + message);
    }

    public void Warning(string message)
    {
        _outputHelper.WriteLine("WARN: " + message);
    }
}