using Intent.Engine;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

// This is meant to run in a single-process environment, so if you need the correct output, you will need to run each container one at a time.

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