using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class StoredProcedureExtractorBase
{
    public abstract Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection,
        SystemObjectFilterBase systemObjectFilter,
        DataTypeMapperBase dataTypeMapper,
        IStoredProcedureAnalyzer analyzer);
}

/// <summary>
/// Default implementation for stored procedure extraction from databases
/// </summary>
internal class DefaultStoredProcedureExtractor : StoredProcedureExtractorBase
{
    public override async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection,
        SystemObjectFilterBase systemObjectFilter,
        DataTypeMapperBase dataTypeMapper,
        IStoredProcedureAnalyzer analyzer)
    {
        var storedProcedures = new List<StoredProcedureSchema>();
        
        // Combine stored procedures and functions (PostgreSQL uses functions instead of stored procedures)
        var routines = new List<DatabaseStoredProcedure>();
        
        // Add stored procedures
        if (databaseSchema.StoredProcedures != null)
        {
            routines.AddRange(databaseSchema.StoredProcedures
                .Where(sp => !systemObjectFilter.IsSystemObject(sp.SchemaOwner, sp.Name) && importFilterService.ExportStoredProcedure(sp.SchemaOwner, sp.Name)));
        }
        
        // Add functions (important for PostgreSQL)
        if (databaseSchema.Functions != null)
        {
            // Convert functions to stored procedure format for consistent handling
            var functionRoutines = databaseSchema.Functions
                .Where(func => !systemObjectFilter.IsSystemObject(func.SchemaOwner, func.Name) && importFilterService.ExportStoredProcedure(func.SchemaOwner, func.Name))
                .Cast<DatabaseStoredProcedure>(); // Functions inherit from base routine type
            
            routines.AddRange(functionRoutines);
        }

        // Apply additional filtering if specific stored procedure names are specified
        if (importFilterService.GetStoredProcedureNames().Count > 0)
        {
            var storedProcLookup = new HashSet<string>(importFilterService.GetStoredProcedureNames(), StringComparer.OrdinalIgnoreCase);
            routines = routines.Where(routine => 
                storedProcLookup.Contains(routine.Name) || 
                storedProcLookup.Contains($"{routine.SchemaOwner}.{routine.Name}"))
                .ToList();
        }

        var progressOutput = ConsoleOutput.CreateSectionProgress("Stored Procedures", routines.Count);
        
        foreach (var routine in routines)
        {
            progressOutput.OutputNext(routine.Name);
            
            var storedProcSchema = new StoredProcedureSchema
            {
                Name = routine.Name,
                Schema = routine.SchemaOwner,
                Parameters = ExtractStoredProcedureParameters(routine, dataTypeMapper),
                ResultSetColumns = await ExtractStoredProcedureResultSetAsync(routine, connection, analyzer, dataTypeMapper)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    private static List<StoredProcedureParameterSchema> ExtractStoredProcedureParameters(DatabaseStoredProcedure routine, DataTypeMapperBase dataTypeMapper)
    {
        var parameters = new List<StoredProcedureParameterSchema>();

        // DatabaseSchemaReader provides parameter information through Arguments
        foreach (var argument in routine.Arguments ?? [])
        {
            var parameterSchema = new StoredProcedureParameterSchema
            {
                Name = argument.Name ?? "",
                DataType = dataTypeMapper.GetDataTypeString(argument.DatabaseDataType),
                NormalizedDataType = dataTypeMapper.GetNormalizedDataTypeString(argument.DataType, argument.DatabaseDataType),
                IsOutputParameter = argument.Out, // DatabaseSchemaReader exposes input/output information
                MaxLength = argument.Length > 0 ? argument.Length : null,
                NumericPrecision = argument.Precision > 0 ? argument.Precision : null,
                NumericScale = argument.Scale > 0 ? argument.Scale : null
            };

            parameters.Add(parameterSchema);
        }

        return parameters;
    }

    private static async Task<List<ResultSetColumnSchema>> ExtractStoredProcedureResultSetAsync(
        DatabaseStoredProcedure routine, 
        DbConnection connection, 
        IStoredProcedureAnalyzer analyzer,
        DataTypeMapperBase dataTypeMapper)
    {
        var resultColumns = new List<ResultSetColumnSchema>();

        // Use the stored procedure analyzer for database-specific result set analysis
        var parameters = ExtractStoredProcedureParameters(routine, dataTypeMapper);
        
        try
        {
            resultColumns = await analyzer.AnalyzeResultSetAsync(routine.Name, routine.SchemaOwner, parameters);
        }
        catch (Exception)
        {
            // If analysis fails, return empty result set
            // This is common for procedures that don't return result sets or require specific parameters
        }

        return resultColumns;
    }
} 