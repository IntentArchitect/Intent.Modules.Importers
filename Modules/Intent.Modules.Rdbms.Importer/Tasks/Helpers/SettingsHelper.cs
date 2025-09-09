using System;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.Utils;
using Microsoft.Data.SqlClient;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal static class SettingsHelper
{
    public static void PersistSettings(DatabaseImportModel importModel)
    {
        ArgumentNullException.ThrowIfNull(importModel);
        
        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);
        
        if (importModel.SettingPersistence != DatabaseSettingPersistence.None)
        {
            package.AddMetadata("rdbms-import:entityNameConvention", importModel.EntityNameConvention);
            package.AddMetadata("rdbms-import:attributeNameConvention", importModel.AttributeNameConvention);
            package.AddMetadata("rdbms-import:tableStereotypes", importModel.TableStereotype);
            package.AddMetadata("rdbms-import:typesToExport", importModel.TypesToExport.Any() ? string.Join(";", importModel.TypesToExport.Select(t => t.ToString())) : "");
            package.AddMetadata("rdbms-import:importFilterFilePath", importModel.ImportFilterFilePath);
            package.AddMetadata("rdbms-import:storedProcedureType", importModel.StoredProcedureType);
            package.AddMetadata("rdbms-import:filterType", importModel.FilterType);
            ProcessConnectionStringSetting(package, importModel);
            package.AddMetadata("rdbms-import:settingPersistence", importModel.SettingPersistence.ToString());
            package.AddMetadata("rdbms-import:databaseType", importModel.DatabaseType.ToString());
        }
        else
        {
            package.RemoveMetadata("rdbms-import:entityNameConvention");
            package.RemoveMetadata("rdbms-import:attributeNameConvention");
            package.RemoveMetadata("rdbms-import:tableStereotypes");
            package.RemoveMetadata("rdbms-import:typesToExport");
            package.RemoveMetadata("rdbms-import:importFilterFilePath");
            package.RemoveMetadata("rdbms-import:storedProcedureType");
            package.RemoveMetadata("rdbms-import:filterType");
            package.RemoveMetadata("rdbms-import:connectionString");
            package.RemoveMetadata("rdbms-import:settingPersistence");
            package.RemoveMetadata("rdbms-import:databaseType");
        }
        
        package.Save();
        return;

        static void ProcessConnectionStringSetting(PackageModelPersistable package, DatabaseImportModel config)
        {
            var connectionString = config.ConnectionString;

            if (config.SettingPersistence == DatabaseSettingPersistence.AllSanitisedConnectionString)
            {
                var builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = connectionString;

                var addPassword = builder.Remove("Password");

                var sanitisedConnectionString = builder.ConnectionString;
                if (addPassword)
                {
                    sanitisedConnectionString = "Password=  ;" + sanitisedConnectionString;
                }

                connectionString = sanitisedConnectionString;
            }

            if (config.SettingPersistence == DatabaseSettingPersistence.AllWithoutConnectionString)
            {
                package.RemoveMetadata("rdbms-import:connectionString");
            }
            else
            {
                package.AddMetadata("rdbms-import:connectionString", connectionString);
            }
        }
    }
    
    public static void PersistSettings(RepositoryImportModel importModel)
    {
        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);

        if (importModel.SettingPersistence == RepositorySettingPersistence.None)
        {
            package.RemoveMetadata("rdbms-import-repository:storedProcedureType");
            package.RemoveMetadata("rdbms-import-repository:connectionString");
            package.RemoveMetadata("rdbms-import-repository:settingPersistence");
            package.RemoveMetadata("rdbms-import-repository:databaseType");
        }
        else
        {
            package.AddMetadata("rdbms-import-repository:storedProcedureType", importModel.StoredProcedureType);
            ProcessConnectionStringSetting(package, importModel);
            package.AddMetadata("rdbms-import-repository:settingPersistence", importModel.SettingPersistence.ToString());
            ProcessDatabaseTypeStringSetting(package, importModel);
        }
        
        package.Save();
        return;

        static void ProcessDatabaseTypeStringSetting(PackageModelPersistable package, RepositoryImportModel settings)
        {
            if (settings.SettingPersistence == RepositorySettingPersistence.InheritDb)
            {
                package.RemoveMetadata("rdbms-import-repository:databaseType");
            }
            else
            {
                package.AddMetadata("rdbms-import-repository:databaseType", settings.DatabaseType.ToString());
            }
        }
        
        static void ProcessConnectionStringSetting(PackageModelPersistable package, RepositoryImportModel settings)
        {
            var connectionString = settings.ConnectionString;

            if (settings.SettingPersistence == RepositorySettingPersistence.AllSanitisedConnectionString)
            {
                var builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = connectionString;

                var addPassword = builder.Remove("Password");

                var sanitisedConnectionString = builder.ConnectionString;
                if (addPassword)
                {
                    sanitisedConnectionString = "Password=  ;" + sanitisedConnectionString;
                }

                connectionString = sanitisedConnectionString;
            }

            if (settings.SettingPersistence == RepositorySettingPersistence.AllWithoutConnectionString)
            {
                package.RemoveMetadata("rdbms-import-repository:connectionString");
            }
            else
            {
                package.AddMetadata("rdbms-import-repository:connectionString", connectionString);
            }

            if (settings.SettingPersistence == RepositorySettingPersistence.InheritDb)
            {
                package.RemoveMetadata("rdbms-import-repository:connectionString");
            }
        }
    }
    
    public static void HydrateDbSettings(RepositoryImportModel importModel)
    {
        var package = LoadPackage(importModel.PackageFileName!);

        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = package.GetMetadataValue("rdbms-import:storedProcedureType");
        }

        importModel.ConnectionString = package.GetMetadataValue("rdbms-import:connectionString")!;
        importModel.DatabaseType = Enum.TryParse<DatabaseType>(package.GetMetadataValue("rdbms-import:databaseType")!, out var databaseType) ? databaseType : DatabaseType.SqlServer;
    }
    
    // We can't use PackageModelPersistable.Load since it uses the underlying cached versions
    // Also it should ONLY load the package as to prevent unfortunate model corruption
    private static PackageModelPersistable LoadPackage(string packagePath)
    {
        var package = XmlSerializationHelper.LoadFromFile<PackageModelPersistable>(packagePath, loadThisFileOnly: true, skipCache: true);
        foreach (var reference in package.References.Where(reference => !string.IsNullOrWhiteSpace(reference.RelativePath)))
        {
            reference.AbsolutePath = Path.GetFullPath(Path.Combine(package.DirectoryPath, reference.RelativePath));
        }
        return package;
    }
}