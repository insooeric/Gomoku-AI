using System.Xml.Linq;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Logic
    {
        private readonly int Iterations;
        private readonly double ExplorationConstant;
        private static readonly Random rand = new Random();

        public MCTS_Logic(int iterations = 1000, double explorationConstant = 1.414213562373095)
        {
            Iterations = iterations;
            ExplorationConstant = explorationConstant;
        }

        public Move? Search(MCTS_Gomoku rootState)
        {
            try
            {
                MCTS_Node rootNode = new MCTS_Node(rootState);
                // Console.WriteLine("Performing Search");

                // Safeguard against null Move for root node
/*                if (rootNode.Move != null)
                {
                    Console.WriteLine($"RootNode: Row {rootNode.Move.Row}, Col {rootNode.Move.Col}");
                }
                else
                {
                    Console.WriteLine("RootNode has no associated move (as expected).");
                }*/

                for (int i = 0; i < Iterations; i++)
                {
                    // Console.WriteLine($"Current iteration: {i}");
                    MCTS_Node node = rootNode;
                    MCTS_Gomoku state = rootState.Clone();

                    // **Selection**
                    while (!node.IsTerminal() && node.IsFullyExpanded())
                    {
                        node = node.SelectChild(ExplorationConstant);
                        if (node.Move == null)
                        {
                            // Console.WriteLine("Selected child has no associated move. Skipping.");
                            break;
                        }
                        state.ApplyMove(node.Move);
                    }

                    // **Expansion**
                    if (!node.IsTerminal())
                    {
                        MCTS_Node child = node.Expand();
                        if (child.Move == null)
                        {
                            // Console.WriteLine("Expanded child has no associated move. Skipping.");
                            continue;
                        }
                        state.ApplyMove(child.Move);
                        node = child;
                    }

                    // **Simulation**
                    int result = Simulate(state);
                    // Console.WriteLine($"Simulation Result: {result}");

                    // **Backpropagation**
                    while (node != null)
                    {
                        node.Update(result);
                        node = node.Parent;
                    }
                }

                // **Select the move with the highest visit count**
                MCTS_Node? bestChild = null;
                int maxVisits = -1;
                foreach (var child in rootNode.Children)
                {
                    if (child.Visits > maxVisits)
                    {
                        bestChild = child;
                        maxVisits = child.Visits;
                    }
                }

                if (bestChild == null)
                {
                    // Console.WriteLine("No valid moves found.");
                    return null;
                }

                Console.WriteLine($"Best Move: Row {bestChild.Move.Row}, Col {bestChild.Move.Col} with {bestChild.Visits} visits.");

                return bestChild.Move;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"An error occurred during MCTS Search: {ex.Message}");
                return null;
            }
        }

        private int Simulate(MCTS_Gomoku state)
        {
            while (!state.IsGameOver())
            {
                List<Move> moves = state.GetPossibleMoves();
                if (moves.Count == 0)
                    break;

                Move move = moves[rand.Next(moves.Count)];
                state.ApplyMove(move);
            }

            return state.CheckWinner();
        }
    }
}
