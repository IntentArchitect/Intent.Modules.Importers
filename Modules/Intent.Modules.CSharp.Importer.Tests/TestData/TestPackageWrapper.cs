using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.MetadataSynchronizer;
using Intent.Persistence;
using IPackageModelPersistable = Intent.Persistence.IPackageModelPersistable;
using IElementPersistable = Intent.Persistence.IElementPersistable;
using MetadataNamingConvention = Intent.Persistence.MetadataNamingConvention;
using SortChildrenOptions = Intent.Persistence.SortChildrenOptions;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Test wrapper that implements IPackageModelPersistable directly.
/// Delegates all calls to the inner instance except GetReferencedPackages.
/// </summary>
internal class TestPackageWrapper : IPackageModelPersistable
{
    private readonly IPackageModelPersistable _inner;

    public TestPackageWrapper(IPackageModelPersistable inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    // Stub setup - returns empty to avoid designer dependency
    public IEnumerable<IPackageModelPersistable> GetReferencedPackages()
    {
         return Enumerable.Empty<IPackageModelPersistable>();
    }

    // Properties with correct types and setters where needed
    public string Id { get => _inner.Id; set => _inner.Id = value; }
    public string Name { get => _inner.Name; set => _inner.Name = value; }
    public string SpecializationType { get => _inner.SpecializationType; set => _inner.SpecializationType = value; }
    public string SpecializationTypeId { get => _inner.SpecializationTypeId; set => _inner.SpecializationTypeId = value; }
    public string ApplicationId { get => _inner.ApplicationId; set => _inner.ApplicationId = value; }
    public string DesignerId { get => _inner.DesignerId; set => _inner.DesignerId = value; }
    public string Comment { get => _inner.Comment; set => _inner.Comment = value; }
    public IIconModelPersistable Icon => _inner.Icon;
    public string ExternalReference { get => _inner.ExternalReference; set => _inner.ExternalReference = value; }
    public SortChildrenOptions? SortChildren { get => _inner.SortChildren; set => _inner.SortChildren = value; }
    public MetadataNamingConvention NamingConvention { get => _inner.NamingConvention; set => _inner.NamingConvention = value; }
    
    // Methods
    public IEnumerable<IElementPersistable> GetAllElements() => _inner.GetAllElements();
    public void Save() => _inner.Save();
    public IEnumerable<IElementPersistable> GetElementsOfType(string type) => _inner.GetElementsOfType(type);
    public IElementPersistable GetElementById(string id) => _inner.GetElementById(id);
    public IReadOnlyCollection<IElementPersistable> FindChildElements(Func<IElementPersistable, bool> predicate) => _inner.FindChildElements(predicate);
    public IApplicationDesignerPersistable GetDesigner() => _inner.GetDesigner();

    // Collections with correct types
    public IEnumerable<IPackageReferenceModelPersistable> References => _inner.References;
    public IStereotypePersistableCollection Stereotypes => _inner.Stereotypes;
    public IGenericMetadataPersistableCollection Metadata => _inner.Metadata;
    public IElementPersistableCollection Classes => _inner.Classes;
    public IAssociationPersistableCollection Associations => _inner.Associations;
}
