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
        try
        {
            _outputHelper.WriteLine("DEBUG: " + message);
        }
        catch
        {
            // If this gets called before/after a unit test, ignore.
        }
    }

    public void Failure(Exception exception)
    {
        try
        {
            _outputHelper.WriteLine("FAIL: " + exception);
        }
        catch
        {
            // If this gets called before/after a unit test, ignore.
        }
    }

    public void Failure(string exceptionMessage)
    {
        try
        {
            _outputHelper.WriteLine("FAIL: " + exceptionMessage);
        }
        catch
        {
            // If this gets called before/after a unit test, ignore.
        }
    }

    public void Info(string message)
    {
        try
        {
            _outputHelper.WriteLine("INFO: " + message);
        }
        catch
        {
            // If this gets called before/after a unit test, ignore.
        }
    }

    public void Warning(string message)
    {
        try
        {
            _outputHelper.WriteLine("WARN: " + message);
        }
        catch
        {
            // If this gets called before/after a unit test, ignore.
        }
    }
}