using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

/// <summary>
/// Represents the result of attempting to create an association from a foreign key
/// </summary>
public class AssociationCreationResult
{
    private AssociationCreationResult(AssociationCreationStatus status, AssociationPersistable? association = null, string? reason = null)
    {
        Status = status;
        Association = association;
        Reason = reason;
    }

    /// <summary>
    /// The status of the association creation attempt
    /// </summary>
    public AssociationCreationStatus Status { get; }

    /// <summary>
    /// The created association (only available when Status is Success)
    /// </summary>
    public AssociationPersistable? Association { get; }

    /// <summary>
    /// Additional details about why the association creation failed or was skipped
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Creates a successful result with the created association
    /// </summary>
    public static AssociationCreationResult Success(AssociationPersistable association)
        => new(AssociationCreationStatus.Success, association);

    /// <summary>
    /// Creates a result indicating the target class was not found
    /// </summary>
    /// <param name="foreignKey"></param>
    public static AssociationCreationResult TargetClassNotFound(ForeignKeySchema foreignKey)
        => new(AssociationCreationStatus.TargetClassNotFound, reason: $"Target class not found for foreign key '{foreignKey.Name}' on table '{foreignKey.TableName}'");

    /// <summary>
    /// Creates a result indicating a duplicate association was detected and skipped
    /// </summary>
    public static AssociationCreationResult DuplicateSkipped(ElementPersistable sourceClass, ElementPersistable targetClass)
        => new(AssociationCreationStatus.DuplicateSkipped, reason: $"Association from '{sourceClass.Name}' to '{targetClass.Name}' already exists");

    /// <summary>
    /// Creates a result indicating the foreign key structure is not supported for association creation
    /// </summary>
    public static AssociationCreationResult UnsupportedForeignKey(ForeignKeySchema foreignKey, string reason)
        => new(AssociationCreationStatus.UnsupportedForeignKey, reason: $"Foreign key '{foreignKey.Name}' on table '{foreignKey.TableName}' is not supported: {reason}");
}