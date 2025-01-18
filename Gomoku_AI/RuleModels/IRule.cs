namespace Gomoku_AI.RuleModels
{
    public interface IRule
    {
        bool IsWinning(int[,] board, int player);
        IRule Clone();
    }
}
