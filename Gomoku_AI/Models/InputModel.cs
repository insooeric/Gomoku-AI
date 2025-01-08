namespace Gomoku_AI.Models
{
    public class InputModel
    {
        public List<List<int>> Board { get; set; }
        public int Depth { get; set; }
        public string RuleType { get; set; }
    }
}
