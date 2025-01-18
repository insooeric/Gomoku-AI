
namespace Gomoku_AI.AIModels.MCTS
{
    public class Move : IEquatable<Move>
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Move(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool Equals(Move other)
        {
            if (other == null)
                return false;
            return this.Row == other.Row && this.Col == other.Col;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Move);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
    }
}
