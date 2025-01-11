namespace Gomoku_AI.AIModels.MCTS
{
    public class Move
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

}
