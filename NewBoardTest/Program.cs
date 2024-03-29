using Avalonia.Media;
using LeoBoard;

await Board.Initialize("Foo", 10, 20, 
                       clickHandler: HandleClick,
                       drawGridNumbers: true);

Board.SetCellContent(0, 0, "X");
Board.SetCellContent(1, 1, "Y", Brushes.Red);
Board.SetCellContent(2, 2, "Z", Brushes.Blue);

Console.WriteLine(Board.GetCellContent(2, 2));

Console.ReadKey();

return;

void HandleClick(int row, int col, bool leftClick)
{
    Console.WriteLine($"Clicked cell ({row}, {col}) with {(leftClick ? "left" : "right")} mouse button");
    if (leftClick)
    {
        Board.SetCellContent(row, col, "A", Brushes.Green);
    }
    else
    {
        Board.SetCellContent(row, col, "B", Brushes.Purple);
    }
}