using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.MetadataSynchronizer.Java;

internal class Stereotypes
{
    public static class Java
    {
        public const string Name = "Java";
        public const string DefinitionId = "7c40ee54-3dd5-4bdd-b6d3-2d65be7c928e";
        public const string DefinitionPackageName = "Intent.Common.Java";
        public const string DefinitionPackageId = "1b8f2b45-99c0-42dc-b28e-bcd4621c3e71";

        public static class Property
        {
            public const string Package = "Package";
            public const string IsCollection = "Is Collection";
        }

        public static StereotypePersistable Create(string package)
        {
            return new StereotypePersistable
            {
                DefinitionId = DefinitionId,
                Name = Name,
                DefinitionPackageName = DefinitionPackageName,
                DefinitionPackageId = DefinitionPackageId,
                Properties = new List<StereotypePropertyPersistable>
                {
                    new()
                    {
                        DefinitionId = Guid.NewGuid().ToString().ToLower(),
                        Name = Property.Package,
                        Value = package
                    }
                }
            };
        }
    }
}