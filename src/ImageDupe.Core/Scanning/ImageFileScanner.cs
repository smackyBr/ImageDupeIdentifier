namespace ImageDupe.Core.Scanning;

/// <summary>
/// Finds image files under a folder. Read-only: it never opens, changes, or moves anything.
/// </summary>
public static class ImageFileScanner
{
    /// <summary>File extensions treated as photos (case-insensitive).</summary>
    public static readonly IReadOnlySet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".heic", ".heif",
        // Camera RAW formats
        ".dng", ".cr2", ".cr3", ".nef", ".arw", ".raf", ".orf", ".rw2",
    };

    /// <summary>
    /// Lazily enumerates image files under <paramref name="rootFolder"/> and all subfolders.
    /// Skips folders it can't read, hidden/system items, and symbolic links / junctions
    /// (so a scan can never loop forever or wander outside the folder the user chose).
    /// </summary>
    public static IEnumerable<FileInfo> EnumerateImages(string rootFolder, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootFolder);

        var root = new DirectoryInfo(Path.GetFullPath(rootFolder));
        if (!root.Exists)
        {
            throw new DirectoryNotFoundException($"Folder not found: {root.FullName}");
        }

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint,
            ReturnSpecialDirectories = false,
        };

        foreach (var file in root.EnumerateFiles("*", options))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ImageExtensions.Contains(file.Extension))
            {
                yield return file;
            }
        }
    }
}
