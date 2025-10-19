using System.Text;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.OpenApi.Importer.Importer;
using Shouldly;

namespace Intent.Modules.OpenApi.Importer.Tests
{
    public partial class OpenApiPersistableFactoryTests
    {
        [Fact]
        public void GetPersistables_WithValidOpenApiDocument_ShouldNotThrow()
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
                        IslnFile = null!,
                        ApplicationName = null!,
                        OpenApiSpecificationFile = null!,
                        TargetFolderId = null,
                        PackageId = null!,
                        AddPostFixes = false,
                        IsAzureFunctions = false,
                        ServiceType = ServiceType.CQRS,
                        AllowRemoval = false,
                        SettingPersistence = SettingPersistence.None
                    },
                    packages: packageModelPersistables);
            });

            // Assert
            exception.ShouldBeNull();
        }
    }
}
