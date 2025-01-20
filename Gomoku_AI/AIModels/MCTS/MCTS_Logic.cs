using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.WebSockets;
using Gomoku_AI.RuleModels;
using Gomoku_AI.Utilities;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Logic
    {
        private readonly int Iterations;
        private bool Debug = false;
        private MCTS_Node RootNode = null;

        private static readonly double ExplorationConstant = Math.Sqrt(2);

        public MCTS_Logic(MCTS_Node root, int iterations = 1000)
        {
            RootNode = root;
            Iterations = iterations;
        }

        public Move Search()
        {
            for (int i = 0; i < Iterations; i++)
            {
                if (Debug)
                {
                    Console.WriteLine($"\nIteration #{i + 1}");
                }

                MCTS_Node selectedNode = Selection(RootNode);
                if (Debug)
                {
                    if (selectedNode.Move != null)
                        Console.WriteLine($"    Selection: ({selectedNode.Move.Row}, {selectedNode.Move.Col})");
                    else
                        Console.WriteLine($"    Selection: Root Node");
                }


                MCTS_Node expandedNode = selectedNode.Expand();

                if (expandedNode == null)
                {
                    if (Debug)
                    {
                        Console.WriteLine($"    Expansion: No expansion possible (terminal node)");
                        Console.WriteLine($"    Simulation: Skipped");
                        Console.WriteLine($"    Backpropagation: Done");
                    }
                    continue;
                }

                int simulationResult = expandedNode.Simulate();

                expandedNode.Backpropagate(simulationResult);
            }

            return ChooseBestMove(RootNode);
        }

        private MCTS_Node Selection(MCTS_Node node)
        {
            while(node.IsFullyExpanded() && node.Children.Count > 0)
            {
                node = node.BestChild(ExplorationConstant);
            }
            return node;
        }

        private Move ChooseBestMove(MCTS_Node root)
        {
            Move bestMove = null;
            int maxVisits = -1;

            if (Debug)
            {
                Console.WriteLine("\nResult:");
                Console.WriteLine("------------------------------------");
                foreach(var child in root.Children)
                {
                    Console.WriteLine($"({child.Key.Row},{child.Key.Col}) = Visits: {child.Value.Visits} Score: {child.Value.WinScore}");
                }
            }

            foreach (var child in root.Children)
            {
                if(child.Value.Visits > maxVisits)
                {
                    maxVisits = child.Value.Visits;
                    bestMove = child.Key;
                }
            }

            if (Debug)
            {
                Console.WriteLine("\nBest Child:");
                Console.WriteLine($"Row {bestMove.Row}, Col {bestMove.Col}");
            }

            return bestMove;
        }
    }
}
