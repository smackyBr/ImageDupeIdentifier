using ImageDupe.Core.Duplicates;
using ImageDupe.Core.Scanning;

namespace ImageDupe.Core.Tests;

public class ExactDuplicateFinderTests
{
    [Fact]
    public async Task Groups_identical_files_and_leaves_unique_ones_out()
    {
        using var temp = new TempFolder();
        var a = temp.Write("a.jpg", "same bytes");
        var b = temp.Write("copies/b.jpg", "same bytes");
        temp.Write("c.jpg", "different!"); // same size as a/b, different content
        temp.Write("d.jpg", "unique size, unique content");

        var groups = await ExactDuplicateFinder.FindAsync(ImageFileScanner.EnumerateImages(temp.Path));

        var group = Assert.Single(groups);
        Assert.Equal(new[] { a, b }.Order(StringComparer.OrdinalIgnoreCase), group.Files);
        Assert.Equal("same bytes".Length, group.ReclaimableBytes);
    }

    [Fact]
    public async Task Finding_duplicates_never_modifies_files()
    {
        using var temp = new TempFolder();
        var a = temp.Write("a.jpg", "same");
        temp.Write("b.jpg", "same");
        var before = File.GetLastWriteTimeUtc(a);

        await ExactDuplicateFinder.FindAsync(ImageFileScanner.EnumerateImages(temp.Path));

        Assert.True(File.Exists(a));
        Assert.Equal(before, File.GetLastWriteTimeUtc(a));
    }
}
