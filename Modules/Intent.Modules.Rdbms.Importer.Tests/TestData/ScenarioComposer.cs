using System;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

internal readonly record struct Scenario(DatabaseSchema Schema, PackageModelPersistable Package);

internal static class ScenarioComposer
{
    public static Scenario Create(DatabaseSchema schema, PackageModelPersistable package)
    {
        return new Scenario(schema, package);
    }

    public static Scenario SchemaOnly(DatabaseSchema schema)
    {
        return new Scenario(schema, PackageModels.Empty());
    }

    public static Scenario PackageOnly(PackageModelPersistable package)
    {
        return new Scenario(DatabaseSchemas.Empty(), package);
    }
}

internal static class ScenarioExtensions
{
    public static Scenario WithSchemaMutation(this Scenario scenario, Action<DatabaseSchema> mutate)
    {
        mutate?.Invoke(scenario.Schema);
        return scenario;
    }

    public static Scenario WithPackageMutation(this Scenario scenario, Action<PackageModelPersistable> mutate)
    {
        mutate?.Invoke(scenario.Package);
        return scenario;
    }

    public static Scenario WithTable(this Scenario scenario, TableSchema table)
    {
        scenario.Schema.Tables.Add(table);
        return scenario;
    }

    public static Scenario WithSchemaColumn(this Scenario scenario, string tableName, ColumnSchema column)
    {
        var table = scenario.Schema.Tables.Single(t => string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));
        table.Columns.Add(column);
        return scenario;
    }

    public static Scenario WithoutSchemaColumn(this Scenario scenario, string tableName, string columnName)
    {
        var table = scenario.Schema.Tables.Single(t => string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));
        var column = table.Columns.FirstOrDefault(c => string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
        if (column != null)
        {
            table.Columns.Remove(column);
        }

        return scenario;
    }

    public static Scenario WithPackageClass(this Scenario scenario, ElementPersistable element)
    {
        scenario.Package.Classes.Add(element);
        return scenario;
    }

    public static Scenario WithoutPackageAttribute(this Scenario scenario, string className, string attributeName)
    {
        var element = scenario.Package.Classes.Single(c => string.Equals(c.Name, className, StringComparison.OrdinalIgnoreCase));
        var attribute = element.ChildElements.FirstOrDefault(a => string.Equals(a.Name, attributeName, StringComparison.OrdinalIgnoreCase));
        if (attribute != null)
        {
            element.ChildElements.Remove(attribute);
        }

        return scenario;
    }

    public static Scenario WithPackageAttribute(this Scenario scenario, string className, ElementPersistable attribute)
    {
        var element = scenario.Package.Classes.Single(c => string.Equals(c.Name, className, StringComparison.OrdinalIgnoreCase));
        element.ChildElements.Add(attribute);
        return scenario;
    }
}
