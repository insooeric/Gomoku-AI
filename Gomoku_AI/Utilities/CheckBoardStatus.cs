using System.Numerics;

namespace Gomoku_AI.Utilities
{
    public class CheckBoardStatus
    {
        private const int WinningCount = 5;

        public static int CheckWinner(int[,] board)
        {

            if (
                CheckRows(board, 1) ||
                CheckColumns(board, 1) ||
                CheckDiagonals(board, 1) ||
                CheckAntiDiagonals(board, 1)
            )
            {
                return 1;
            }

            if (
                CheckRows(board, -1) ||
                CheckColumns(board, -1) ||
                CheckDiagonals(board, -1) ||
                CheckAntiDiagonals(board, -1)
            )
            {
                return -1;
            }

            return 0;
        }

        public static bool IsBoardFull(int[,] board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool CheckRows(int[,] board, int player)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                int count = 0;
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }
        private static bool CheckColumns(int[,] board, int player)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private static bool CheckDiagonals(int[,] board, int player)
        {
            for (int d = -board.GetLength(0) + 1; d < board.GetLength(1); d++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    int y = x + d;
                    if (y >= 0 && y < board.GetLength(1))
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= WinningCount) return true;
                    }
                }
            }
            return false;
        }
        private static bool CheckAntiDiagonals(int[,] board, int player)
        {
            for (int d = 0; d < board.GetLength(0) + board.GetLength(1) - 1; d++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    int y = d - x;
                    if (y >= 0 && y < board.GetLength(1))
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= WinningCount) return true;
                    }
                }
            }
            return false;
        }
    }
}
