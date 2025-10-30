using System.Text;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.MetadataSynchronizer;

namespace Intent.Modules.Json.Importer.Tests.TestData;

/// <summary>
/// Helper class for building readable snapshots of Persistables for verification testing.
/// </summary>
public static class SnapshotBuilder
{
    public static string BuildSnapshot(Persistables result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== ELEMENTS ===");
        sb.AppendLine();

        var sortedElements = result.Elements
            .OrderBy(e => e.SpecializationType)
            .ThenBy(e => e.Name)
            .ToList();

        foreach (var element in sortedElements)
        {
            AppendElement(sb, element, indent: 0);
        }

        sb.AppendLine();
        sb.AppendLine("=== ASSOCIATIONS ===");
        sb.AppendLine();

        if (result.Associations.Any())
        {
            foreach (var association in result.Associations.OrderBy(a => a.SourceEnd.Name).ThenBy(a => a.TargetEnd.Name))
            {
                sb.AppendLine($"Association: {association.SourceEnd.Name} → {association.TargetEnd.Name}");
                sb.AppendLine($"  Type: {association.AssociationType}");
                sb.AppendLine($"  Source:");
                sb.AppendLine($"    TypeId: {association.SourceEnd.TypeReference.TypeId}");
                sb.AppendLine($"    IsNavigable: {association.SourceEnd.TypeReference.IsNavigable}");
                sb.AppendLine($"    IsCollection: {association.SourceEnd.TypeReference.IsCollection}");
                sb.AppendLine($"  Target:");
                sb.AppendLine($"    TypeId: {association.TargetEnd.TypeReference.TypeId}");
                sb.AppendLine($"    IsNavigable: {association.TargetEnd.TypeReference.IsNavigable}");
                sb.AppendLine($"    IsCollection: {association.TargetEnd.TypeReference.IsCollection}");
                sb.AppendLine();
            }
        }
        else
        {
            sb.AppendLine("(none)");
        }

        return sb.ToString();
    }

    private static void AppendElement(StringBuilder sb, ElementPersistable element, int indent)
    {
        var indentStr = new string(' ', indent * 2);

        sb.AppendLine($"{indentStr}{element.SpecializationType}: {element.Name}");
        sb.AppendLine($"{indentStr}  Id: {element.Id}");

        if (!string.IsNullOrEmpty(element.ExternalReference))
        {
            sb.AppendLine($"{indentStr}  ExternalReference: {element.ExternalReference}");
        }

        if (!string.IsNullOrEmpty(element.ParentFolderId))
        {
            sb.AppendLine($"{indentStr}  ParentFolderId: {element.ParentFolderId}");
        }

        if (element.TypeReference != null)
        {
            sb.AppendLine($"{indentStr}  TypeReference:");
            sb.AppendLine($"{indentStr}    TypeId: {element.TypeReference.TypeId}");
            sb.AppendLine($"{indentStr}    IsNavigable: {element.TypeReference.IsNavigable}");
            sb.AppendLine($"{indentStr}    IsCollection: {element.TypeReference.IsCollection}");

            if (!string.IsNullOrEmpty(element.TypeReference.Comment))
            {
                sb.AppendLine($"{indentStr}    Comment: {element.TypeReference.Comment}");
            }
        }

        if (element.Stereotypes.Any())
        {
            sb.AppendLine($"{indentStr}  Stereotypes:");
            foreach (var stereotype in element.Stereotypes)
            {
                sb.AppendLine($"{indentStr}    - {stereotype.Name} (DefinitionId: {stereotype.DefinitionId})");
            }
        }

        if (element.Metadata.Any())
        {
            sb.AppendLine($"{indentStr}  Metadata:");
            foreach (var metadata in element.Metadata)
            {
                sb.AppendLine($"{indentStr}    - {metadata.Key}: {metadata.Value}");
            }
        }

        if (element.ChildElements.Any())
        {
            sb.AppendLine($"{indentStr}  Children:");
            foreach (var child in element.ChildElements.OrderBy(c => c.Name))
            {
                AppendElement(sb, child, indent + 2);
            }
        }

        sb.AppendLine();
    }
}
