using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class DatabaseImportModel
{
	public DatabaseImportModel()
	{
	}
	
	public DatabaseImportModel(DatabaseImportModel other)
	{
		ApplicationId = other.ApplicationId;
		PackageId = other.PackageId;
		ConnectionString = other.ConnectionString;
		PackageFileName = other.PackageFileName;
		ImportFilterFilePath = other.ImportFilterFilePath;
		StoredProcedureType = other.StoredProcedureType;
		EntityNameConvention = other.EntityNameConvention;
		AttributeNameConvention = other.AttributeNameConvention;
		TableStereotype = other.TableStereotype;
		TypesToExport = other.TypesToExport?.ToList() ?? [];
		DatabaseType = other.DatabaseType;
		FilterType = other.FilterType;
		AllowDeletions = other.AllowDeletions;
		PreserveAttributeTypes = other.PreserveAttributeTypes;
	}
	
	public string ApplicationId { get; set; } = null!;
	public string PackageId { get; set; } = null!;
	public string? PackageFileName { get; set; }
	public string EntityNameConvention { get; set; } = null!;
	public string AttributeNameConvention { get; set; } = null!;
	public string TableStereotype { get; set; } = null!;
	public string? StoredProcedureType { get; set; }
	public string FilterType { get; set; } = "include";
	public DatabaseSettingPersistence SettingPersistence { get; set; } = DatabaseSettingPersistence.None;
	public bool AllowDeletions { get; set; } = true;
	public bool PreserveAttributeTypes { get; set; } = true;
	
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