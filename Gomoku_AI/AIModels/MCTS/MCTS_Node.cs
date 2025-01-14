namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Node
    {
        public MCTS_Gomoku State { get; private set; }
        public MCTS_Node? Parent { get; private set; }
        public List<MCTS_Node> Children { get; private set; }
        public Move? Move { get; private set; } // Nullable Move
        public int Wins { get; private set; }
        public int Visits { get; private set; }
        public List<Move> UntriedMoves { get; private set; }
        public int WhoMoved { get; private set; }

        private static readonly Random rand = new Random();

        public MCTS_Node(MCTS_Gomoku state, MCTS_Node? parent = null, Move? move = null, int maxChildren = 15)
        {
            State = state;
            Parent = parent;
            Move = move;
            WhoMoved = (parent == null) ? state.CurrentPlayer : -state.CurrentPlayer;
            Children = new List<MCTS_Node>();
            Wins = 0;
            Visits = 0;
            // UntriedMoves = state.GetPossibleMoves();
            var allMoves = state.GetPossibleMoves();
            if (allMoves.Count > maxChildren)
            {
                // random sample
                UntriedMoves = allMoves.OrderBy(x => rand.Next()).Take(maxChildren).ToList();
            }
            else
            {
                UntriedMoves = allMoves;
            }
        }

        public bool IsFullyExpanded()
        {
            return UntriedMoves.Count == 0;
        }

        public MCTS_Node Expand()
        {
            // 1) Get prioritized moves from your new static method
            //    This returns a List<Move>, but you can decide whether 
            //    to pick the first or pick from that list in some way:
            var prioritized = Prioritizer.PickPrioritizedMove(State, UntriedMoves);

            Move move;
            if (prioritized.Count > 0)
            {
                // e.g. pick the first from the prioritized list
                move = prioritized[0];
            }
            else
            {
                // fallback if no priority found
                move = UntriedMoves[rand.Next(UntriedMoves.Count)];
            }

            MCTS_Gomoku nextState = State.Clone();
            nextState.ApplyMove(move);

            // create child node
            MCTS_Node child = new MCTS_Node(nextState, this, move);
            Children.Add(child);

            // remove from untried
            UntriedMoves.Remove(move);
            return child;
        }



        public MCTS_Node SelectChild(double cParam = 1.414213562373095)
        {
            // Select the child with the highest UCT value
            MCTS_Node selected = null;
            double bestValue = double.MinValue;

            foreach (var child in Children)
            {
                double uctValue = ((double)child.Wins / (child.Visits + 1e-6)) +
                                  cParam * Math.Sqrt(Math.Log(this.Visits + 1) / (child.Visits + 1e-6));
                if (uctValue > bestValue)
                {
                    selected = child;
                    bestValue = uctValue;
                }
            }

            return selected!;
        }

        public void Update(int result)
        {
            if (result == WhoMoved)
            {
                Wins++;
            }
            // If the result is a win for the player who just moved, count it as a win
            if (result == -State.CurrentPlayer) // Because CurrentPlayer was switched after the move
                Wins++;
        }

        public bool IsTerminal()
        {
            return State.IsGameOver();
        }
    }
}
