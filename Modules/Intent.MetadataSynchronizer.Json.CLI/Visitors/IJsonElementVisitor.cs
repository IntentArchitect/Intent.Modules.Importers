using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;

namespace Intent.MetadataSynchronizer.Json.CLI.Visitors;

public interface IJsonElementVisitor
{
    string DesignerName { get; }

    /// <summary>
    /// Create the root element for a JSON file.
    /// </summary>
    ElementPersistable VisitRoot(string name, string? parentFolderId, string externalReference);

    /// <summary>
    /// Handle a JSON property with an object value. Implementations may create an association
    /// or a reference property to the created class-like element.
    /// </summary>
    /// <param name="config">The configuration for the JSON processing.</param>
    /// <param name="property">The JSON property.</param>
    /// <param name="owner">The element that owns this property.</param>
    /// <param name="parentFolderId">Folder to place created class-like element under.</param>
    /// <param name="sourcePath">Logical path for the owner side (e.g., "person" or "person.address").</param>
    /// <param name="targetPath">Logical path for the created element (e.g., "person.address").</param>
    /// <param name="isCollection">Whether the property is an array of objects.</param>
    /// <returns>Result with the created class-like element and optional association.</returns>
    VisitorClassResult VisitObject(JsonConfig config, JsonProperty property, ElementPersistable owner, string? parentFolderId, string sourcePath, string targetPath, bool isCollection);

    /// <summary>
    /// Handle a JSON property with a primitive value.
    /// Implementations should create the correct attribute/property element and set its TypeReference and any stereotypes.
    /// </summary>
    /// <param name="config">The configuration for the JSON processing.</param>
    /// <param name="property">The JSON property.</param>
    /// <param name="owner">The element that will own the created attribute/property.</param>
    /// <param name="externalReference">Logical path for the attribute/property.</param>
    /// <param name="referencedType">The type definition to reference.</param>
    /// <param name="isCollection">Whether the value is an array of primitives.</param>
    /// <param name="remarks">Optional remarks to place on the type reference comment.</param>
    /// <param name="isRootElement">True if owning element is the file root; used for special handling like primary key.</param>
    /// <returns>The created attribute/property element.</returns>
    void VisitProperty(JsonConfig config, JsonProperty property, ElementPersistable owner, string externalReference, ElementPersistable referencedType, bool isCollection, string? remarks, bool isRootElement);
}

public record VisitorClassResult(ElementPersistable ClassElement, AssociationPersistable? Association);