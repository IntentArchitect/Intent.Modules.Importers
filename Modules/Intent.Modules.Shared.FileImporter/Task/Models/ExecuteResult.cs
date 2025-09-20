using System.Collections.Generic;

namespace Intent.Modules.Json.Importer.Tasks.Models;

public class ExecuteResult<T>
{
    public T? Result { get; set; }
    public List<string> Warnings { get; private set; } = [];
    public List<string> Errors { get; private set; } = [];
}