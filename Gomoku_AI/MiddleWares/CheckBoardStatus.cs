using System.Numerics;

namespace Gomoku_AI.MiddleWares
{
    public class CheckBoardStatus
    {
        private readonly int boardSizeX;
        private readonly int boardSizeY;
        private const int WinningCount = 5;

        public CheckBoardStatus(int boardSizeX, int boardSizeY)
        {
            this.boardSizeX = boardSizeX;
            this.boardSizeY = boardSizeY;
        }

        public int CheckWinner(int[,] board)
        {
            int blackCount = board.Cast<int>().Count(v => v == 1);
            int whiteCount = board.Cast<int>().Count(v => v == -1);

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

        public bool IsBoardFull(int[,] board)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
                {
                    if (board[x, y] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckRows(int[,] board, int player)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                int count = 0;
                for (int y = 0; y < boardSizeY; y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }
        private bool CheckColumns(int[,] board, int player)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private bool CheckDiagonals(int[,] board, int player)
        {
            for (int d = -boardSizeX + 1; d < boardSizeY; d++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    int y = x + d;
                    if (y >= 0 && y < boardSizeY)
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= WinningCount) return true;
                    }
                }
            }
            return false;
        }
        private bool CheckAntiDiagonals(int[,] board, int player)
        {
            for (int d = 0; d < boardSizeX + boardSizeY - 1; d++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    int y = d - x;
                    if (y >= 0 && y < boardSizeY)
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
