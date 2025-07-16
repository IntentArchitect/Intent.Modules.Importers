using System;
using System.Collections.Generic;

namespace Intent.SQLSchemaExtractor.Models;

public class StandardResponse
{
    public object? Result { get; private set; }
    public string? ResultType { get; private set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();

    public void SetResult(object model)
    {
        Result = model;
        ResultType = model.GetType().FullName;
    }
    
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
    
    public void AddError(string error)
    {
        Errors.Add(error);
    }
}
