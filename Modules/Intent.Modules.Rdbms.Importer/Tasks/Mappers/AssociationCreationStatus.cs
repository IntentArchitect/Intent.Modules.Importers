namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

/// <summary>
/// Represents the different outcomes when attempting to create an association
/// </summary>
public enum AssociationCreationStatus
{
    /// <summary>
    /// Association was successfully created
    /// </summary>
    Success,

    /// <summary>
    /// Target class for the association was not found
    /// </summary>
    TargetClassNotFound,

    /// <summary>
    /// Association already exists and duplicate was skipped
    /// </summary>
    DuplicateSkipped
}
