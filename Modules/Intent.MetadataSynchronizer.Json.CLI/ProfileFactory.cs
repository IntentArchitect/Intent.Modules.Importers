using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.Json.CLI;

// Enum for import profiles
public enum ImportProfile
{
    DomainDocumentDB, // Traditional domain with Document DB annotations
    EventingMessages // For integration events
}

// Factory to resolve profile settings internally
public static class ProfileFactory
{
    public static ProfileSettings GetSettings(ImportProfile profile)
    {
        return profile switch
        {
            ImportProfile.DomainDocumentDB => new ProfileSettings
            {
                RootSpecializationType = ClassModel.SpecializationType,
                RootSpecializationTypeId = ClassModel.SpecializationTypeId,
                AttributeSpecializationType = AttributeModel.SpecializationType,
                AttributeSpecializationTypeId = AttributeModel.SpecializationTypeId,
                AssociationSpecializationType = AssociationModel.SpecializationType,
                AssociationSpecializationTypeId = AssociationModel.SpecializationTypeId,
                AssociationSourceEndSpecializationType = @"Association Source End",
                AssociationSourceEndSpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                AssociationTargetEndSpecializationType = @"Association Target End",
                AssociationTargetEndSpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                DesignerName = "Domain"
            },
            ImportProfile.EventingMessages => new ProfileSettings
            {
                RootSpecializationType = @"Message",
                RootSpecializationTypeId = @"cbe970af-5bad-4d92-a3ed-a24b9fdaa23e",
                AttributeSpecializationType = @"Property",
                AttributeSpecializationTypeId = @"bde29850-5fb9-4f47-9941-b9e182fd9bdc",
                AssociationSpecializationType = AssociationModel.SpecializationType,
                AssociationSpecializationTypeId = AssociationModel.SpecializationTypeId,
                AssociationSourceEndSpecializationType = @"Association Source End",
                AssociationSourceEndSpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                AssociationTargetEndSpecializationType = @"Association Target End",
                AssociationTargetEndSpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                DesignerName = "Services"
            },
            _ => throw new ArgumentOutOfRangeException($"Profile '{profile}' is not supported.")
        };
    }
}

// Class to hold resolved settings
public class ProfileSettings
{
    public string RootSpecializationType { get; set; } = null!;
    public string RootSpecializationTypeId { get; set; } = null!;
    public string AttributeSpecializationType { get; set; } = null!;
    public string AttributeSpecializationTypeId { get; set; } = null!;
    public string AssociationSpecializationType { get; set; } = null!;
    public string AssociationSpecializationTypeId { get; set; } = null!;
    public string AssociationSourceEndSpecializationType { get; set; } = null!;
    public string AssociationSourceEndSpecializationTypeId { get; set; } = null!;
    public string AssociationTargetEndSpecializationType { get; set; } = null!;
    public string AssociationTargetEndSpecializationTypeId { get; set; } = null!;
    public string DesignerName { get; set; } = null!;
}