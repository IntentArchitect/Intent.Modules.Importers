using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Xml;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.MetadataSynchronizer.Configuration;
using Intent.MetadataSynchronizer.CSharp.CLI.Builders;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using AssociationType = System.Data.Entity.Core.Metadata.Edm.AssociationType;

// NOTE!!!
// This is experimental, quick and dirty for now, once we can prove it works then we can clean it up and make available like all the other tools

class Program
{
    static async Task<int> Main(string[] args)
    {
        Helpers.Execute(
            intentSolutionPath: @"E:\TestApps\AlulaEdmxImport\intent\AlulaEdmxImport.isln",
            applicationName: "AlulaEdmxImport",
            designerName: "Domain",
            packageId: "b234e861-1434-4827-a1fd-d4514515519e",
            targetFolderId: null,
            deleteExtra: false,
            debug: true,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge,
            additionalPreconditionChecks: null,
            getPersistables: packages => GetFromEdmx(@"E:\Clients\Ovation.OLB.Entities\Ovation.OLB.Entities\OnlineBenefits.edmx", packages));

        return 0;
    }

    private static Persistables GetFromEdmx(string edmxPath, IReadOnlyCollection<PackageModelPersistable> packages)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(edmxPath);

        // Load the CSDL (Conceptual Schema Definition Language)
        var csdl = xmlDoc.GetElementsByTagName("Schema")[1].OuterXml;
        var csdlReader = XmlReader.Create(new StringReader(csdl));
        var csdlItemCollection = new EdmItemCollection(new[] { csdlReader });

        var elements = new List<ElementPersistable>();
        var associations = new List<AssociationPersistable>();

        var standardLookup = new MetadataLookup(packages);
        var builderMetadataManager = new BuilderMetadataManager(standardLookup, elements);

        var entityBuilders = new List<(EntityType Entity, ClassElementBuilder Builder)>();
        var associationMetaLookup = new Dictionary<string, AssociationMetadata>();

        foreach (var enumType in csdlItemCollection.GetItems<EnumType>())
        {
            var builder = Builder.CreateEnumBuilder(enumType.FullName, enumType.Name, builderMetadataManager);
            foreach (var literal in enumType.Members)
            {
                builder.AddLiteral(literal.Name, literal.Value?.ToString());
            }

            elements.Add(builder.Build());
        }

        foreach (var complexType in csdlItemCollection.GetItems<ComplexType>())
        {
            var builder = Builder.CreateValueObjectBuilder(complexType.FullName, complexType.Name, builderMetadataManager);
            builder.AddAttributes(complexType.Properties,
                prop => new AttributeBuilder(prop.Name, GetIntentType(prop), prop.Nullable, prop.IsCollectionType, GetDocumentation(prop.Documentation)));
            elements.Add(builder.Build());
        }

        foreach (var entityType in csdlItemCollection.GetItems<EntityType>())
        {
            var builder = Builder.CreateClassBuilder(entityType.FullName, entityType.Name, builderMetadataManager);
            builder.AddAttributes(entityType.DeclaredProperties, prop =>
            {
                var attr = new AttributeBuilder(prop.Name, GetIntentType(prop), prop.Nullable, prop.IsCollectionType, GetDocumentation(prop.Documentation));
                if (entityType.KeyMembers.Any(p => p.Name == prop.Name))
                {
                    attr.AddStereotype("b99aac21-9ca4-467f-a3a6-046255a9eed6", "Primary Key", new[]
                    {
                        new StereotypeProperty(Name: "4c1e3f7e-61d4-460d-bd20-c2edbc0c0e2e", Display: "Identity", Value: "false"),
                        new StereotypeProperty(Name: "a7a5e921-18b9-43b4-8078-b4ac4e5dae6f", Display: "Data source", Value: "Default")
                    });
                }

                return attr;
            });
            builder.SetIsAbstract(entityType.Abstract);
            entityBuilders.Add((entityType, builder));
        }

        foreach (var entry in entityBuilders)
        {
            if (entry.Entity.BaseType is not null)
            {
                var generalizationBuilder = Builder.CreateGeneralizationBuilder(
                    builderMetadataManager, entry.Entity.FullName, entry.Entity.BaseType.FullName);
                var association = generalizationBuilder.Build();
                if (!standardLookup.HasExistingAssociation(association))
                {
                    associations.Add(association);
                    standardLookup.AddAssociation(association);
                }
            }

            BuildAssociation(entry.Entity, associationMetaLookup, builderMetadataManager);

            elements.Add(entry.Builder.Build());
        }

        foreach (var meta in associationMetaLookup.Values)
        {
            var association = meta.Builder.Build();
            associations.Add(association);
            
            var sourceEnd = (meta.AssociationType.KeyMembers.First() as AssociationEndMember)!;
            var targetEnd = (meta.AssociationType.KeyMembers.ElementAt(1) as AssociationEndMember)!;
            
            ReadOnlyCollection<EdmProperty> toProps = meta.AssociationType.ReferentialConstraints.FirstOrDefault()?.ToProperties ?? ReadOnlyCollection<EdmProperty>.Empty;
            foreach (var prop in toProps)
            {
                standardLookup.TryGetElementByReference(meta.AssociationType.ReferentialConstraints.First().ToRole.GetEntityType().FullName, ClassModel.SpecializationTypeId, out var toEntityElement);
                var attribute = toEntityElement.ChildElements.FirstOrDefault(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId && p.Name == prop.Name);
                if (attribute is null)
                {
                    continue;
                }
            
                attribute.Stereotypes.Add(new StereotypePersistable
                {
                    DefinitionId = "793a5128-57a1-440b-a206-af5722b752a6",
                    Name = "Foreign Key",
                    Properties = new List<StereotypePropertyPersistable>
                    {
                        new StereotypePropertyPersistable
                        {
                            DefinitionId = "42e4f9b5-f834-4e5f-86aa-d3a35c505076",
                            Name = "Association",
                            Value = association.TargetEnd.Id,
                            IsActive = true
                        }
                    }
                });
            
                attribute.AddMetadata("fk-original-name", prop.Name);
                attribute.AddMetadata("association", association.TargetEnd.Id);
                attribute.AddMetadata("is-managed-key", "true");
            }
        }

        return new Persistables(elements, associations);
    }

    record AssociationMetadata(AssociationBuilder Builder, AssociationType AssociationType);
    
    private static void BuildAssociation(EntityType entity, Dictionary<string, AssociationMetadata> associationBuilderLookup, BuilderMetadataManager builderMetadataManager)
    {
        foreach (var navigationProperty in entity.DeclaredNavigationProperties)
        {
            var associationType = (navigationProperty.RelationshipType as AssociationType)!;
            var sourceEnd = (associationType.KeyMembers.First() as AssociationEndMember)!;
            var targetEnd = (associationType.KeyMembers.ElementAt(1) as AssociationEndMember)!;
            
            if (!associationBuilderLookup.TryGetValue(navigationProperty.RelationshipType.Name, out var meta))
            {
                meta = new AssociationMetadata(Builder.CreateAssociationBuilder(builderMetadataManager), associationType);
                associationBuilderLookup.Add(navigationProperty.RelationshipType.Name, meta);
            }

            EntityType fromEntity;
            EntityType toEntity;
            
            if (navigationProperty.FromEndMember == sourceEnd && navigationProperty.ToEndMember == targetEnd)
            {
                fromEntity = entity;
                toEntity = targetEnd.GetEntityType();

                var bidirectionalNavProperty = toEntity.DeclaredNavigationProperties.FirstOrDefault(p => p.RelationshipType.Name == navigationProperty.RelationshipType.Name);
                if (bidirectionalNavProperty is null)
                {
                    var sourceComps = GetComponents(sourceEnd.RelationshipMultiplicity);
                    meta.Builder.AddUnidirectionalSource(fromEntity.FullName, navigationProperty.Name, sourceComps.IsNullable, sourceComps.IsCollection);
                }
                else
                {
                    var sourceComps = GetComponents(sourceEnd.RelationshipMultiplicity);
                    meta.Builder.AddBidirectionalSource(fromEntity.FullName, GetName(navigationProperty, bidirectionalNavProperty, sourceEnd), sourceComps.IsNullable, sourceComps.IsCollection);
                }

                var targetComps = GetComponents(targetEnd.RelationshipMultiplicity);
                meta.Builder.AddTarget(toEntity.FullName, navigationProperty.Name, targetComps.IsNullable, targetComps.IsCollection);
            }
            else
            {
                fromEntity = entity;
                toEntity = sourceEnd.GetEntityType();
                
                var bidirectionalNavProperty = toEntity.DeclaredNavigationProperties.FirstOrDefault(p => p.RelationshipType.Name == navigationProperty.RelationshipType.Name);
                if (bidirectionalNavProperty is null)
                {
                    var sourceComps = GetComponents(targetEnd.RelationshipMultiplicity);
                    meta.Builder.AddBidirectionalSource(fromEntity.FullName, associationType.Name, sourceComps.IsNullable, sourceComps.IsCollection);
                }
                else
                {
                    var sourceComps = GetComponents(targetEnd.RelationshipMultiplicity);
                    meta.Builder.AddBidirectionalSource(fromEntity.FullName, GetName(navigationProperty, bidirectionalNavProperty, targetEnd), sourceComps.IsNullable, sourceComps.IsCollection);
                }

                var targetComps = GetComponents(sourceEnd.RelationshipMultiplicity);
                meta.Builder.AddTarget(toEntity.FullName, navigationProperty.Name, targetComps.IsNullable, targetComps.IsCollection);
            }
        }

        return;
        string GetName(NavigationProperty navigationProperty, NavigationProperty? bidirectionalNavProperty, AssociationEndMember associationEnd)
        {
            var cmp = GetComponents(associationEnd.RelationshipMultiplicity);
            return navigationProperty == bidirectionalNavProperty 
                ? cmp.IsCollection ? associationEnd.GetEntityType().Name.Pluralize() : associationEnd.GetEntityType().Name 
                : (bidirectionalNavProperty?.Name ?? associationEnd.Name);
        }
    }

    private static (bool IsNullable, bool IsCollection) GetComponents(RelationshipMultiplicity multiplicity)
    {
        return multiplicity switch
        {
            RelationshipMultiplicity.Many => (false, true),
            RelationshipMultiplicity.One => (false, false),
            RelationshipMultiplicity.ZeroOrOne => (true, false)
        };
    }

    private static string? GetDocumentation(Documentation? docs)
    {
        if (docs is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(docs.Summary))
        {
            return docs.Summary;
        }

        if (!string.IsNullOrWhiteSpace(docs.LongDescription))
        {
            return docs.LongDescription;
        }

        return null;
    }

    private static string GetIntentType(EdmProperty edmProperty)
    {
        if (!string.IsNullOrWhiteSpace(edmProperty.EnumType?.FullName))
        {
            return edmProperty.EnumType.FullName;
        }

        if (!string.IsNullOrWhiteSpace(edmProperty.ComplexType?.FullName))
        {
            return edmProperty.ComplexType.FullName;
        }

        return edmProperty.TypeName switch
        {
            "Int32" => "int",
            "Int64" => "long",
            "String" => "string",
            "Guid" => "guid",
            "Boolean" => "bool",
            "Decimal" => "decimal",
            "DateTime" => "datetime",
            "DateTimeOffset" => "datetimeoffset",
            _ => edmProperty.TypeName
        };
    }
}