using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for common ImportConfiguration scenarios
/// </summary>
internal static class ImportConfigurations
{
    public static ImportConfiguration TablesOnly() => new()
    {
        TypesToExport = [ExportType.Table],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent
    };

    public static ImportConfiguration TablesWithDeletions() => new()
    {
        TypesToExport = [ExportType.Table],
        AllowDeletions = true,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent
    };

    public static ImportConfiguration Everything() => new()
    {
        TypesToExport = [ExportType.Table, ExportType.View, ExportType.StoredProcedure, ExportType.Index],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent
    };

    /// <summary>
    /// Configuration for inclusive import of specific tables only (like Table B) with deletions enabled.
    /// Uses filter file to specify which tables to import.
    /// </summary>
    public static ImportConfiguration InclusiveImportTableBOnly() => new()
    {
        TypesToExport = [ExportType.Table],
        AllowDeletions = true,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent,
        ImportFilterFilePath = "test-filter.json" // This would contain the filter configuration
    };

    public static ImportConfiguration StoredProceduresAsOperations(bool isEfRepositoriesInstalled = true) => new()
    {
        TypesToExport = [ExportType.StoredProcedure],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent,
        StoredProcedureType = StoredProcedureType.RepositoryOperation,
        IsEfRepositoriesInstalled = isEfRepositoriesInstalled
    };

    public static ImportConfiguration StoredProceduresAsElements(bool isEfRepositoriesInstalled = true) => new()
    {
        TypesToExport = [ExportType.StoredProcedure],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent,
        StoredProcedureType = StoredProcedureType.StoredProcedureElement,
        IsEfRepositoriesInstalled = isEfRepositoriesInstalled
    };

    public static ImportConfiguration StoredProceduresInDefaultMode(bool isEfRepositoriesInstalled = true) => new()
    {
        TypesToExport = [ExportType.StoredProcedure],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent,
        StoredProcedureType = StoredProcedureType.Default,
        IsEfRepositoriesInstalled = isEfRepositoriesInstalled
    };

    public static ImportConfiguration StoredProceduresMappedToOperation(bool isEfRepositoriesInstalled = true) => new()
    {
        TypesToExport = [ExportType.StoredProcedure],
        AllowDeletions = false,
        EntityNameConvention = EntityNameConvention.SingularEntity,
        AttributeNameConvention = AttributeNameConvention.Default,
        TableStereotype = TableStereotype.WhenDifferent,
        StoredProcedureType = StoredProcedureType.RepositoryOperationMapping,
        IsEfRepositoriesInstalled = isEfRepositoriesInstalled
    };
}
