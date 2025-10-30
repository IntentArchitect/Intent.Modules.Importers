using System.Text;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;
using Shouldly;

namespace Intent.MetadataSynchronizer.OpenApi.CLI.Tests
{
    public partial class ItShouldWork
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var factory = new OpenApiPersistableFactory();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(Test1Json));
            var dictionary = ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "Dictionary", null);
            dictionary.GenericTypes.AddRange([new GenericType { Name = "TKey" }, new GenericType { Name = "TValue" }]);

            var packageModelPersistables = new[] { new PackageModelPersistable
            {
                Classes = new List<ElementPersistable>
                {
                    ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "string", null),
                    ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "int", null),
                    ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "decimal", null),
                    ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "bool", null),
                    dictionary,
                }
            } };

            // Act
            var exception = Record.Exception(() =>
            {
                factory.GetPersistables(
                    stream: stream,
                    config: new ImportConfig
                    {
                        ServiceType = ServiceType.CQRS,
                        SettingPersistence = SettingPersistence.None
                    },
                    packages: packageModelPersistables);
            });

            // Assert
            exception.ShouldBeNull();
        }
    }
}