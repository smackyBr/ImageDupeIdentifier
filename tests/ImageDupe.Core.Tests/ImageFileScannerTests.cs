using ImageDupe.Core.Scanning;

namespace ImageDupe.Core.Tests;

public class ImageFileScannerTests
{
    [Fact]
    public void Finds_images_in_subfolders_and_ignores_other_files()
    {
        using var temp = new TempFolder();
        temp.Write("a.jpg", "1");
        temp.Write("nested/deeper/b.CR3", "2");
        temp.Write("notes.txt", "3");
        temp.Write("clip.mp4", "4");

        var names = ImageFileScanner.EnumerateImages(temp.Path).Select(f => f.Name).Order().ToList();

        Assert.Equal(["a.jpg", "b.CR3"], names);
    }

    [Fact]
    public void Missing_folder_throws_a_clear_error()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Assert.Throws<DirectoryNotFoundException>(() => ImageFileScanner.EnumerateImages(missing).ToList());
    }
}
