using System;
using System.Linq;
using System.Threading;

class Tetris
{
    static int width = 20;  // Increased width
    static int height = 20;
    static int[,] board = new int[height, width];
    static Random random = new Random();
    static int[,] currentPiece;
    static int currentPieceRow, currentPieceCol;

    static void Main()
    {
        StartGame();
    }

    static void StartGame()
    {
        while (true)
        {
            SpawnPiece();
            while (true)
            {
                DrawBoard();
                Thread.Sleep(200); // Adjust speed for how fast the pieces drop

                // Check for user input
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            MovePiece(-1);
                            break;
                        case ConsoleKey.RightArrow:
                            MovePiece(1);
                            break;
                        case ConsoleKey.DownArrow:
                            MovePieceDown();
                            break;
                        case ConsoleKey.UpArrow:
                            RotatePiece();
                            break;
                    }
                }

                // Attempt to move the piece down
                if (!MovePieceDown())
                {
                    PlacePiece();
                    ClearLines();
                    break; // Spawn a new piece
                }
            }
        }
    }

    static void SpawnPiece()
    {
        currentPiece = GetRandomPiece();
        currentPieceRow = 0;
        currentPieceCol = width / 2 - 1;

        // Check for game over
        if (!CanMove(currentPieceRow, currentPieceCol))
        {
            Console.Clear();
            Console.WriteLine("Game Over!");
            Environment.Exit(0);
        }
    }

    static bool MovePiece(int direction)
    {
        if (CanMove(currentPieceRow, currentPieceCol + direction))
        {
            currentPieceCol += direction;
            return true;
        }
        return false;
    }

    static bool MovePieceDown()
    {
        if (CanMove(currentPieceRow + 1, currentPieceCol))
        {
            currentPieceRow++;
            return true;
        }
        return false;
    }

    static void PlacePiece()
    {
        for (int row = 0; row < currentPiece.GetLength(0); row++)
        {
            for (int col = 0; col < currentPiece.GetLength(1); col++)
            {
                if (currentPiece[row, col] > 0)
                {
                    board[currentPieceRow + row, currentPieceCol + col] = currentPiece[row, col];
                }
            }
        }
    }

    static void ClearLines()
    {
        for (int row = height - 1; row >= 0; row--)
        {
            bool lineComplete = true;
            for (int col = 0; col < width; col++)
            {
                if (board[row, col] == 0)
                {
                    lineComplete = false;
                    break;
                }
            }

            if (lineComplete)
            {
                // Shift all rows down
                for (int r = row; r > 0; r--)
                {
                    for (int c = 0; c < width; c++)
                    {
                        board[r, c] = board[r - 1, c];
                    }
                }
                // Clear the top row
                for (int c = 0; c < width; c++)
                {
                    board[0, c] = 0;
                }
                row++; // Check the same row again
            }
        }
    }

    static void DrawBoard()
    {
        Console.Clear();
        // Draw top border
        Console.WriteLine(new string('-', width + 2));

        for (int row = 0; row < height; row++)
        {
            Console.Write("|"); // Left border
            for (int col = 0; col < width; col++)
            {
                // Check if the cell is part of the current piece
                if (IsCurrentPiece(row, col))
                {
                    Console.Write("#"); // Draw current piece
                }
                else
                {
                    Console.Write(board[row, col] > 0 ? "#" : " "); // Draw the board
                }
            }
            Console.WriteLine("|"); // Right border
        }

        // Draw bottom border
        Console.WriteLine(new string('-', width + 2));
    }

    static bool IsCurrentPiece(int row, int col)
    {
        for (int r = 0; r < currentPiece.GetLength(0); r++)
        {
            for (int c = 0; c < currentPiece.GetLength(1); c++)
            {
                if (currentPiece[r, c] > 0)
                {
                    int pieceRow = currentPieceRow + r;
                    int pieceCol = currentPieceCol + c;
                    if (pieceRow == row && pieceCol == col)
                    {
                        return true; // The cell is part of the current piece
                    }
                }
            }
        }
        return false;
    }

    static bool CanMove(int newRow, int newCol)
    {
        for (int row = 0; row < currentPiece.GetLength(0); row++)
        {
            for (int col = 0; col < currentPiece.GetLength(1); col++)
            {
                if (currentPiece[row, col] > 0)
                {
                    int targetRow = newRow + row;
                    int targetCol = newCol + col;
                    if (targetRow >= height || targetCol < 0 || targetCol >= width || board[targetRow, targetCol] > 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    static void RotatePiece()
    {
        int[,] rotatedPiece = new int[currentPiece.GetLength(1), currentPiece.GetLength(0)];
        for (int row = 0; row < currentPiece.GetLength(0); row++)
        {
            for (int col = 0; col < currentPiece.GetLength(1); col++)
            {
                rotatedPiece[col, currentPiece.GetLength(0) - 1 - row] = currentPiece[row, col];
            }
        }

        if (CanMove(currentPieceRow, currentPieceCol)) // Check if the piece can be placed
        {
            currentPiece = rotatedPiece;
        }
    }

    static int[,] GetRandomPiece()
    {
        int pieceIndex = random.Next(0, 7);
        return pieceIndex switch
        {
            0 => new[,] { { 1, 1, 1, 1 } }, // I
            1 => new[,] { { 1, 1, 1 }, { 0, 1, 0 } }, // T
            2 => new[,] { { 1, 1 }, { 1, 1 } }, // O
            3 => new[,] { { 0, 1, 1 }, { 1, 1, 0 } }, // S
            4 => new[,] { { 1, 1, 0 }, { 0, 1, 1 } }, // Z
            5 => new[,] { { 1, 0, 0 }, { 1, 1, 1 } }, // L
            6 => new[,] { { 0, 0, 1 }, { 1, 1, 1 } }, // J
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
