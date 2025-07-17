namespace Intent.RelationalDbSchemaImporter.Contracts.Models;

public abstract class StandardResponse
{
    public List<string> Warnings { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
    
    public void AddError(string error)
    {
        Errors.Add(error);
    }
}

public class StandardResponse<TResult> : StandardResponse
{
    public TResult? Result { get; set; }
    public string? ResultType { get; set; }

    public void SetResult(TResult? model)
    {
        Result = model;
        ResultType = model?.GetType().FullName;
    }
}
