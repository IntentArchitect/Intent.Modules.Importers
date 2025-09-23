using System.Collections.Generic;

namespace Intent.Modules.Json.Importer.Tasks.Models;

public class ExecuteResult<T>(T? result)
{
    public ExecuteResult() : this(default)
    {
    }

    public T? Result { get; set; } = result;
    public List<string> Warnings { get; private set; } = [];
    public List<string> Errors { get; private set; } = [];
}