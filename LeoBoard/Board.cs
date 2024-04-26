using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LeoBoard;

public static class Board
{
    private static readonly TimeSpan initWaitInterval = TimeSpan.FromMilliseconds(125);
    private static readonly TimeSpan initWaitMax = TimeSpan.FromSeconds(10);
    private static Config? _config;
    private static readonly ValueCache cellValues = new();

    public static bool Initialized { get; internal set; }
    internal static Config Config => _config ?? throw new BoardException("No config set");
    internal static Action<int, int, string, IBrush>? SetCellContentOnWindow { get; set; }

    public static void InitializeForTest(int rows, int columns)
    {
        Initialize(new ("Test", rows, columns, 20, 12, 0, 
                        false, true, null));
    }

    private static void Initialize(Config config)
    {
        _config = config;
        _config.EnsureValid();
        cellValues.Clear();
    }

    public static async Task Initialize(string title = "LeoBoard", int rows = 8, int columns = 8, int cellSize = 20,
                                        int fontSize = 12, bool drawGridNumbers = false, int extraXTextOffset = 0,
                                        Action<int, int, bool>? clickHandler = null, int afterInitWaitSeconds = 1)
    {
        if (Initialized)
        {
            throw new BoardException("Already initialized");
        }

        Initialize(new(title, rows, columns, cellSize, fontSize, extraXTextOffset, 
                       drawGridNumbers, false, clickHandler));

        OpenWindow();

        var waitStart = GetNow();
        var maxWait = waitStart + initWaitMax;
        while (!Initialized && GetNow() < maxWait)
        {
            await Task.Delay(initWaitInterval);
        }

        if (!Initialized)
        {
            Console.WriteLine("Failed to initialize application window!");
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Max(afterInitWaitSeconds, 0)));
        }
    }

    public static void SetCellContent(int row, int col, string content, IBrush? color = null)
    {
        if (content.Length > 1)
        {
            throw new BoardException("Cell content may only be a single character");
        }

        ThrowIfCellNotInRange(row, col);
        cellValues[row, col] = content;

        if (Config.TestMode)
        {
            return;
        }
        
        if (SetCellContentOnWindow is null)
        {
            throw new BoardException("Handler for setting cell content not set");
        }
        
        if (Config.DrawGridNumbers)
        {
            row++;
            col++;
        }

        Dispatcher.UIThread.Invoke(() => { SetCellContentOnWindow.Invoke(row, col, content, color ?? Brushes.Black); });
    }

    public static string GetCellContent(int row, int col)
    {
        ThrowIfCellNotInRange(row, col);
        return cellValues[row, col];
    } 

    public static void ShowMessageBox(string message, string title = "Information")
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            try
            {
                var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok);
                await box.ShowAsync();
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });
    }

    private static void ThrowIfCellNotInRange(int row, int col)
    {
        if (row < 0 || row >= Config.Rows || col < 0 || col >= Config.Columns)
        {
            throw new BoardException($"Cell ({row}, {col}) is out of range");
        }
    }

    private static void OpenWindow()
    {
        // separate thread to allow interaction
        Task.Run(() =>
        {
            try
            {
                var exitCode = BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime([]);
                Console.WriteLine($"Window exited with code {exitCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });

        return;

        static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .WithInterFont()
                      .LogToTrace();
    }

    private static DateTimeOffset GetNow() => DateTimeOffset.UtcNow;
}

public sealed class BoardException(string message) : InvalidOperationException(message);