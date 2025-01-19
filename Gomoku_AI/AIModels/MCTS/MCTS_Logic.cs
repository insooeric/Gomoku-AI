using System;
using System.Collections.Generic;
using System.Linq;
using Gomoku_AI.RuleModels;
using Gomoku_AI.Utilities;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Logic
    {
        private readonly int Iterations;
        private readonly IRule Rule;
        private static readonly double ExplorationConstant = Math.Sqrt(2);
        private bool Debug = true;
        private MCTS_Node RootNode = null;
        private Random rand;

        public MCTS_Logic(int iterations, IRule rule)
        {
            this.Iterations = iterations;
            this.Rule = rule;
            this.rand = new Random();
        }

        public Move Search(int[,] board)
        {
            try
            {
                if (Debug)
                {
                    Console.WriteLine("\nSTARTING MCTS SEARCH");
                    Console.WriteLine("---------------------------------------");
                }

                if (IsBoardEmpty(board))
                {
                    // Define the center move
                    int centerRow = board.GetLength(0) / 2;
                    int centerCol = board.GetLength(1) / 2;

                    Move centerMove = new Move(centerRow, centerCol);

                    if (Debug)
                    {
                        Console.WriteLine($"DEBUG: Board is empty. Returning center move ({centerMove.Row}, {centerMove.Col}).");
                    }

                    return centerMove;
                }

                if (RootNode == null)
                {
                    RootNode = new MCTS_Node(new Move(-1, -1), board, null);
                }

                // MCTS iteration
                for (int i = 0; i < Iterations; i++)
                {
                    if (Debug)
                    {
                        Console.WriteLine($"Iteration #{i + 1}");
                    }

                    // 1. Selection
                    MCTS_Node selectedNode = Selection(RootNode);

                    if (selectedNode == null)
                    {
                        if (Debug)
                        {
                            Console.WriteLine("Selected node is null. Skipping iteration.");
                        }
                        continue;
                    }

                    if (Debug)
                    {
                        Console.WriteLine($"Selected Node: Move=({selectedNode.Move.Row}, {selectedNode.Move.Col})");
                    }

                    // 2. Expansion
                    MCTS_Node expandedNode = Expansion(selectedNode);

                    // 3. Simulation
                    int currentPlayer = CurrentPlayer.Get(expandedNode.BoardState);
                    int simulationResult = Simulation(expandedNode.BoardState, currentPlayer);

                    if (Debug)
                    {
                        Console.WriteLine($"Simulation Result: {(simulationResult == 1 ? "AI Win" : simulationResult == -1 ? "Opponent Win" : "Draw")}");
                    }

                    // 3. Backpropagation
                    Backpropagation(expandedNode, simulationResult);
                }

                return ChooseBestMove();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in MCTS Search: {e.Message}");
                return new Move(-1, -1); // Invalid move
            }
        }

        private MCTS_Node Selection(MCTS_Node node)
        {
            MCTS_Node currentNode = node;

            while (true)
            {
                if (!currentNode.GetChildren().Any())
                {
                    // Node has no children; return it for expansion
                    return currentNode;
                }

                List<Move> possibleMoves = Prioritizer.PickPrioritizedMove(currentNode.BoardState, Rule);
                List<Move> untriedMoves = possibleMoves.Where(move =>
                    !currentNode.GetChildren().Any(child => child.Move.Equals(move))
                ).ToList();

                if (untriedMoves.Any())
                {
                    // Node has untried moves; return it for expansion
                    return currentNode;
                }

                // Node is fully expanded; select the best child based on UCT
                MCTS_Node selectedChild = currentNode.SelectChildWithHighestUCT(ExplorationConstant);

                if (selectedChild == null)
                {
                    // No children to select; return current node
                    return currentNode;
                }

                currentNode = selectedChild;
            }
        }

        private MCTS_Node Expansion(MCTS_Node node)
        {
            if (node.IsTerminal)
                return node; // No expansion needed for terminal nodes

            List<Move> possibleMoves = Prioritizer.PickPrioritizedMove(node.BoardState, Rule);

            // Filter out moves that have already been expanded
            List<Move> untriedMoves = possibleMoves.Where(move =>
                !node.GetChildren().Any(child => child.Move.Equals(move))
            ).ToList();

            if (!untriedMoves.Any())
            {
                node.SetExpanded(); // All moves have been tried
                return node;
            }

            // Select one untried move (randomly or based on priority)
            Move moveToTry = untriedMoves[rand.Next(untriedMoves.Count)];

            // Apply the move to the board to get the new state
            int[,] newBoard = CloneBoard(node.BoardState);
            int currentPlayer = CurrentPlayer.Get(newBoard);
            newBoard[moveToTry.Row, moveToTry.Col] = currentPlayer;

            // Check if the move results in a win
            bool isWin = Rule.IsWinning(newBoard, currentPlayer);

            // Create a new child node for this move
            MCTS_Node childNode = new MCTS_Node(moveToTry, newBoard, node)
            {
                IsTerminal = isWin
            };

            // Add the child node to the current node
            node.AddChild(childNode);

/*            if (Debug)
            {
                Console.WriteLine($"DEBUG: Added child node ({moveToTry.Row}, {moveToTry.Col}), IsTerminal={childNode.IsTerminal}");
            }*/

            return childNode;
        }

        private int Simulation(int[,] board, int currentPlayer)
        {
            int[,] simulationBoard = CloneBoard(board);
            int player = currentPlayer;
            int moveCount = 0;
            int maxMoves = 60; // Prevent infinite simulations

            while (moveCount < maxMoves)
            {
                List<Move> availableMoves = Prioritizer.PickPrioritizedMove(simulationBoard, Rule);
                if (availableMoves.Count == 0)
                {
                    if (Debug)
                    {
                        Console.WriteLine("DEBUG: Simulation ended in a Draw due to no available moves.");
                    }
                    return 0; // Draw
                }

                // Select a random move
                Move randomMove = availableMoves[rand.Next(availableMoves.Count)];
                simulationBoard[randomMove.Row, randomMove.Col] = player;
                moveCount++;

/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Player {(player == 1 ? "Black" : "White")} placed at ({randomMove.Row}, {randomMove.Col}). Move Count: {moveCount}");
                }*/
                    

                // Check for a win
                if (Rule.IsWinning(simulationBoard, player))
                {
                    string winningPlayer = player == 1 ? "AI (Black)" : "Opponent (White)";
/*                    if (Debug)
                    {
                        Console.WriteLine($"DEBUG: {winningPlayer} wins the simulation.");
                    }*/

                    return player == 1 ? 1 : -1; // 1 for AI win, -1 for Opponent win
                }

                // Correctly switch player
                player = -player;
            }

            // If maxMoves reached without a win, declare a draw
/*            if (Debug)
            {
                Console.WriteLine("DEBUG: Simulation ended in a Draw due to reaching maximum move limit.");
            }*/

            return 0; // Draw
        }


        private void Backpropagation(MCTS_Node node, int result)
        {
            MCTS_Node currentNode = node;
            while (currentNode != null)
            {
                currentNode.IncrementVisits();

                // We're only considering wins here.
                if (result == 1)
                {
                    currentNode.AddWins(1);
                    if (Debug)
                    {
                        // Console.WriteLine($"DEBUG: Node ({currentNode.Move.Row}, {currentNode.Move.Col}) wins incremented by 1.");
                    }
                }
                else if (result == -1)
                {
                    currentNode.AddWins(0);
                    if (Debug)
                    {
                        // Console.WriteLine($"DEBUG: Node ({currentNode.Move.Row}, {currentNode.Move.Col}) wins remain unchanged (Opponent Win).");
                    }
                }
                else
                {
                    currentNode.AddWins(0);
                    if (Debug)
                    {
                        // Console.WriteLine($"DEBUG: Node ({currentNode.Move.Row}, {currentNode.Move.Col}) wins remain unchanged (Draw).");
                    }
                }

                currentNode = currentNode.Parent;
            }
        }

        private Move ChooseBestMove()
        {
            if (RootNode == null || !RootNode.GetChildren().Any())
            {
                return new Move(-1, -1); // No valid moves
            }

            // Select the child with the highest visit count
            var bestChild = RootNode.GetChildren()
                                     .OrderByDescending(child => child.Visits)
                                     .FirstOrDefault();

            if (bestChild != null)
            {
                if (Debug)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Root's Immediate Children:");
                    foreach (var child in RootNode.GetChildren())
                    {
                        Console.WriteLine($"Move: ({child.Move.Row}, {child.Move.Col}) - Visits: {child.Visits}, Wins: {child.Wins}");
                    }
                    PrintTreeStatistics();

                    Console.WriteLine($"\nBest Move Selected: ({bestChild.Move.Row}, {bestChild.Move.Col}) with {bestChild.Visits} visits and {bestChild.Wins} wins.");
                }
                return bestChild.Move;
            }

            return new Move(-1, -1); // Default invalid move
        }

        private bool IsBoardEmpty(int[,] _board)
        {
            for (int row = 0; row < _board.GetLength(0); row++)
            {
                for (int col = 0; col < _board.GetLength(1); col++)
                {
                    if (_board[row, col] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int[,] CloneBoard(int[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            int[,] newBoard = new int[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    newBoard[row, col] = board[row, col];
                }
            }
            return newBoard;
        }

        /*----------------------------------------------------*
         *              ⭐⭐⭐ FOR DEBUGGING ⭐⭐⭐              *
         *----------------------------------------------------*/
        private void PrintTreeStatistics()
        {
            if (RootNode == null)
            {
                Console.WriteLine("The MCTS tree is empty.");
                return;
            }

            Console.WriteLine("MCTS Tree Statistics:");
            Console.WriteLine("---------------------------------------");
            TraverseAndPrint(RootNode, 0);
        }

        private void TraverseAndPrint(MCTS_Node node, int depth)
        {
            string indent = new string(' ', depth * 2);
            string moveInfo = node.Move.Row == -1 && node.Move.Col == -1
                ? "Root Node"
                : $"Move: ({node.Move.Row}, {node.Move.Col})";

            Console.WriteLine($"{indent}{moveInfo} - Visits: {node.Visits}, Wins: {node.Wins}");

            foreach (var child in node.GetChildren())
            {
                TraverseAndPrint(child, depth + 1);
            }
        }
        /*----------------------------------------------------*
         *            ⭐⭐⭐ FOR DEBUGGING END ⭐⭐⭐            *
         *----------------------------------------------------*/
    }
}
