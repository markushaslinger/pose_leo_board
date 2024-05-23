// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Text;
using Avalonia.Media;
using LeoBoard;

const int BOARD_SIZE = 60;

Console.WriteLine("Hello, Leoboard!!");

Board.Initialize(Points, "Foo", BOARD_SIZE, BOARD_SIZE, 
    20, 
    12, 
    true, 
    0,
    clickHandler: HandleClick);

return;

void Run()
{
    Console.OutputEncoding = Encoding.UTF8;
    
    Board.SetCellContent(0, 0, "X");
    Board.SetCellContent(1, 1, "Y", Brushes.Red);
    Board.SetCellContent(2, 2, "Z", Brushes.Blue);
    
    Console.WriteLine(Board.GetCellContent(2, 2));
    
    Board.ShowMessageBox("Hello World!");
    
    Console.ReadKey();
}

void Points()
{
    var world = new bool[BOARD_SIZE, BOARD_SIZE];
    for (int i = 0; i < world.GetLength(0); i++)
    {
        for(int j=0; j < world.GetLength(1); j++)
        {
            world[i, j] = Random.Shared.Next(0, 2) == 0;
        }
    }

    Console.Write("\nPress any key to close...");
    Stopwatch sw = new Stopwatch();
    while (!Console.KeyAvailable)
    {
        ModifyWorld(world);
        sw.Restart();
        PrintWorld(world);
        sw.Stop();
        Console.WriteLine("Elapsed time: {0} ms / {1} fps", sw.ElapsedMilliseconds, 1000 / sw.ElapsedMilliseconds);
        //Task.Delay(500).Wait();
    }
}

void PrintWorld(bool[,] bools)
{
    for(int i = 0; i<bools.GetLength(0); i++)
    {
        for(int j = 0; j<bools.GetLength(1); j++)
        {
            Board.SetCellContent(i, j, bools[i, j] ? "X" : " ", Brushes.Black);
        }
    }
}

void ModifyWorld(bool[,] bools)
{
    for (int i = 0; i < bools.GetLength(0); i++)
    {
        for (int j = 0; j < bools.GetLength(1); j++)
        {
            bools[i, j] = Random.Shared.Next(0, 2) == 0;
        }
    }

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