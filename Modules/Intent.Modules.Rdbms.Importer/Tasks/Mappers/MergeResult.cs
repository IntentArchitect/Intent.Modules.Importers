using System;
using System.Collections.Generic;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

internal class MergeResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public List<string> Warnings { get; set; } = [];
}