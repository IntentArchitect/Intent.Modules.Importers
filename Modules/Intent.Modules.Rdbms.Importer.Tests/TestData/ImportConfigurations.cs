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
}
