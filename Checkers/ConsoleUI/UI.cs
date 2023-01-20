using Domain;
using GameBrain;
using Utilities;

namespace ConsoleUI;

public class UI
{
    public static void DrawGameBoard(CheckersBrain brain)
    {
        Console.Clear();
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var board = brain.GetBoard();
        var width = brain.Options.Width;
        var height = brain.Options.Height;

        // add letter for each column
        for (var i = 0; i < height; i++)
            Console.Write($" {(char)(65 + i)} ");

        Console.WriteLine();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                // highlight selected piece
                if (brain.Piece?.X == x && brain.Piece?.Y == y)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.BackgroundColor = ConsoleColor.Black;

                switch (board[x, y])
                {
                    case EGamePiece.Empty:
                        Console.Write("   ");
                        break;
                    case EGamePiece.Player1:
                        Console.Write(" ⦿ ");
                        break;
                    case EGamePiece.King1:
                        Console.Write(" ♛ ");
                        break;
                    case EGamePiece.Player2:
                        Console.Write(" ⦾ ");
                        break;
                    case EGamePiece.King2:
                        Console.Write(" ♕ ");
                        break;
                    case EGamePiece.Playable:
                    {
                        if (brain.Options.HighlightMoves)
                            Console.BackgroundColor = ConsoleColor.DarkGreen;

                        Console.Write("   ");
                        break;
                    }
                    case EGamePiece.Takeable:
                    {
                        if (brain.Options.HighlightMoves)
                            Console.ForegroundColor = ConsoleColor.DarkRed;

                        Console.Write(brain.State.Turn == EGamePiece.Player1 ? " ⦾ " : " ⦿ ");
                        break;
                    }
                    case EGamePiece.TakeableKing:
                    {
                        if (brain.Options.HighlightMoves)
                            Console.ForegroundColor = ConsoleColor.DarkRed;

                        Console.Write(brain.State.Turn == EGamePiece.Player1 ? " ♕ " : " ♛ ");
                        break;
                    }
                    default:
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("   ");
                        break;
                }

                Console.ResetColor();
            }

            // add number for each row
            Console.Write($" {x}\n");
        }

        Util.WriteLineColored($"\nPlayer1 score: {brain.State.Player1Score}", ConsoleColor.DarkCyan);
        Util.WriteLineColored($"Player2 score: {brain.State.Player2Score}\n", ConsoleColor.DarkCyan);

        if (brain.State.Winner == null)
            Util.WriteLineColored($"{brain.State.Turn}'s turn\n", ConsoleColor.Yellow);
    }
}