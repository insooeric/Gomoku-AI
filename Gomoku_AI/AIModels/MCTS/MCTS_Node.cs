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
        private bool Debug = true;

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
            UntriedMoves = state.GetPossibleMoves();
            //var allMoves = state.GetPossibleMoves();
/*            if (allMoves.Count > maxChildren)
            {
                // random sample
                UntriedMoves = allMoves.OrderBy(x => rand.Next()).Take(maxChildren).ToList();
            }
            else
            {
                UntriedMoves = allMoves;
            }*/
        }

        public bool IsFullyExpanded()
        {
            return UntriedMoves.Count == 0;
        }

        // testing with random move
        public MCTS_Node Expand()
        {
            // 1) Pick randomly from the untried moves
            Move move = UntriedMoves[rand.Next(UntriedMoves.Count)];

            // 2) Clone the current state and apply that move
            MCTS_Gomoku nextState = State.Clone();
            nextState.ApplyMove(move);

            // 3) Create a new child node for that move
            MCTS_Node child = new MCTS_Node(nextState, this, move);
            Children.Add(child);

            // 4) Remove the chosen move from untried
            UntriedMoves.Remove(move);

            // 5) Return the newly created child node
            return child;
        }


        /*        public MCTS_Node Expand()
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
                }*/



        public MCTS_Node SelectChild(double cParam = 1.414213562373095)
        {
            // 1) If no children, return 'this' to indicate no further selection
            if (Children.Count == 0)
            {
                // Another option could be: throw new InvalidOperationException("No children to select from.");
                return this;
            }

            // 2) If *all* children have zero visits, pick randomly
            bool allZeroVisits = Children.All(ch => ch.Visits == 0);
            if (allZeroVisits)
            {
                // random tie-breaking among all children
                int randomIndex = rand.Next(Children.Count);
                return Children[randomIndex];
            }

            // 3) Standard UCT selection among children
            MCTS_Node selected = null;
            double bestValue = double.MinValue;

            foreach (var child in Children)
            {
                // child.Wins / (child.Visits + 1e-6) => average win rate
                // + cParam * sqrt( ln(parent.Visits+1) / (child.Visits+1e-6) ) => exploration
                double uctValue = ((double)child.Wins / (child.Visits + 1e-6))
                                  + cParam * Math.Sqrt(Math.Log(this.Visits + 1)
                                                       / (child.Visits + 1e-6));

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
            Visits++;

            // If the simulation result is a win for the player who made the move to this node
            if (result == WhoMoved)
            {
                Wins++;
            }
        }


        public bool IsTerminal()
        {
            return State.IsGameOver();
        }
    }
}
