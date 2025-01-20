namespace Gomoku_AI.RuleModels
{
    public class FreeStyle : IRule
    {
        private const int WinningCount = 5;

        public FreeStyle()
        {
        }

        public bool IsWinning(int[,] board, int player)
        {
            bool hasWon = CheckRows(board, player) ||
                          CheckColumns(board, player) ||
                          CheckDiagonals(board, player) ||
                          CheckAntiDiagonals(board, player);

            if (hasWon)
            {
                // Console.WriteLine($"Player {player} has achieved a winning condition.");
            }

            return hasWon;
        }

        public bool IsForbiddenMove(int[,] board)
        {
            return false;
        }

        private bool CheckRows(int[,] board, int player)
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
        private bool CheckColumns(int[,] board, int player)
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

        private bool CheckDiagonals(int[,] board, int player)
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
        private bool CheckAntiDiagonals(int[,] board, int player)
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

        public IRule Clone()
        {
            // Since Renju is stateless, return a new instance
            return new FreeStyle();
        }
    }
}
