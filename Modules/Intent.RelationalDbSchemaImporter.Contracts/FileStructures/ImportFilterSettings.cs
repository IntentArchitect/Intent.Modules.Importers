using System.Text.Json.Serialization;

namespace Intent.RelationalDbSchemaImporter.Contracts.FileStructures;

public class ImportFilterSettings
{
	[JsonPropertyName("schemas")]
	public HashSet<string> Schemas { get; set; } = new();

	[JsonPropertyName("include_tables")]
	public List<ImportFilterTable> IncludeTables { get; set; } = new();

	[JsonPropertyName("include_dependant_tables")]
	public bool IncludeDependantTables = false;

	[JsonPropertyName("include_views")]
	public List<ImportFilterTable> IncludeViews { get; set; } = new();
	
	[JsonPropertyName("exclude_tables")]
	public List<string> ExcludeTables { get; set; } = new();
	
	[JsonPropertyName("exclude_views")]
	public List<string> ExcludeViews { get; set; } = new();
	
	[JsonPropertyName("include_stored_procedures")]
	public List<string> IncludeStoredProcedures { get; set; } = new();
	
	[JsonPropertyName("exclude_stored_procedures")]
	public List<string> ExcludeStoredProcedures { get; set; } = new();

	[JsonPropertyName("exclude_table_columns")]
	public HashSet<string> ExcludedTableColumns { get; set; } = new();

	[JsonPropertyName("exclude_view_columns")]
	public HashSet<string> ExcludedViewColumns { get; set; } = new();
}

public class ImportFilterTable
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("exclude_columns")]
	public HashSet<string> ExcludeColumns { get; set; } = new();
}