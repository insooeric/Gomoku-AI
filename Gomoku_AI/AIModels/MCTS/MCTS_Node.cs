using Gomoku_AI.AIModels.MCTS;

public class MCTS_Node
{
    public Move Move { get; private set; }
    public int[,] BoardState { get; private set; }
    public MCTS_Node Parent { get; private set; }
    private List<MCTS_Node> children;
    public bool IsTerminal { get; set; }
    public int Visits { get; private set; }
    public int Wins { get; private set; }
    public bool IsExpanded { get; private set; }

    public MCTS_Node(Move move, int[,] boardState, MCTS_Node parent)
    {
        Move = move;
        BoardState = boardState;
        Parent = parent;
        children = new List<MCTS_Node>();
        Visits = 0;
        Wins = 0;
        IsExpanded = false;
    }

    public void AddChild(MCTS_Node child)
    {
        children.Add(child);
    }

    public List<MCTS_Node> GetChildren()
    {
        return children;
    }

    public void IncrementVisits()
    {
        Visits++;
    }

    public void AddWins(int result)
    {
        Wins += result;
    }

    public void SetExpanded()
    {
        IsExpanded = true;
    }

    public MCTS_Node SelectChildWithHighestUCT(double explorationConstant)
    {
        double bestUCT = double.MinValue;
        MCTS_Node selectedChild = null;

        foreach (var child in children)
        {
            if (child.Visits == 0)
            {
                return child;
            }

            double exploitation = (double)child.Wins / child.Visits;
            double exploration = explorationConstant * Math.Sqrt(Math.Log(this.Visits) / child.Visits);
            double uct = exploitation + exploration;

            if (uct > bestUCT)
            {
                bestUCT = uct;
                selectedChild = child;
            }
        }

        return selectedChild;
    }
}
