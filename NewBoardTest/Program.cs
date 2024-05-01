using Avalonia.Media;
using LeoBoard;

Board.Initialize(Run, "Foo", 10, 20,
                 clickHandler: HandleClick,
                 drawGridNumbers: true);

return;

void Run()
{
    Board.SetCellContent(0, 0, "X");
    Board.SetCellContent(1, 1, "Y", Brushes.Red);
    Board.SetCellContent(2, 2, "Z", Brushes.Blue);
    Board.SetCellContent(2, 2, "⬤", Brushes.Blue);

    Console.WriteLine(Board.GetCellContent(2, 2));

    Board.ShowMessageBox("Hello World!");

    Console.ReadKey();
}

void HandleClick(int row, int col, bool leftClick, bool ctrlKeyPressed)
{
    string ctrlState = ctrlKeyPressed ? " while pressing the Ctrl key" : string.Empty;
    Console.WriteLine($"Clicked cell ({row}, {col}) with {(leftClick ? "left" : "right")} mouse button{ctrlState}");
    if (leftClick)
    {
        Board.SetCellContent(row, col, "A", Brushes.Green);
    }
    else
    {
        Board.SetCellContent(row, col, "B", Brushes.Purple);
    }
}
