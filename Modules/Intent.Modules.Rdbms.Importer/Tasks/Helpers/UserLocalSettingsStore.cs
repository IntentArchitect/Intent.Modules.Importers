using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

/// <summary>
/// Stores a settings object per domain package in a user-local file, outside source control.
/// The <paramref name="fileNameSuffix"/> keeps otherwise-independent settings (e.g. database import
/// versus stored procedure import) in separate files for the same package.
/// </summary>
internal sealed class UserLocalSettingsStore<TSettings>(string? fileNameSuffix) where TSettings : class
{
    private const string SettingsVersion = "v1";
    private const string StorageFolderName = "Intent Architect";
    private const string FeatureFolderName = "Intent.Modules.Rdbms.Importer";

    public string GetDisplayPath(string packageFileName)
    {
        return GetSettingsFilePath(packageFileName);
    }

    public TSettings? Load(string packageFileName)
    {
        var settingsFilePath = GetSettingsFilePath(packageFileName);
        if (!File.Exists(settingsFilePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(settingsFilePath);
            return JsonSerializer.Deserialize<TSettings>(json, SerializationHelper.SerializerOptions);
        }
        catch (Exception ex)
        {
            Logging.Log.Warning($"Unable to load user-local import settings from '{settingsFilePath}': {ex.Message}");
            return null;
        }
    }

    public void Save(string packageFileName, TSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);
        ArgumentNullException.ThrowIfNull(settings);

        var settingsFilePath = GetSettingsFilePath(packageFileName);
        var directory = Path.GetDirectoryName(settingsFilePath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, SerializationHelper.IndentedSerializerOptions);

        File.WriteAllText(settingsFilePath, json);
    }

    public void Delete(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        var settingsFilePath = GetSettingsFilePath(packageFileName);
        if (!File.Exists(settingsFilePath))
        {
            return;
        }

        try
        {
            File.Delete(settingsFilePath);
        }
        catch (Exception ex)
        {
            Logging.Log.Warning($"Unable to delete user-local import settings at '{settingsFilePath}': {ex.Message}");
        }
    }

    private string GetSettingsFilePath(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        var suffix = string.IsNullOrWhiteSpace(fileNameSuffix) ? string.Empty : $".{fileNameSuffix}";
        var packageHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(packageFileName))).ToLowerInvariant();
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            StorageFolderName,
            FeatureFolderName,
            SettingsVersion,
            $"db-import-{packageHash}{suffix}.json");
    }
}
