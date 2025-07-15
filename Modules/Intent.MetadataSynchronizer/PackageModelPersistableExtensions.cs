using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.MetadataSynchronizer;

public static class PackageModelPersistableExtensions
{
    public static IEnumerable<ElementPersistable> GetAllElements(this PackageModelPersistable package)
    {
        static IEnumerable<ElementPersistable> GetAll(IEnumerable<ElementPersistable> elements)
        {
            foreach (var element in elements)
            {
                yield return element;

                foreach (var childElement in GetAll(element.ChildElements))
                {
                    yield return childElement;
                }
            }
        }

        return GetAll(package.Classes);
    }
	public static void AddMetadata(this PackageModelPersistable package, string key, string value)
	{
		var existing = package.Metadata.SingleOrDefault(x => x.Key == key);
		if (existing != null)
		{
			existing.Value = value;
		}
		else
		{
			package.Metadata.Add(new GenericMetadataPersistable() { Key = key, Value = value });
		}
	}

	public static void RemoveMetadata(this PackageModelPersistable package, string key)
	{
		if (package.Metadata.Any(x => x.Key == key))
		{
			package.Metadata.Remove(package.Metadata.Single(x => x.Key == key));
		}
	}
}
