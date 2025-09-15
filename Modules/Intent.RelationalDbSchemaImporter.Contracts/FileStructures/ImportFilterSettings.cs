using System.Text.Json.Serialization;

namespace Intent.RelationalDbSchemaImporter.Contracts.FileStructures;

public class ImportFilterSettings
{
    public class FilterTypes
    {
        public const string Include = "include";
        public const string Exclude = "exclude";
    }

    [JsonPropertyName("filter_type")]
	public string FilterType { get; set; } = FilterTypes.Include;

	[JsonPropertyName("schemas")]
    public HashSet<string> Schemas { get; set; } = [];

	[JsonPropertyName("include_tables")]
	public List<ImportFilterTable> IncludeTables { get; set; } = [];

	[JsonPropertyName("include_dependant_tables")]
	public bool IncludeDependantTables { get; set; }

	[JsonPropertyName("include_views")]
	public List<ImportFilterTable> IncludeViews { get; set; } = [];
	
	[JsonPropertyName("exclude_tables")]
	public List<string> ExcludeTables { get; set; } = [];
	
	[JsonPropertyName("exclude_views")]
	public List<string> ExcludeViews { get; set; } = [];
	
	[JsonPropertyName("include_stored_procedures")]
	public List<string> IncludeStoredProcedures { get; set; } = [];
	
	[JsonPropertyName("exclude_stored_procedures")]
	public List<string> ExcludeStoredProcedures { get; set; } = [];

	[JsonPropertyName("exclude_table_columns")]
	public HashSet<string> ExcludedTableColumns { get; set; } = [];

	[JsonPropertyName("exclude_view_columns")]
	public HashSet<string> ExcludedViewColumns { get; set; } = [];
}

public class ImportFilterTable
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = null!;

	[JsonPropertyName("exclude_columns")]
	public HashSet<string> ExcludeColumns { get; set; } = [];
}