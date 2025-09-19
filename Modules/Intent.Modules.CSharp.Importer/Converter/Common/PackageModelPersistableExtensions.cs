using System.Collections.Generic;
using System.Linq;
using Intent.Persistence;

namespace Intent.MetadataSynchronizer;

public static class IPackageModelPersistableExtensions
{
    public static IEnumerable<IElementPersistable> GetAllElements(this IPackageModelPersistable package)
    {
        static IEnumerable<IElementPersistable> GetAll(IEnumerable<IElementPersistable> elements)
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
	public static void AddMetadata(this IPackageModelPersistable package, string key, string value)
	{
		var existing = package.Metadata.SingleOrDefault(x => x.Key == key);
		if (existing != null)
		{
			existing.Value = value;
		}
		else
        {
            package.Metadata.Add(key, value);
		}
	}

	public static void RemoveMetadata(this IPackageModelPersistable package, string key)
	{
		if (package.Metadata.Any(x => x.Key == key))
		{
			package.Metadata.Remove(package.Metadata.Single(x => x.Key == key));
		}
	}
}
