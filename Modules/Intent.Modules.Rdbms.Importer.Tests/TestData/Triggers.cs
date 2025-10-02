using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating TriggerSchema test objects
/// </summary>
internal static class Triggers
{
    public static TriggerSchema AfterInsertTrigger(string tableName) => new()
    {
        Name = $"trg_{tableName}_AfterInsert",
        ParentSchema = "dbo",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };

    public static TriggerSchema AfterUpdateTrigger(string tableName) => new()
    {
        Name = $"trg_{tableName}_AfterUpdate",
        ParentSchema = "dbo",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };

    public static TriggerSchema AfterDeleteTrigger(string tableName) => new()
    {
        Name = $"trg_{tableName}_AfterDelete",
        ParentSchema = "dbo",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };

    public static TriggerSchema InsteadOfInsertTrigger(string tableName) => new()
    {
        Name = $"trg_{tableName}_InsteadOfInsert",
        ParentSchema = "dbo",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };

    public static TriggerSchema SimpleInsteadOfInsertTrigger(string viewName) => new()
    {
        Name = $"trg_{viewName}_InsteadOfInsert",
        ParentSchema = "dbo",
        ParentName = viewName,
        ParentType = "View",
        Metadata = new()
    };

    public static TriggerSchema AuditTrigger(string tableName) => new()
    {
        Name = $"trg_{tableName}_Audit",
        ParentSchema = "dbo",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };

    public static TriggerSchema TriggerInSchema2(string tableName) => new()
    {
        Name = $"trg_{tableName}_CustomLogic",
        ParentSchema = "schema2",
        ParentName = tableName,
        ParentType = "Table",
        Metadata = new()
    };
}
