namespace Gomoku_AI.Models
{
    public class MinimaxRequest
    {
        public List<List<int>> Board { get; set; }
        public int Depth { get; set; }
    }
}
