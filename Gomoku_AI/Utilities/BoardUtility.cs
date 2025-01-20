namespace Gomoku_AI.Utilities
{
    public static class BoardUtility
    {
        public static int[,] CloneBoard(int[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            int[,] newBoard = new int[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    newBoard[row, col] = board[row, col];
                }
            }
            return newBoard;
        }
    }
}
