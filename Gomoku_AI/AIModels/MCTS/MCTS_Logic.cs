using Microsoft.OpenApi.Services;
using System.Diagnostics;
using System.Xml.Linq;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Logic
    {
        private readonly int Iterations;
        private readonly double ExplorationConstant;
        private static readonly Random rand = new Random();
        private bool DebugMode = true;
        private const int PriorityDepthDefault = 10;

        public MCTS_Logic(int iterations = 3000, double explorationConstant = 1.414213562373095)
        {
            Iterations = iterations;
            ExplorationConstant = explorationConstant;
        }

        public Move Search(MCTS_Gomoku rootState)
        {
            // Create the root node
            MCTS_Node rootNode = new MCTS_Node(rootState);

            // Main MCTS loop
            for (int i = 0; i < Iterations; i++)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Iteration: {i}");
                }
                MCTS_Node node = rootNode;
                MCTS_Gomoku state = rootState.Clone();

                // 1) Selection
                while (!node.IsTerminal() && node.IsFullyExpanded())
                {
                    node = node.SelectChild(ExplorationConstant);
                    if (DebugMode)
                    {
                        Console.WriteLine($"Selected Move: Row={node.Move?.Row}, Col={node.Move?.Col}");
                    }
                    /*                    if (node.Move == null)
                                            break; // Should rarely happen if your tree logic is correct*/
                    // Console.WriteLine($"Applying Move Row={node.Move?.Row}, Col={node.Move?.Col}:");
                    if (node.Move != null)
                    {
                        state.ApplyMove(node.Move);
                    }
                }

                // 2) Expansion
                if (!node.IsTerminal())
                {
                    MCTS_Node child = node.Expand();
                    if (child.Move == null)
                        continue;
                    state.ApplyMove(child.Move);
                    node = child;
                }

                // 3) Simulation (heuristic or random)
                int result = Simulate(state);//, maxDepth: 60);//, priorityDepth: PriorityDepthDefault);

                /*                if (DebugMode)
                                {
                                    Console.WriteLine($"Simulated Move: Row={node.Move?.Row} Col={node.Move?.Col}");
                                    Console.WriteLine($"Result after Simulation: {result}");
                                    Console.WriteLine("--------------------------------------------");
                                }*/


                // 4) Backpropagation
                while (node != null)
                {
                    node.Update(result);
                    if (DebugMode)
                    {
                        Console.WriteLine($"[Backpropagation] Updated Node (Move: R={node.Move?.Row}, C={node.Move?.Col}) with result {result}");
                    }
                    node = node.Parent;
                }
                /*                while (node != null && node.Parent != null)
                                {
                                    node.Update(result);
                                    node = node.Parent;
                                }*/
            }


            // ------------------------------------------------
            // Final Move Selection: never return null
            // ------------------------------------------------

            // 1) Prefer the child with the highest visits
            MCTS_Node? bestChild = null;
            int maxVisits = -1;
            foreach (var c in rootNode.Children)
            {
                if (c.Visits > maxVisits)
                {
                    bestChild = c;
                    maxVisits = c.Visits;
                }
            }

            if (bestChild != null && bestChild.Move != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Best Move: Row={bestChild.Move.Row}, Col={bestChild.Move.Col}");
                }
                return bestChild.Move;
            }
            else if (rootNode.UntriedMoves.Count > 0)
            {
                Move fallback = rootNode.UntriedMoves[0];
                if (DebugMode)
                {
                    Console.WriteLine($"Best Move (fallback untried): Row={fallback.Row}, Col={fallback.Col}");
                }
                return fallback;
            }

            throw new InvalidOperationException("No valid moves remain. The board is likely terminal.");

            // ------------------------------------------------
            // Final Move Selection: never return null
            // ------------------------------------------------

            // 1) Prefer the child with the highest visits
            /*            MCTS_Node? bestChild = null;
                        int maxVisits = -1;
                        foreach (var c in rootNode.Children)
                        {
                            if (c.Visits > maxVisits)
                            {
                                bestChild = c;
                                maxVisits = c.Visits;
                            }
                        }

                        if (bestChild != null && bestChild.Move != null)
                        {
                            if (DebugMode)
                            {
                                Console.WriteLine($"Best Move: Row={bestChild.Move.Row}, Col={bestChild.Move.Col}");
                            }
                            return bestChild.Move;
                        }
                        else if (rootNode.UntriedMoves.Count > 0)
                        {
                            Move fallback = rootNode.UntriedMoves[0];
                            if (DebugMode)
                            {
                                Console.WriteLine($"Best Move (fallback untried): Row={fallback.Row}, Col={fallback.Col}");
                            }
                            return fallback;
                        }

                        throw new InvalidOperationException("No valid moves remain. The board is likely terminal.");*/
        }

        /*        private int SimulateHeuristic(MCTS_Gomoku state)
                {
                    int moveLimit = 60;  // limit the playout

                    while (!state.IsGameOver() && moveLimit-- > 0)
                    {
                        var moves = state.GetPossibleMoves();
                        if (moves.Count == 0) break;

                        Move chosen = FindImmediateWinOrBlock(state, moves);
                        if (chosen == null)
                        {
                            chosen = moves[rand.Next(moves.Count)];
                        }

                        state.ApplyMove(chosen);
                    }

                    return state.CheckWinner();
                }*/


        /// <summary>
        /// Simulates a random playout from the current board state until
        /// the game is over or a maximum depth is reached.
        /// Returns:
        ///   1 if Black (player=1) eventually wins,
        ///  -1 if White (player=-1) eventually wins,
        ///   0 if neither wins by the end (draw).
        /// </summary>
        private int Simulate(MCTS_Gomoku state, int maxDepth = 60)
        {
            int steps = 0;
            // Continue until the game ends or we reach a move-limit
            while (!state.IsGameOver() && steps < maxDepth)
            {
                var moves = state.GetPossibleMoves();
                if (moves.Count == 0)
                    break; // no moves => terminal

                // Pick a random move from the available moves
                //Move chosen = moves[rand.Next(moves.Count)];
                Move chosen = ChooseStrategicMove(state, moves);
                state.ApplyMove(chosen);

                steps++;
            }

            // After we exit, check the winner:
            //   1 => Black wins, -1 => White wins, 0 => no winner => "draw"
            return state.CheckWinner();
        }

        private Move ChooseStrategicMove(MCTS_Gomoku state, List<Move> moves)
        {
            // 1. Check for immediate win
            Move? winningMove = Prioritizer.FindImmediateWin(state, moves, state.CurrentPlayer);
            if (winningMove != null)
                return winningMove;

            // 2. Check to block opponent's immediate win
            Move? blockMove = Prioritizer.FindImmediateWin(state, moves, -state.CurrentPlayer);
            if (blockMove != null)
                return blockMove;

            // 3. Otherwise, pick a random move
            return moves[rand.Next(moves.Count)];
        }


        /*        private int Simulate(MCTS_Gomoku state)//, int maxDepth = 60, int priorityDepth = 6)
                {
                    int steps = 0;
                    while (!state.IsGameOver())// && maxDepth-- > 0)
                    {
                        var moves = state.GetPossibleMoves();
                        if (moves.Count == 0)
                            break;

                        // If we always have at least one move from PickPrioritizedMove, just pick it
                        var prioritized = Prioritizer.PickPrioritizedMove(state, moves);
                        Move chosen;  // no fallback needed if we're guaranteed at least one
                        if (prioritized.Count > 0)
                        {
                            // Use the first move or pick randomly among them
                            chosen = prioritized[0];
                        }
                        else
                        {
                            // fallback – pick from all moves or random, or just break
                            chosen = moves[rand.Next(moves.Count)];
                        }


                        state.ApplyMove(chosen);
                        if (DebugMode)
                        {
                            Console.WriteLine($"Chosen Move: R={chosen.Row}, C={chosen.Col}");

                        }

                        steps++;
                    }
                    return state.CheckWinner();
                }*/
    }
}
