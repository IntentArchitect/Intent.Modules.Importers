using System.Collections.Generic;

namespace Intent.SQLSchemaExtractor.Models;

public class DatabaseObjectsResult
{
    public List<string> Tables { get; set; } = new();
    public List<string> Views { get; set; } = new();
    public List<string> StoredProcedures { get; set; } = new();
}
