using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using DatabaseSchema = DatabaseSchemaReader.DataSchema.DatabaseSchema;

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

        // NOTE: PostgreSQL has Trigger functions which should be filtered out.
        // You can identify them by the "RETURNS TRIGGER" keyword.

        // KNOWN LIMITATION: DatabaseSchemaReader has a bug with procedure/function overloads
        // (same name but different parameters). For now, we filter out duplicates by keeping only
        // the first occurrence. This primarily affects PostgreSQL where function overloading is common.
        // TODO: Implement custom PostgreSQL query solution if this becomes a reported issue (or ask the lib author to fix).
        var uniqueRoutines = routines
            .GroupBy(r => new { Schema = r.SchemaOwner, Name = r.Name })
            .Select(g => g.First()) // Take first overload only
            .ToList();

        // Apply additional filtering if specific stored procedure names are specified
        if (importFilterService.GetStoredProcedureNames().Count > 0)
        {
            var storedProcLookup = new HashSet<string>(importFilterService.GetStoredProcedureNames(), StringComparer.OrdinalIgnoreCase);
            uniqueRoutines = uniqueRoutines.Where(routine => 
                storedProcLookup.Contains(routine.Name) || 
                storedProcLookup.Contains($"{routine.SchemaOwner}.{routine.Name}"))
                .ToList();
        }

        var progressOutput = ConsoleOutput.CreateSectionProgress("Stored Procedures", uniqueRoutines.Count);
        
        foreach (var routine in uniqueRoutines)
        {
            progressOutput.OutputNext(routine.Name);
            
            var storedProcSchema = new StoredProcedureSchema
            {
                Name = routine.Name,
                Schema = routine.SchemaOwner,
                Parameters = ExtractStoredProcedureParameters(databaseSchema, routine, dataTypeMapper),
                ResultSetColumns = await ExtractStoredProcedureResultSetAsync(databaseSchema, routine, analyzer, dataTypeMapper)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    private static List<StoredProcedureParameterSchema> ExtractStoredProcedureParameters(DatabaseSchema databaseSchema, DatabaseStoredProcedure routine,
        DataTypeMapperBase dataTypeMapper)
    {
        var parameters = new List<StoredProcedureParameterSchema>();

        // DatabaseSchemaReader provides parameter information through Arguments
        foreach (var argument in routine.Arguments?.DistinctBy(x => x.Name) ?? [])
        {
            var udt = ExtractUserDefinedTableType(databaseSchema, argument, dataTypeMapper);
            
            var parameterSchema = new StoredProcedureParameterSchema
            {
                Name = argument.Name ?? "",
                DbDataType = udt is not null ? argument.DatabaseDataType?.Replace("\"", string.Empty) 
                                               ?? "" : dataTypeMapper.GetDbDataTypeString(argument.DatabaseDataType),
                LanguageDataType = udt is not null ? "class" : dataTypeMapper.GetLanguageDataTypeString(argument.DataType, argument.DatabaseDataType),
                IsOutputParameter = argument.Out, // DatabaseSchemaReader exposes input/output information
                MaxLength = argument.Length > 0 ? argument.Length : null,
                NumericPrecision = argument.Precision > 0 ? argument.Precision : null,
                NumericScale = argument.Scale > 0 ? argument.Scale : null,
                UserDefinedTableType = udt
            };

            parameters.Add(parameterSchema);
        }

        return parameters;
    }

    private static UserDefinedTableTypeSchema? ExtractUserDefinedTableType(DatabaseSchema databaseSchema, DatabaseArgument argument, DataTypeMapperBase dataTypeMapper)
    {
        if (!TryGetUserDefinedTable(databaseSchema, argument, out var foundUdt))
        {
            return null;
        }

        var udtSchema = new UserDefinedTableTypeSchema
        {
            Name = foundUdt.Name,
            Schema = foundUdt.SchemaOwner ?? "",
            Columns = foundUdt.Columns.Select(col => new ColumnSchema
            {
                Name = col.Name,
                DbDataType = col.DbDataType,
                LanguageDataType = dataTypeMapper.GetLanguageDataTypeString(col.DataType, col.DbDataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = col.IsPrimaryKey,
                IsIdentity = col.IsAutoNumber,
                MaxLength = col.Length > 0 ? col.Length : null,
                NumericPrecision = col.Precision > 0 ? col.Precision : null,
                NumericScale = col.Scale > 0 ? col.Scale : null
            }).ToList()
        };

        return udtSchema;
    }

    private static bool TryGetUserDefinedTable(DatabaseSchema databaseSchema, DatabaseArgument argument, [NotNullWhen(true)] out UserDefinedTable? udt)
    {
        if (argument.UserDefinedTable is not null)
        {
            udt = argument.UserDefinedTable;
            return true;
        }

        udt = databaseSchema.UserDefinedTables.FirstOrDefault(x => argument.DatabaseDataType?.Replace("\"", string.Empty).StartsWith(x.Name) == true);
        return udt is not null;
    }

    private static async Task<List<ResultSetColumnSchema>> ExtractStoredProcedureResultSetAsync(DatabaseSchema databaseSchema, DatabaseStoredProcedure routine,
        IStoredProcedureAnalyzer analyzer,
        DataTypeMapperBase dataTypeMapper)
    {
        var resultColumns = new List<ResultSetColumnSchema>();

        // Use the stored procedure analyzer for database-specific result set analysis
        var parameters = ExtractStoredProcedureParameters(databaseSchema, routine, dataTypeMapper);
        
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