using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class DatabaseMetadataInputModel
{
    public string ConnectionString { get; set; } = null!;
    public DatabaseType DatabaseType { get; set; }
}

public class DatabaseMetadataResultModel
{
    public DatabaseMetadataSchema[] Schemas { get; set; } = [];
}

public class DatabaseMetadataSchema
{
    public string SchemaName { get; set; }
    public List<string> Tables { get; set; } = [];
    public List<string> Views { get; set; } = [];
    public List<string> StoredProcedures { get; set; } = [];
}