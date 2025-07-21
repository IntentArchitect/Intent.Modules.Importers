using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class DatabaseMetadataInputModel
{
    public string ConnectionString { get; set; } = null!;
    public DatabaseType DatabaseType { get; set; }
}

public class DatabaseMetadataResultModel
{
    public Dictionary<string, string[]> Tables { get; set; }
    public Dictionary<string, string[]> Views { get; set; }
    public Dictionary<string, string[]> StoredProcedures { get; set; }
} 