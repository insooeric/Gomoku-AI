namespace Gomoku_AI.Utilities
{
    public static class CurrentPlayer
    {
        public static int Get(int[,] board)
        {
            int blackCount = 0;
            int whiteCount = 0;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    if (board[x, y] == 1) blackCount++;
                    else if (board[x, y] == -1) whiteCount++;
                }
            }

            // Black starts first; if counts are equal, it's Black's turn; else, it's White's turn
            int currentPlayer = blackCount > whiteCount ? -1 : 1;
            // Console.WriteLine($"Current Player: {(currentPlayer == 1 ? "Black" : "White")}");
            return currentPlayer;
        }

    }
}
