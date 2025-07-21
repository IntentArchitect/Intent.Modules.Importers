using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class StoredProcListInputModel
{
    public string ConnectionString { get; set; } = null!;
    public DatabaseType DatabaseType { get; set; }
}

public class StoredProcListResultModel
{
    public Dictionary<string, string[]> StoredProcs { get; set; }
}