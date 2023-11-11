using Avalonia.Media;
using LeoBoard;

await Board.Initialize("Foo", 10, 20, HandleClick);

await Task.Delay(1000);

Board.SetCellContent(0, 0, "X");
Board.SetCellContent(1, 1, "Y", Brushes.Red);
Board.SetCellContent(2, 2, "Z", Brushes.Blue);

Console.ReadKey();

void HandleClick(int row, int col, bool leftClick)
{
    Console.WriteLine($"Clicked cell ({row}, {col}) with {(leftClick ? "left" : "right")} mouse button");
}