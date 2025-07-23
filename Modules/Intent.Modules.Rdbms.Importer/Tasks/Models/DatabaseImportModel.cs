using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class DatabaseImportModel
{
	public string ApplicationId { get; set; } = null!;
	public string PackageId { get; set; } = null!;
	public string? PackageFileName { get; set; }
	public string EntityNameConvention { get; set; } = null!;
	public string TableStereotype { get; set; } = null!;
	public string? StoredProcedureType { get; set; }
	public DatabaseSettingPersistence SettingPersistence { get; set; } = DatabaseSettingPersistence.None;
	
	// BEGIN - ImportSchemaRequest
	public string ConnectionString { get; set; } = null!;
	public string ImportFilterFilePath { get; set; } = null!;
	public List<string> TypesToExport { get; set; } = [];
	public DatabaseType DatabaseType { get; set; }
	// END - ImportSchemaRequest
}

public enum DatabaseSettingPersistence
{
	None,
	AllSanitisedConnectionString,
	AllWithoutConnectionString,
	All
}