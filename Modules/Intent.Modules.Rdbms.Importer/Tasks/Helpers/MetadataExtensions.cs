using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal static class MetadataExtensions
{
	internal static void AddMetadata(this PackageModelPersistable package, string key, string? value)
	{
		var existing =  package.Metadata.FirstOrDefault(x => x.Key == key);
		if (existing != null)
		{
			existing.Value = value!;
		}
		else
		{
			package.Metadata.Add(new GenericMetadataPersistable { Key = key, Value = value! });
		}
	}

	internal static void RemoveMetadata(this PackageModelPersistable package, string key)
	{
		if (package.Metadata.Any(x => x.Key == key))
		{
			package.Metadata.Remove(package.Metadata.Single(x => x.Key == key));
		}
	}

	internal static string? GetMetadataValue(this PackageModelPersistable package, string key)
	{
		return package.Metadata.FirstOrDefault(x => x.Key == key)?.Value;
	}
	
	internal static void SetElementMetadata(this ElementPersistable element, string key, string? value)
	{
		var existing =  element.Metadata.FirstOrDefault(x => x.Key == key);
		if (existing != null)
		{
			existing.Value = value!;
		}
		else
		{
			element.Metadata.Add(new GenericMetadataPersistable { Key = key, Value = value! });
		}
	}

	internal static void RemoveElementMetadata(this ElementPersistable element, string key)
	{
		if (element.Metadata.Any(x => x.Key == key))
		{
			element.Metadata.Remove(element.Metadata.Single(x => x.Key == key));
		}
	}

	internal static string? GetElementMetadataValue(this ElementPersistable element, string key)
	{
		return element.Metadata.FirstOrDefault(x => x.Key == key)?.Value;
	}
}