using Avalonia.Media;

namespace GameOfLife;

using System;

public static class Program {
    private const int MAX_BOARD_SIZE = 80;
    private const int SIZE = 20;
    public const string CELL = "\u2588";
    //public const string CELL = "*";
    
    private static readonly IImmutableSolidColorBrush DrawingColor = Brushes.White;
    private static readonly IImmutableSolidColorBrush SimulationColor = Brushes.Blue;
    
    private static void Main() {
        Console.WriteLine("Game of Life");
        Console.WriteLine("============");
        Console.WriteLine("If you input a negative size, a random world with the positive size will be generated");
        int length;
        do {
            Console.Write($"Input size: [-{MAX_BOARD_SIZE}..{MAX_BOARD_SIZE}]: ");
        } while (!int.TryParse(Console.ReadLine(), out length) || Math.Abs(length) > MAX_BOARD_SIZE);

        if (length == 0) {
            Console.WriteLine("Can't create a 0-length world");
            return;
        }

        bool[,] world = length < 0 ? GameOfLife.GenerateRandomWorld(-length) : new bool[length, length];

        bool canDraw = true;
        length = Math.Abs(length);

        Console.WriteLine("\nInitializing LeoBoard...\n");

        /*
         * Documentation of currently used tools
         *  Start the Board with Board.Initialize(Main, optional)
         *      Main: The board suspends the thread until it's closed again
         *            The MainMethod parameter is the method that will be called while the board is open
         *
         *      Optional: Title: The Window Title
         *                Rows/Columns: The numbers of rows/columns of the window
         *                DrawGridNumbers: If the grid should be numbered
         *                ClickHandler (x, y, isLeft, unknown):
         *                  X/Y: The position where the click occured
         *                  Flag1: True if the right mouse button is pressed, False otherwise
         *                  Flag2: True if CTRL is pressed, False otherwise
         *
         *  Set a single cell in the Board with Board.SetCellContent(x, y, symbol, color)
         *      X/Y: The Position of the content
         *      Symbol: The symbol to write (only 1 char)
         *      Color: The color of the text (usage: Brushes.Color)
         *
         *
         *  Get the content of a single cell in the Board with Board.GetCellContent(x, y):
         *      X/Y: The Position to get the contents from
         *      Returns (string): The Character at the Position (X, Y)
         */
        LeoBoard.Board.Initialize(() => {
                GameOfLife.PrintWorld(world, DrawingColor);
                Console.WriteLine("Click on a point to toggle it");
                Console.WriteLine("\nPress any key to start the simulation...");
                Console.ReadKey(true);
                
                canDraw = false;

                Console.Write("\nPress any key to close...");
                while (!Console.KeyAvailable) {
                    world = GameOfLife.CalculateNextGeneration(world);
                    GameOfLife.PrintWorld(world, SimulationColor);
                    Task.Delay(100).Wait();
                }

                Environment.Exit(0);
            },
            clickHandler: (x, y, isNotRightClick, controlPressed) => {
                if (!canDraw) return;
                
                LeoBoard.Board.SetCellContent(x, y, world[x, y] ? " " : CELL, DrawingColor);
                world[x, y] = !world[x, y];
            },
            title: "Game of Life",
            rows: length,
            columns: length,
            cellSize: SIZE,
            fontSize: SIZE
        );
    }
}