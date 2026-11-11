namespace ImageDupe.Core.Duplicates;

/// <summary>A set of files whose bytes are identical.</summary>
/// <param name="Sha256">Hex SHA-256 of the file contents.</param>
/// <param name="SizeBytes">Size of each file in the group.</param>
/// <param name="Files">Two or more full file paths, sorted.</param>
public sealed record DuplicateGroup(string Sha256, long SizeBytes, IReadOnlyList<string> Files)
{
    /// <summary>Disk space recovered if all but one copy were removed.</summary>
    public long ReclaimableBytes => SizeBytes * (Files.Count - 1);
}
