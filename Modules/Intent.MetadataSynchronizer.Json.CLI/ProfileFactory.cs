using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.Json.CLI;

public enum ImportProfile
{
    DomainDocumentDB = 1,
    EventingMessages = 2
}

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
                ComplexTypeSpecializationType = ClassModel.SpecializationType,
                ComplexTypeSpecializationTypeId = ClassModel.SpecializationTypeId,
                ComplexTypeAttributeSpecializationType = AttributeModel.SpecializationType,
                ComplexTypeAttributeSpecializationTypeId = AttributeModel.SpecializationTypeId,
                AssociationSpecializationType = AssociationModel.SpecializationType,
                AssociationSpecializationTypeId = AssociationModel.SpecializationTypeId,
                AssociationSourceEndSpecializationType = @"Association Source End",
                AssociationSourceEndSpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                AssociationTargetEndSpecializationType = @"Association Target End",
                AssociationTargetEndSpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                DesignerName = "Domain",
                CreateAssociations = true
            },
            ImportProfile.EventingMessages => new ProfileSettings
            {
                // Root elements are Messages; nested complex types will be Eventing DTOs (handled in factory logic)
                RootSpecializationType = @"Message",
                RootSpecializationTypeId = @"cbe970af-5bad-4d92-a3ed-a24b9fdaa23e",
                AttributeSpecializationType = @"Property",
                AttributeSpecializationTypeId = @"bde29850-5fb9-4f47-9941-b9e182fd9bdc",
                ComplexTypeSpecializationType = @"Eventing DTO",
                ComplexTypeSpecializationTypeId = @"544f1d57-27ce-4985-a4ec-cc01568d72b0",
                ComplexTypeAttributeSpecializationType = @"Eventing DTO-Field",
                ComplexTypeAttributeSpecializationTypeId = @"93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7",
                DesignerName = "Services",
                CreateAssociations = false
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
    public string ComplexTypeSpecializationType { get; set; } = null!;
    public string ComplexTypeSpecializationTypeId { get; set; } = null!;
    public string ComplexTypeAttributeSpecializationType { get; set; } = null!;
    public string ComplexTypeAttributeSpecializationTypeId { get; set; } = null!;
    public string? AssociationSpecializationType { get; set; }
    public string? AssociationSpecializationTypeId { get; set; }
    public string? AssociationSourceEndSpecializationType { get; set; }
    public string? AssociationSourceEndSpecializationTypeId { get; set; }
    public string? AssociationTargetEndSpecializationType { get; set; }
    public string? AssociationTargetEndSpecializationTypeId { get; set; }
    public string DesignerName { get; set; } = null!;
    public bool CreateAssociations { get; set; }
}