namespace Gomoku_AI.Utilities
{
    public class CurrentPlayer
    {
        public static int Get(int[,] board)
        {
            int blackCount = board.Cast<int>().Count(v => v == 1);
            int whiteCount = board.Cast<int>().Count(v => v == -1);

            return blackCount == whiteCount ? 1 : -1;
        }
    }
}
