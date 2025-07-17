using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Microsoft.SqlServer.Management.Smo;

namespace Intent.SQLSchemaExtractor.Extractors;

public class DatabaseSchemaExtractor
{
    private readonly Database _db;
    private readonly ImportConfiguration _config;
    private readonly List<string> _tablesToIgnore = ["sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory"];
    private readonly List<string> _viewsToIgnore = [];

    public DatabaseSchemaExtractor(ImportConfiguration config, Database db)
    {
        _db = db;
        _config = config;
    }

    public DatabaseSchema ExtractSchema()
    {
        var schema = new DatabaseSchema
        {
            DatabaseName = _db.Name,
            Tables = [],
            Views = [],
            StoredProcedures = [],
            UserDefinedTableTypes = []
        };

        if (_config.ExportTables())
        {
            schema.Tables = ExtractTables();
        }

        if (_config.ExportViews())
        {
            schema.Views = ExtractViews();
        }

        if (_config.ExportStoredProcedures())
        {
            schema.StoredProcedures = ExtractStoredProcedures();
        }

        return schema;
    }

    private List<TableSchema> ExtractTables()
    {
        var tables = new List<TableSchema>();
        var filteredTables = GetFilteredTables();

        foreach (var table in filteredTables)
        {
            var tableSchema = new TableSchema
            {
                Name = table.Name,
                Schema = table.Schema,
                Columns = ExtractTableColumns(table),
                Indexes = ExtractTableIndexes(table),
                ForeignKeys = ExtractTableForeignKeys(table),
                Triggers = ExtractTableTriggers(table)
            };

            tables.Add(tableSchema);
        }

        return tables;
    }

    private List<ViewSchema> ExtractViews()
    {
        var views = new List<ViewSchema>();
        var filteredViews = GetFilteredViews();

        foreach (var view in filteredViews)
        {
            var viewSchema = new ViewSchema
            {
                Name = view.Name,
                Schema = view.Schema,
                Columns = ExtractViewColumns(view),
                Triggers = ExtractViewTriggers(view)
            };

            views.Add(viewSchema);
        }

        return views;
    }

    private List<StoredProcedureSchema> ExtractStoredProcedures()
    {
        var storedProcedures = new List<StoredProcedureSchema>();
        var filteredStoredProcedures = GetFilteredStoredProcedures();

        foreach (var storedProc in filteredStoredProcedures)
        {
            if (!_config.ExportStoredProcedure(storedProc.Schema, storedProc.Name))
            {
                continue;
            }

            if (_config.StoredProcNames?.Count > 0)
            {
                var storedProcLookup = new HashSet<string>(_config.StoredProcNames, StringComparer.OrdinalIgnoreCase);
                if (!storedProcLookup.Contains(storedProc.Name) && !storedProcLookup.Contains($"{storedProc.Schema}.{storedProc.Name}"))
                {
                    continue;
                }
            }

            var storedProcSchema = new StoredProcedureSchema
            {
                Name = storedProc.Name,
                Schema = storedProc.Schema,
                Parameters = ExtractStoredProcedureParameters(storedProc),
                ResultSetColumns = ExtractStoredProcedureResultSet(storedProc)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    private List<ColumnSchema> ExtractTableColumns(Table table)
    {
        var columns = new List<ColumnSchema>();

        foreach (Column col in table.Columns)
        {
            if (!_config.ExportTableColumn(table.Schema, table.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = GetDataTypeString(col.DataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = col.InPrimaryKey,
                IsIdentity = col.Identity,
                MaxLength = GetMaxLength(col.DataType),
                NumericPrecision = GetNumericPrecision(col.DataType),
                NumericScale = GetNumericScale(col.DataType),
                DefaultConstraint = ExtractDefaultConstraint(col),
                ComputedColumn = ExtractComputedColumn(col)
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    private List<ColumnSchema> ExtractViewColumns(View view)
    {
        var columns = new List<ColumnSchema>();

        foreach (Column col in view.Columns)
        {
            if (!_config.ExportViewColumn(view.Schema, view.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = GetDataTypeString(col.DataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = false, // Views don't have primary keys
                IsIdentity = false, // Views don't have identity columns
                MaxLength = GetMaxLength(col.DataType),
                NumericPrecision = GetNumericPrecision(col.DataType),
                NumericScale = GetNumericScale(col.DataType),
                DefaultConstraint = null, // Views don't have default constraints
                ComputedColumn = null // Views don't have computed columns in this context
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    private List<IndexSchema> ExtractTableIndexes(Table table)
    {
        var indexes = new List<IndexSchema>();

        if (!_config.ExportIndexes())
        {
            return indexes;
        }

        foreach (Microsoft.SqlServer.Management.Smo.Index index in table.Indexes)
        {
            if (index.IsClustered)
            {
                continue; // Skip clustered indexes for now
            }

            var indexSchema = new IndexSchema
            {
                Name = index.Name,
                IsUnique = index.IsUnique,
                IsClustered = index.IsClustered,
                HasFilter = index.HasFilter,
                FilterDefinition = index.HasFilter ? index.FilterDefinition : null,
                Columns = ExtractIndexColumns(index)
            };

            indexes.Add(indexSchema);
        }

        return indexes;
    }

    private List<IndexColumnSchema> ExtractIndexColumns(Microsoft.SqlServer.Management.Smo.Index index)
    {
        var columns = new List<IndexColumnSchema>();

        foreach (IndexedColumn indexedColumn in index.IndexedColumns)
        {
            var columnSchema = new IndexColumnSchema
            {
                Name = indexedColumn.Name,
                IsDescending = indexedColumn.Descending,
                IsIncluded = indexedColumn.IsIncluded
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    private List<ForeignKeySchema> ExtractTableForeignKeys(Table table)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        foreach (ForeignKey foreignKey in table.ForeignKeys)
        {
            var foreignKeySchema = new ForeignKeySchema
            {
                Name = foreignKey.Name,
                ReferencedTableSchema = foreignKey.ReferencedTableSchema,
                ReferencedTableName = foreignKey.ReferencedTable,
                Columns = ExtractForeignKeyColumns(foreignKey)
            };

            foreignKeys.Add(foreignKeySchema);
        }

        return foreignKeys;
    }

    private List<ForeignKeyColumnSchema> ExtractForeignKeyColumns(ForeignKey foreignKey)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        foreach (ForeignKeyColumn fkColumn in foreignKey.Columns)
        {
            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = fkColumn.Name,
                ReferencedColumnName = fkColumn.ReferencedColumn
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    private List<TriggerSchema> ExtractTableTriggers(Table table)
    {
        var triggers = new List<TriggerSchema>();

        foreach (Trigger trigger in table.Triggers)
        {
            var triggerSchema = new TriggerSchema
            {
                Name = trigger.Name,
                ParentSchema = table.Schema,
                ParentName = table.Name,
                ParentType = "Table"
            };

            triggers.Add(triggerSchema);
        }

        return triggers;
    }

    private List<TriggerSchema> ExtractViewTriggers(View view)
    {
        var triggers = new List<TriggerSchema>();

        foreach (Trigger trigger in view.Triggers)
        {
            var triggerSchema = new TriggerSchema
            {
                Name = trigger.Name,
                ParentSchema = view.Schema,
                ParentName = view.Name,
                ParentType = "View"
            };

            triggers.Add(triggerSchema);
        }

        return triggers;
    }

    private List<StoredProcedureParameterSchema> ExtractStoredProcedureParameters(StoredProcedure storedProc)
    {
        var parameters = new List<StoredProcedureParameterSchema>();

        foreach (StoredProcedureParameter parameter in storedProc.Parameters)
        {
            var parameterSchema = new StoredProcedureParameterSchema
            {
                Name = parameter.Name,
                DataType = GetDataTypeString(parameter.DataType),
                IsOutputParameter = parameter.IsOutputParameter,
                MaxLength = GetMaxLength(parameter.DataType),
                NumericPrecision = GetNumericPrecision(parameter.DataType),
                NumericScale = GetNumericScale(parameter.DataType)
            };

            parameters.Add(parameterSchema);
        }

        return parameters;
    }

    private List<ResultSetColumnSchema> ExtractStoredProcedureResultSet(StoredProcedure storedProc)
    {
        var columns = new List<ResultSetColumnSchema>();

        try
        {
            var resultSet = StoredProcExtractor.GetStoredProcedureResultSet(_db, storedProc);
            
            foreach (var column in resultSet.Columns)
            {
                var columnSchema = new ResultSetColumnSchema
                {
                    Name = column.Name,
                    DataType = GetDataTypeString(column.SqlDataType),
                    IsNullable = column.IsNullable,
                    MaxLength = null, // Not available in ResultSetColumn
                    NumericPrecision = null, // Not available in ResultSetColumn
                    NumericScale = null // Not available in ResultSetColumn
                };

                columns.Add(columnSchema);
            }
        }
        catch (Exception ex)
        {
            Logging.LogWarning($"Could not extract result set for stored procedure {storedProc.Schema}.{storedProc.Name}: {ex.Message}");
        }

        return columns;
    }

    private DefaultConstraintSchema? ExtractDefaultConstraint(Column column)
    {
        if (column.DefaultConstraint == null)
        {
            return null;
        }

        return new DefaultConstraintSchema
        {
            Text = column.DefaultConstraint.Text
        };
    }

    private ComputedColumnSchema? ExtractComputedColumn(Column column)
    {
        if (!column.Computed)
        {
            return null;
        }

        return new ComputedColumnSchema
        {
            Expression = column.ComputedText,
            IsPersisted = column.IsPersisted
        };
    }

    private string GetDataTypeString(DataType dataType)
    {
        // Return base type name without size/precision information
        return dataType.SqlDataType switch
        {
            SqlDataType.VarBinaryMax => "varbinary",
            SqlDataType.VarCharMax => "varchar", 
            SqlDataType.NVarCharMax => "nvarchar",
            _ => dataType.Name
        };
    }

    private string GetDataTypeString(SqlDataType sqlDataType)
    {
        return sqlDataType.ToString().ToLower();
    }

    private int? GetMaxLength(DataType dataType)
    {
        return dataType.MaximumLength > 0 ? dataType.MaximumLength : null;
    }

    private int? GetNumericPrecision(DataType dataType)
    {
        return dataType.NumericPrecision > 0 ? dataType.NumericPrecision : null;
    }

    private int? GetNumericScale(DataType dataType)
    {
        return dataType.NumericScale > 0 ? dataType.NumericScale : null;
    }

    private Table[] GetFilteredTables()
    {
        return _db.Tables.OfType<Table>()
            .Where(table => !_tablesToIgnore.Contains(table.Name) && _config.ExportTable(table.Schema, table.Name))
            .ToArray();
    }

    private View[] GetFilteredViews()
    {
        return _db.Views.OfType<View>()
            .Where(view => view.Schema is not "sys" and not "INFORMATION_SCHEMA" &&
                           !_viewsToIgnore.Contains(view.Name) && _config.ExportView(view.Schema, view.Name))
            .ToArray();
    }

    private StoredProcedure[] GetFilteredStoredProcedures()
    {
        return _db.StoredProcedures.OfType<StoredProcedure>()
            .Where(storedProc => storedProc.Schema is not "sys" && 
                                 _config.ExportStoredProcedure(storedProc.Schema, storedProc.Name))
            .ToArray();
    }
}
