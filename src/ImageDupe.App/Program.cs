using Avalonia;

namespace ImageDupe.App;

internal static class Program
{
    // Don't use Avalonia or any third-party APIs before AppMain is called:
    // things aren't initialized yet.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Also used by the XAML previewer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
