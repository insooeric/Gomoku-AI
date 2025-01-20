using Gomoku_AI.AIModels.MCTS;
using Gomoku_AI.RuleModels;
using Gomoku_AI.Utilities;
using System.Diagnostics;
using System.Numerics;

public class MCTS_Node
{
    public int[,] Board { get; private set; }
    public int CurrentPlayer { get; private set; }
    public MCTS_Node Parent { get; private set; }
    public Move Move { get; private set; }
    public Dictionary<Move, MCTS_Node> Children { get; private set; }
    public int Visits { get; private set; }
    public double WinScore { get; private set; }
    public IRule Rule { get; private set; }

    private const bool Debug = false;

    private static Random rand = new Random();


    public MCTS_Node(int[,] board, int currentPlayer, IRule rule, MCTS_Node parent = null, Move move = null)
    {
        Board = board;
        CurrentPlayer = currentPlayer;
        Parent = parent;
        Move = move;
        Children = new Dictionary<Move, MCTS_Node>();
        Visits = 0;
        WinScore = 0;
        Rule = rule;
    }

    public bool IsFullyExpanded()
    {
        return Children.Count == Prioritizer.PickPrioritizedMove(Board, this.Rule).Count;
    }

    public MCTS_Node BestChild(double explorationConstant)
    {
        double bestScore = double.MinValue;
        MCTS_Node bestNode = null;

        foreach (var child in Children.Values)
        {
            double uctScore;
            if (child.Visits == 0)
            {
                uctScore = double.MaxValue; // Prioritize unvisited nodes
            }
            else
            {
                double exploitation = child.WinScore / child.Visits;
                double exploration = explorationConstant * Math.Sqrt(Math.Log(this.Visits) / child.Visits);
                uctScore = exploitation + exploration;
            }

            if (uctScore > bestScore)
            {
                bestScore = uctScore;
                bestNode = child;
            }
        }

        return bestNode;
    }

    public MCTS_Node BestChild()
    {
        return BestChild(Math.Sqrt(2));
    }

    public MCTS_Node Expand()
    {
        List<Move> legalMoves = Prioritizer.PickPrioritizedMove(Board, this.Rule);
        List<Move> triedMoves = new List<Move>(Children.Keys);

        List<Move> untriedMoves = new List<Move>();
        foreach (var move in legalMoves)
        {
            if (!Children.ContainsKey(move))
            {
                untriedMoves.Add(move);
            }
        }

        if (untriedMoves.Count == 0)
        {
            return null; // No moves to expand
        }

        // Randomly select an untried move
        Move moveToTry = untriedMoves[rand.Next(untriedMoves.Count)];
        int[,] newBoard = BoardUtility.CloneBoard(Board);
        newBoard[moveToTry.Row, moveToTry.Col] = CurrentPlayer;
        int nextPlayer = -CurrentPlayer;

        MCTS_Node childNode = new MCTS_Node(newBoard, nextPlayer, this.Rule, this, moveToTry);
        Children.Add(moveToTry, childNode);

        if (Debug)
        {
            Console.WriteLine($"    Expansion: ({moveToTry.Row}, {moveToTry.Col})");
        }

        return childNode;
    }

    public int Simulate()
    {
        int[,] simulationBoard = BoardUtility.CloneBoard(Board);
        int simulationPlayer = CurrentPlayer;

        while (true)
        {
            List<Move> legalMoves = Prioritizer.PickPrioritizedMove(simulationBoard, this.Rule);
            if (legalMoves.Count == 0)
            {
                if (Debug)
                {
                    Console.WriteLine($"    Simulation: Draw");
                }
                return 0; // Draw
            }

            Move move = legalMoves[rand.Next(legalMoves.Count)];

            simulationBoard[move.Row, move.Col] = simulationPlayer;

            if (Rule.IsWinning(simulationBoard, simulationPlayer))
            {
                if (Debug)
                {
                    string winner = (simulationPlayer == 1) ? "Black Wins" : "White Wins";
                    Console.WriteLine($"    Simulation: {winner}");
                }
                return simulationPlayer;
            }

            simulationPlayer = -simulationPlayer;
        }
    }

    public void Backpropagate(int result)
    {
        Visits++;
        WinScore += result;

        if (Parent != null)
        {
            Parent.Backpropagate(result);
        }

        if (Parent == null && Debug)
        {
            Console.WriteLine($"    Backpropagation: Done");
        }
    }
}
