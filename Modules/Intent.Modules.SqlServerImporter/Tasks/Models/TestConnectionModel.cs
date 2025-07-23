using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class TestConnectionInputModel
{
    public string ConnectionString { get; set; } = null!;
    public DatabaseType DatabaseType { get; set; }
}