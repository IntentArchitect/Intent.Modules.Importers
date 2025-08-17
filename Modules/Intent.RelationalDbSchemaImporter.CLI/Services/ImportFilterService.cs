using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Contracts.FileStructures;
using Json.Schema;

namespace Intent.RelationalDbSchemaImporter.CLI.Services;

internal class ImportFilterService
{
    private readonly ImportSchemaRequest _importSchemaRequest;

    public ImportFilterService(ImportSchemaRequest importSchemaRequest)
    {
        _importSchemaRequest = importSchemaRequest;
    }

    public IReadOnlyList<string> GetStoredProcedureNames()
    {
	    return _importSchemaRequest.StoredProcNames;
    }
    
    private ImportFilterSettings? _importFilterSettings;
    public ImportFilterSettings GetImportFilterSettings()
	{
		if (_importFilterSettings is not null)
		{
			return _importFilterSettings;
		}
		
		if (string.IsNullOrWhiteSpace(_importSchemaRequest.ImportFilterFilePath))
		{
			return new ImportFilterSettings();
		}

		var jsonContent = File.ReadAllText(_importSchemaRequest.ImportFilterFilePath);
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
			PropertyNameCaseInsensitive = true
		};
		_importFilterSettings = JsonSerializer.Deserialize<ImportFilterSettings>(jsonContent, options)
			?? throw new Exception("Import filter settings are not valid.");

		return _importFilterSettings;
	}

	public bool ExportSchema(string schema)
    {
        return true;
		// GCB - This doesn't seem to actually provide any value to the filtering...
		//var settings = GetImportFilterSettings();
		//return settings.FilterType == ImportFilterSettings.FilterTypes.Exclude || settings.Schemas.Contains(schema);
	}

	public bool ExportTable(string schema, string tableName)
	{
		var settings = GetImportFilterSettings();
		if (!ExportSchema(schema))
		{
			return false;
		}
		
		if (settings.ExcludeTables.Contains(tableName) || settings.ExcludeTables.Contains($"{schema}.{tableName}"))
		{
			return false;
		}
		
		return settings.FilterType == ImportFilterSettings.FilterTypes.Exclude || 
               settings.IncludeTables.Any(x => x.Name == tableName || x.Name == $"{schema}.{tableName}");
	}

	public bool ExportView(string schema, string viewName)
	{
		var settings = GetImportFilterSettings();
		if (!ExportSchema(schema))
		{
			return false;
		}
		
		if (settings.ExcludeViews.Contains(viewName) || settings.ExcludeViews.Contains($"{schema}.{viewName}"))
		{
			return false;
		}
		
		return settings.FilterType == ImportFilterSettings.FilterTypes.Exclude || 
               settings.IncludeViews.Any(x => x.Name == viewName || x.Name == $"{schema}.{viewName}");
	}

	public bool ExportStoredProcedure(string schema, string storedProcedureName)
	{
		var settings = GetImportFilterSettings();
		if (!ExportSchema(schema))
		{
			return false;
		}
		
		if (settings.ExcludeStoredProcedures.Contains(storedProcedureName) || settings.ExcludeStoredProcedures.Contains($"{schema}.{storedProcedureName}"))
		{
			return false;
		}

		return settings.FilterType == ImportFilterSettings.FilterTypes.Exclude || 
               settings.IncludeStoredProcedures.Contains(storedProcedureName) ||
               settings.IncludeStoredProcedures.Contains($"{schema}.{storedProcedureName}");
	}
	
	public bool ExportDependantTable(string schema, string tableName)
	{
		if (!ExportSchema(schema))
		{
			return false;
		}

		var settings = GetImportFilterSettings();
		if (settings.ExcludeTables.Contains(tableName) || settings.ExcludeTables.Contains($"{schema}.{tableName}"))
		{
			return false;
		}

		return true;
	}
	
	public bool ExportTableColumn(string schema, string tableName, string colName)
	{
		var filterSettings = GetImportFilterSettings();
		var table = filterSettings.IncludeTables.FirstOrDefault(x => x.Name == tableName || x.Name == $"{schema}.{tableName}");
		return table?.ExcludeColumns.Contains(colName) != true && filterSettings.ExcludedTableColumns.Contains(colName) != true;
	}

	public bool ExportViewColumn(string schema, string viewName, string colName)
	{
		var filterSettings = GetImportFilterSettings();
		var view = filterSettings.IncludeViews.FirstOrDefault(x => x.Name == viewName || x.Name == $"{schema}.{viewName}");
		return view?.ExcludeColumns.Contains(colName) != true && filterSettings.ExcludedViewColumns.Contains(colName) != true;
	}
	
	public bool IncludeDependantTables()
	{
		var settings = GetImportFilterSettings();

		return settings.IncludeDependantTables;
	}
	
	public bool ExportTables()
	{
		return _importSchemaRequest.TypesToExport.Contains(ExportType.Table);
	}

	public bool ExportViews()
	{
		return _importSchemaRequest.TypesToExport.Contains(ExportType.View);
	}

	public bool ExportIndexes()
	{
		return _importSchemaRequest.TypesToExport.Contains(ExportType.Index);
	}
	
	public bool ExportStoredProcedures()
	{
		return _importSchemaRequest.TypesToExport.Contains(ExportType.StoredProcedure);
	}

	public bool ValidateFilterFile(out List<string> errors)
	{
		errors = [];
		
        if (string.IsNullOrWhiteSpace(_importSchemaRequest.ImportFilterFilePath))
        {
	        // If no filter file is specified then don't validate
			return true;
        }

        var jsonContent = File.ReadAllText(_importSchemaRequest.ImportFilterFilePath);
		var jsonSchema = JsonSchema.FromFile("Resources/filter-file-schema.json");

        var options = new EvaluationOptions
        {
			AddAnnotationForUnknownKeywords = true,
			OutputFormat = OutputFormat.List
        };

        var result = jsonSchema.Evaluate(JsonNode.Parse(jsonContent), options);
        if (!result.IsValid)
        {
	        var errorMessage = new StringBuilder();
	        errorMessage.AppendLine("The Import Filter File failed schema validation.");
	        foreach (var detail in result.Details.Where(d => d.HasErrors))
	        {
		        errorMessage.AppendLine($"Error at path: {detail.EvaluationPath}");
		        errorMessage.AppendLine($"Instance location: {detail.InstanceLocation}");
		        foreach (var error in detail?.Errors ?? new Dictionary<string, string>())
		        {
			        errorMessage.AppendLine($"  - {error.Key}: {error.Value}");
		        }
	        }
	        errors.Add(errorMessage.ToString());
	        return false;
        }

		return true;
    }
}