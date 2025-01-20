namespace Gomoku_AI.RuleModels
{
    public interface IRule
    {
        bool IsWinning(int[,] board, int player);
        bool IsForbiddenMove(int[,] board);
        IRule Clone();
    }
}
