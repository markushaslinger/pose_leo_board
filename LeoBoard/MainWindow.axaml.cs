using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace LeoBoard;

public partial class MainWindow : Window
{
    private readonly Dictionary<CellId, TextBlock?> _cellContents;
    
    public MainWindow()
    {
        const int ExtraMargin = 4;
        
        _cellContents = new();
        Width = Board.Config.Columns * Config.CellSize + ExtraMargin;
        Height = Board.Config.Rows * Config.CellSize + ExtraMargin;
        
        InitializeComponent();
        
        Title = Board.Config.Title;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DrawGrid();
        MainCanvas.PointerPressed += HandleClick;
        Board.SetCellContentOnWindow = SetCellContent;
    }

    private void SetCellContent(int row, int col, string content, IBrush color)
    {
        var id = new CellId(row, col);
        var currentContent = _cellContents[id];
        if (currentContent is not null)
        {
            MainCanvas.Children.Remove(currentContent);
        }
        
        var textBlock = CreateCellContent(content, row, col, color);
        
        /*var left = (Config.CellSize - textBlock.Bounds.Width) / 2;
        var top = (Config.CellSize - textBlock.Bounds.Height) / 2;
        Canvas.SetLeft(textBlock, col * Config.CellSize + left);
        Canvas.SetTop(textBlock, row * Config.CellSize + top);*/

        MainCanvas.Children.Add(textBlock);
        
        _cellContents[id] = textBlock;
    }

    private static TextBlock CreateCellContent(string text, int row, int col, IBrush color)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = color,
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            RenderTransform = new TranslateTransform
            {
                X = (col  * Config.CellSize) + 0.25 * Config.CellSize,
                Y = (row  * Config.CellSize) + 0.125 * Config.CellSize
            }
        };

        return textBlock;
    }

    private void HandleClick(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var (row, col) = GetCell(e.GetPosition(this));
        var isLeftClick = !e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
        Board.Config.ClickHandler?.Invoke(row, col, isLeftClick);
    }

    private static (int row, int col) GetCell(Point clickPosition)
    {
        var col = Math.Floor(clickPosition.X / Config.CellSize);
        var row = Math.Floor(clickPosition.Y / Config.CellSize);
        return ((int) row, (int) col);
    }

    private void DrawGrid()
    {
        MainCanvas.Children.Clear();
        for (var row = 0; row < Board.Config.Rows; row++)
        {
            for (var col = 0; col < Board.Config.Columns; col++)
            {
                // we use rectangles instead of lines, because the space between lines is not clickable
                // but rectangles receive and propagate pointer events => which is what we want
                var rectangle = new Rectangle
                {
                    Width = Config.CellSize,
                    Height = Config.CellSize,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.25D
                };

                Canvas.SetLeft(rectangle, col * Config.CellSize);
                Canvas.SetTop(rectangle, row * Config.CellSize);
                MainCanvas.Children.Add(rectangle);
                _cellContents.Add(new(row, col), null);
            }
        }
    }
    
    private readonly record struct CellId(int Row, int Col);
}