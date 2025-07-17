using System.Collections.Generic;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class ExecuteResult
{
    public object? Result { get; set; }
    public List<string> Warnings { get; private set; } = [];
    public List<string> Errors { get; private set; } = [];
}