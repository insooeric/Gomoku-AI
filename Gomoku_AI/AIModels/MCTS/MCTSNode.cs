namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTSNode
    {
        public Board State { get; private set; }
        public Move Move { get; private set; }
        public MCTSNode Parent { get; private set; }
        public List<MCTSNode> Children { get; private set; }
        public int Wins { get; private set; }
        public int Visits { get; private set; }
        public Cell Player { get; private set; }

        private static readonly double C = Math.Sqrt(2);

        public MCTSNode(Board state, Move move, MCTSNode parent, Cell player)
        {
            State = state;
            Move = move;
            Parent = parent;
            Player = player;
            Children = new List<MCTSNode>();
            Wins = 0;
            Visits = 0;
        }

        public bool IsFullyExpanded()
        {
            return GetUntriedMoves().Count == 0;
        }

        public List<Move> GetUntriedMoves()
        {
            var allMoves = GetPossibleMoves(State);
            var triedMoves = Children.Select(c => c.Move).ToHashSet(new MoveComparer());
            return allMoves.Where(m => !triedMoves.Contains(m, new MoveComparer())).ToList();
        }

        public MCTSNode SelectChild()
        {
            return Children.OrderByDescending(c => UCB1(c)).First();
        }

        private double UCB1(MCTSNode child)
        {
            if (child.Visits == 0)
                return double.MaxValue;

            double exploitation = (double)child.Wins / child.Visits;
            double exploration = C * Math.Sqrt(Math.Log(this.Visits) / child.Visits);
            return exploitation + exploration;
        }

        public MCTSNode AddChild(Move move, Board state, Cell player)
        {
            var childNode = new MCTSNode(state, move, this, player);
            Children.Add(childNode);
            return childNode;
        }

        public void Update(int result)
        {
            Visits++;
            Wins += result;
        }

        private List<Move> GetPossibleMoves(Board board)
        {
            // To reduce the branching factor, consider moves adjacent to existing stones
            var moves = new List<Move>();
            int[] directions = { -1, 0, 1 };
            var hasNeighbor = new bool[board.Size, board.Size];

            for (int x = 0; x < board.Size; x++)
            {
                for (int y = 0; y < board.Size; y++)
                {
                    if (board.GetCell(x, y) != Cell.Empty)
                    {
                        foreach (var dx in directions)
                        {
                            foreach (var dy in directions)
                            {
                                if (dx == 0 && dy == 0)
                                    continue;

                                int nx = x + dx;
                                int ny = y + dy;

                                if (board.IsValid(nx, ny) && board.IsEmpty(nx, ny))
                                {
                                    hasNeighbor[nx, ny] = true;
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < board.Size; x++)
            {
                for (int y = 0; y < board.Size; y++)
                {
                    if (hasNeighbor[x, y])
                        moves.Add(new Move(x, y));
                }
            }

            // If the board is empty, choose the center
            if (!moves.Any())
                moves.Add(new Move(board.Size / 2, board.Size / 2));

            return moves;
        }
    }

    public class MoveComparer : IEqualityComparer<Move>
    {
        public bool Equals(Move a, Move b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public int GetHashCode(Move move)
        {
            return HashCode.Combine(move.X, move.Y);
        }
    }

}
