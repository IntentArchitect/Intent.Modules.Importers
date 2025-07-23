using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class StoredProcListInputModel
{
    public string ConnectionString { get; set; } = null!;
    public DatabaseType DatabaseType { get; set; }
}

public class StoredProcListResultModel
{
    public Dictionary<string, string[]> StoredProcs { get; set; } = [];
}