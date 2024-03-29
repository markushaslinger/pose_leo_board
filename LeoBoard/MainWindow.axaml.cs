using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace LeoBoard;

internal partial class MainWindow : Window
{
    private readonly Dictionary<CellId, TextBlock?> _cellContents;
    private readonly int _effectiveColumns;
    private readonly int _effectiveRows;
    private readonly bool _drawGridNumbers;
    
    public MainWindow()
    {
        const int ExtraMargin = 4;
        
        _cellContents = new();
        _effectiveColumns = Board.Config.DrawGridNumbers
            ? Board.Config.Columns + 1
            : Board.Config.Columns;
        _effectiveRows = Board.Config.DrawGridNumbers
            ? Board.Config.Rows + 1
            : Board.Config.Rows;
        Width = Math.Max(Config.MinWidth, _effectiveColumns * Board.Config.CellSize + ExtraMargin);
        Height = Math.Max(Config.MinHeight, _effectiveRows * Board.Config.CellSize + ExtraMargin);
        MinWidth = Width;
        MinHeight = Height;
        _drawGridNumbers = Board.Config.DrawGridNumbers;
        
        InitializeComponent();
        
        Title = Board.Config.Title;
        CanResize = false;
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

        MainCanvas.Children.Add(textBlock);
        
        _cellContents[id] = textBlock;
    }

    private TextBlock CreateCellContent(string text, int row, int col, IBrush color)
    {
        var xOffset = text.Length switch
                      {
                          1 => 0.25D,
                          2 => 0.125D,
                          _ => throw new ArgumentOutOfRangeException(nameof(text), "Cell content can not exceed 2 characters")
                      };
        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = color,
            FontSize = Board.Config.FontSize,
            FontWeight = FontWeight.Bold,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            FontFamily = new FontFamily("Consolas, Courier New, Courier"),
            RenderTransform = new TranslateTransform
            {
                X = (col  * Board.Config.CellSize) + (xOffset * Board.Config.CellSize) + Board.Config.ExtraXTextOffset,
                Y = (row  * Board.Config.CellSize) + 0.25D * Board.Config.CellSize
            }
        };

        return textBlock;
    }

    private void HandleClick(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var (row, col) = GetCell(e.GetPosition(this));
        
        if (_drawGridNumbers)
        {
            if (row == 0 || col == 0)
            {
                return;
            }
            
            row--;
            col--;
        }
        
        var isLeftClick = !e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
        Board.Config.ClickHandler?.Invoke(row, col, isLeftClick);
    }

    private static (int row, int col) GetCell(Point clickPosition)
    {
        var col = Math.Floor(clickPosition.X / Board.Config.CellSize);
        var row = Math.Floor(clickPosition.Y / Board.Config.CellSize);
        return ((int) row, (int) col);
    }

    private void DrawGrid()
    {
        MainCanvas.Children.Clear();
        for (var row = 0; row < _effectiveRows; row++)
        {
            for (var col = 0; col < _effectiveColumns; col++)
            {
                // we use rectangles instead of lines, because the space between lines is not clickable
                // but rectangles receive and propagate pointer events => which is what we want
                var rectangle = new Rectangle
                {
                    Width = Board.Config.CellSize,
                    Height = Board.Config.CellSize,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.25D
                };

                Canvas.SetLeft(rectangle, col * Board.Config.CellSize);
                Canvas.SetTop(rectangle, row * Board.Config.CellSize);
                MainCanvas.Children.Add(rectangle);
                _cellContents.Add(new(row, col), null);
                
                if (!_drawGridNumbers || (row != 0 && col != 0))
                {
                    continue;
                }

                if (row == 0 && col > 0)
                {
                    SetCellContent(0, col, FormatGridNumber(col), Brushes.DimGray);    
                }
                else if (col == 0 && row > 0)
                {
                    SetCellContent(row, 0, FormatGridNumber(row), Brushes.DimGray);
                }
            }
        }
        
        return;
        
        static string FormatGridNumber(int n) => n.ToString("00");
    }
    
    private readonly record struct CellId(int Row, int Col);
}