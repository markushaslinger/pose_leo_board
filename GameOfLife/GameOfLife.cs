using Avalonia.Media;

namespace GameOfLife;

public static class GameOfLife {
    public static bool[,] CalculateNextGeneration(bool[,] world) {
        bool[,] newWorld = new bool[world.GetLength(0), world.GetLength(1)];
        Array.Copy(world, newWorld, world.Length);

        for (int i = 0; i < world.GetLength(0); i++) {
            for (int j = 0; j < world.GetLength(1); j++) {
                // if the cell is alive and doesn't have 2 or 3 neighbours, it dies
                if (world[i, j] && CountNeighbours(world, i, j) is not 2 and not 3) newWorld[i, j] = false;

                // if the cell is dead and has 3 neighbours, it lives
                if (!world[i, j] && CountNeighbours(world, i, j) == 3) newWorld[i, j] = true;
            }
        }

        return newWorld;
    }

    private static int CountNeighbours(bool[,] world, int row, int col) {
        int[] rows = [row - 1, row, row + 1];
        int[] cols = [col - 1, col, col + 1];
            
        return (from r in rows
            from c in cols
            where r != row || c != col
            let currentRow = (r + world.GetLength(0)) % world.GetLength(0)
            let currentCol = (c + world.GetLength(1)) % world.GetLength(1)
            where world[currentRow, currentCol]
            select 0).Count();
    }

    public static bool[,] GenerateRandomWorld(int length) {
        bool[,] world = new bool[length, length];
        for (int i = 0; i < length; i++) {
            for (int j = 0; j < length; j++) {
                // Random.Shared.Next is exclusive, so this will generate either 0 or 1
                world[i, j] = Random.Shared.Next(0, 2) == 0;
            }
        }

        return world;
    }

    public static void PrintWorld(bool[,] world, IImmutableSolidColorBrush color) {
        for (int i = 0; i < world.GetLength(0); i++) {
            for (int j = 0; j < world.GetLength(1); j++) {
                LeoBoard.Board.SetCellContent(i, j, world[i, j] ? Program.CELL : " ", color);
            }
        }
    }
}