using System;
using System.IO;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal record AccessResult(bool Allowed, string? Reason = null);

internal class FileAccessChecks
{
     /// <summary>
    /// Returns true iff the current process identity can:
    ///  - open an existing file for write (without modifying its contents), OR
    ///  - create a new file at the target path and then remove it.
    /// Cross-platform (.NET 6+). Does not modify content of existing files.
    /// </summary>
    public static AccessResult CanCreateOrWriteToFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return new(false, "Path is null or empty.");

        try
        {
            // Normalize early so we can reliably find the parent directory
            var full = Path.GetFullPath(path);

            if (File.Exists(full))
            {
                // Prove write permission WITHOUT writing any data.
                // Opening with FileAccess.Write is sufficient.
                using var fs = new FileStream(
                    full,
                    FileMode.Open,
                    FileAccess.Write,
                    FileShare.None);

                _ = fs.CanWrite;
                return new(true);
            }
            else
            {
                var dir = Path.GetDirectoryName(full);
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                    return new(false, "Parent directory does not exist.");

                // Try to create and then delete the file.
                // We avoid FileOptions.DeleteOnClose for maximum cross-OS compatibility.
                // If another process races to create the file, treat that as not allowed for this operation.
                bool created = false;
                try
                {
                    using var fs = new FileStream(
                        full,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 1);

                    created = true;
                    // No write; creation itself proves the permission.
                }
                finally
                {
                    if (created && File.Exists(full))
                    {
                        try { File.Delete(full); } catch { /* swallow: best effort */ }
                    }
                }

                return new(true);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            return new(false, $"Unauthorized: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            return new(false, $"Directory not found: {ex.Message}");
        }
        catch (PathTooLongException ex)
        {
            return new(false, $"Path too long: {ex.Message}");
        }
        catch (IOException ex)
        {
            // Includes sharing violations, read-only attributes, locks, SMB share perms, etc.
            return new(false, $"I/O error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new(false, $"Unexpected error: {ex.Message}");
        }
    }
}