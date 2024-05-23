// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Text;
using Avalonia.Media;
using LeoBoard;

const int BOARD_SIZE = 10;
object worldLock = new();
var world = new int[BOARD_SIZE, BOARD_SIZE];

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
    Console.WriteLine($"Points thread: {Thread.CurrentThread.ManagedThreadId}");
   
    for (int i = 0; i < world.GetLength(0); i++)
    {
        for(int j=0; j < world.GetLength(1); j++)
        {
            world[i, j] = Random.Shared.Next(0, 2);
        }
    }

    bool limitIter = true;
    int maxIter = 1000;
    Console.Write("\nPress any key to close...");
    Stopwatch sw = new Stopwatch();
    while (!Console.KeyAvailable && (!limitIter || maxIter-- > 0))
    {
        ModifyWorld(world);
        sw.Restart();
        PrintWorld(world);
        sw.Stop();
        Console.WriteLine("Elapsed time: {0} ms / {1} fps", sw.ElapsedMilliseconds, 1000 / sw.ElapsedMilliseconds);
    }
}

void PrintWorld(int[,] bools)
{
    for(int i = 0; i<bools.GetLength(0); i++)
    {
        for(int j = 0; j<bools.GetLength(1); j++)
        {
            if (bools[i, j] != 2)
            {
                Board.SetCellContent(i, j, bools[i, j] == 0 ? "X" : " ", Brushes.Black);
            }
        }
    }
}

void ModifyWorld(int[,] bools)
{
    for (int i = 0; i < bools.GetLength(0); i++)
    {
        for (int j = 0; j < bools.GetLength(1); j++)
        {
            if(bools[i,j] != 2)
                bools[i, j] = Random.Shared.Next(0, 2);
        }
    }

}

void SetWorld(int row, int col, int value)
{
    lock (worldLock)
    {
        world[row, col] = value;
    }
}

void HandleClick(int row, int col, bool leftClick, bool ctrlKeyPressed)
{
    Console.WriteLine($"HandleClick API thread: {Thread.CurrentThread.ManagedThreadId}");
    string ctrlState = ctrlKeyPressed ? " while pressing the Ctrl key" : string.Empty;
    Console.WriteLine($"Clicked cell ({row}, {col}) with {(leftClick ? "left" : "right")} mouse button{ctrlState}");
    if (leftClick)
    {
        SetWorld(row, col, 2);
        Board.SetCellContent(row, col, "A", Brushes.Green);
    }
    else
    {
        SetWorld(row, col, 0);
        Board.SetCellContent(row, col, "B", Brushes.Purple);
    }
}