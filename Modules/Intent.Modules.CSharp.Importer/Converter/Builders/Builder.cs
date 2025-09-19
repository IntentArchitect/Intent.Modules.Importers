namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public static class Builder
{
    public static ClassElementBuilder CreateClassBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    {
        var classBuilder = new ClassElementBuilder(externalReference, name, builderMetadataManager);
        builderMetadataManager.AddElementForLookup(classBuilder.InternalElement);
        return classBuilder;
    }

    public static AssociationBuilder CreateAssociationBuilder(BuilderMetadataManager builderMetadataManager)
    {
        var associationBuilder = new AssociationBuilder(builderMetadataManager);
        return associationBuilder;
    }

    //public static EnumElementBuilder CreateEnumBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var enumBuilder = new EnumElementBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(enumBuilder.InternalElement);
    //    return enumBuilder;
    //}


    //public static DomainServiceElementBuilder CreateDomainServiceBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var domainServiceBuilder = new DomainServiceElementBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(domainServiceBuilder.InternalElement);
    //    return domainServiceBuilder;
    //}

    //public static DomainRepositoryBuilder CreateDomainRepositoryBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var domainRepositoryBuilder = new DomainRepositoryBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(domainRepositoryBuilder.InternalElement);
    //    return domainRepositoryBuilder;
    //}

    //public static DomainDataContractBuilder CreateDomainDataContractBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var domainDataContractBuilder = new DomainDataContractBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(domainDataContractBuilder.InternalElement);
    //    return domainDataContractBuilder;
    //}

    //public static DtoElementBuilder CreateServiceDtoBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var serviceDtoBuilder = new DtoElementBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(serviceDtoBuilder.InternalElement);
    //    return serviceDtoBuilder;
    //}

    //public static ValueObjectBuilder CreateValueObjectBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var valueObjectBuilder = new ValueObjectBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(valueObjectBuilder.InternalElement);
    //    return valueObjectBuilder;
    //}

    //public static EventMessageElementBuilder CreateEventMessageBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var eventMessageBuilder = new EventMessageElementBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(eventMessageBuilder.InternalElement);
    //    return eventMessageBuilder;
    //}

    //public static EventDtoElementBuilder CreateEventDtoBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    //{
    //    var eventMessageBuilder = new EventDtoElementBuilder(externalReference, name, builderMetadataManager);
    //    builderMetadataManager.AddElementForLookup(eventMessageBuilder.InternalElement);
    //    return eventMessageBuilder;
    //}


    //public static GeneralizationBuilder CreateGeneralizationBuilder(BuilderMetadataManager builderMetadataManager, string entityFullName, string baseTypeFullName)
    //{
    //    var generalizationBuilder = new GeneralizationBuilder(builderMetadataManager, entityFullName, baseTypeFullName);
    //    return generalizationBuilder;
    //}
}