using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class TestConnectionInputModel
{
    public string ConnectionString { get; set; }
    public DatabaseType DatabaseType { get; set; }
}