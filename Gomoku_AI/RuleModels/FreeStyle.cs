namespace Gomoku_AI.RuleModels
{
    public class FreeStyle
    {
        private readonly int boardSizeX;
        private readonly int boardSizeY;
        private const int WinningCount = 5;

        public FreeStyle(int boardSizeX, int boardSizeY)
        {
            this.boardSizeX = boardSizeX;
            this.boardSizeY = boardSizeY;
        }

        public bool IsWinning(int[,] board, int player)
        {
            return CheckRows(board, player) ||
                   CheckColumns(board, player) ||
                   CheckDiagonals(board, player) ||
                   CheckAntiDiagonals(board, player);
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
