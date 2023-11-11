using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;

namespace LeoBoard;

public static class Board
{
    public static bool Initialized { get; internal set; }
    private static Config? _config;
    internal static Config Config => _config ?? throw new InvalidOperationException("No config set");
    internal static Action<int, int, string, IBrush>? SetCellContentOnWindow { get; set; }
    
    public static async Task Initialize(string title = "LeoBoard", int rows = 8, int columns = 8, 
                                        Action<int, int, bool>? clickHandler = null)
    {
        if (Initialized)
        {
            throw new InvalidOperationException("Already initialized");
        }
        
        _config = new(title, rows, columns, clickHandler);
        
        OpenWindow();
        
        var waitStart = DateTime.Now;
        var maxWait = waitStart + TimeSpan.FromSeconds(10);
        while (!Initialized && DateTime.Now < maxWait)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(125));
        }

        if (!Initialized)
        {
            Console.WriteLine("Failed to initialize application window!");
        }
    }
    
    public static void SetCellContent(int row, int col, string content, IBrush? color = null)
    {
        if (SetCellContentOnWindow is null)
        {
            throw new InvalidOperationException("Handler for setting cell content not set");
        }
        
        Dispatcher.UIThread.Invoke(() =>
        {
            SetCellContentOnWindow.Invoke(row, col, content, color ?? Brushes.Black);
        });
    }

    private static void OpenWindow()
    {
        // separate thread to allow interaction
        Task.Run(() =>
        {
            try
            {
                var exitCode = BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(Array.Empty<string>());
                Console.WriteLine($"Window exited with code {exitCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });

        return;
        
        static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .WithInterFont()
                         .LogToTrace();
    } 
}

internal sealed record Config(string Title, int Rows, int Columns, Action<int, int, bool>? ClickHandler)
{
    public const int CellSize = 20;
}