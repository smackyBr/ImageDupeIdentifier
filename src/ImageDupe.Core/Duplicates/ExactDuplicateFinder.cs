using System.Security.Cryptography;

namespace ImageDupe.Core.Duplicates;

/// <summary>
/// Finds byte-for-byte identical files. Read-only.
/// Strategy for large libraries (100K+ photos): group by file size first (free - no reading),
/// and only hash files that share a size with at least one other file.
/// </summary>
public static class ExactDuplicateFinder
{
    /// <summary>Progress report: how many candidate files have been hashed so far, out of how many.</summary>
    public readonly record struct Progress(int Hashed, int TotalToHash);

    public static async Task<IReadOnlyList<DuplicateGroup>> FindAsync(
        IEnumerable<FileInfo> files,
        IProgress<Progress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var candidates = files
            .GroupBy(f => f.Length)
            .Where(g => g.Key > 0 && g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        var byHash = new Dictionary<(string Hash, long Size), List<string>>();
        var hashed = 0;

        foreach (var file in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hash = await TryHashAsync(file, cancellationToken);
            hashed++;
            progress?.Report(new Progress(hashed, candidates.Count));

            if (hash is null)
            {
                continue; // file vanished or became unreadable mid-scan; skip it
            }

            var key = (hash, file.Length);
            if (!byHash.TryGetValue(key, out var list))
            {
                list = [];
                byHash[key] = list;
            }
            list.Add(file.FullName);
        }

        return byHash
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => new DuplicateGroup(
                kv.Key.Hash,
                kv.Key.Size,
                kv.Value.Order(StringComparer.OrdinalIgnoreCase).ToList()))
            .OrderByDescending(g => g.ReclaimableBytes)
            .ToList();
    }

    private static async Task<string?> TryHashAsync(FileInfo file, CancellationToken cancellationToken)
    {
        try
        {
            // Read-only, and lets other programs keep reading the file while we hash it.
            await using var stream = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 1 << 16,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            var bytes = await SHA256.HashDataAsync(stream, cancellationToken);
            return Convert.ToHexString(bytes);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
