using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageDupe.App.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private string _status = "Ready. Works fully offline - your photos never leave this computer.";

    public string Title => "Image Dupe Identifier";

    public string VersionText { get; } =
        "Version " + (Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "dev");

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}
