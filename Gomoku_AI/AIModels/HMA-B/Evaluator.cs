using System;
using Gomoku_AI.RuleModels;

namespace Gomoku_AI.AIModels.HMA_B
{
    public class Evaluator
    {
        private readonly IRule rule;

        private const int WIN_SCORE = 1000000;
        private const int LOSE_SCORE = -1000000;

        public Evaluator(IRule rule)
        {
            this.rule = rule;
        }

        public int EvaluateBoard(int[,] board)
        {
            if (rule.IsWinning(board, 1))
            {
                // Console.WriteLine("Black is winning");
                return WIN_SCORE;
            }

            if (rule.IsWinning(board, -1))
            {
                // Console.WriteLine("White is winning");
                return LOSE_SCORE;
            }

            int blackScore = EvaluatePartialLines(board, 1);
            int whiteScore = EvaluatePartialLines(board, -1);

            return blackScore - whiteScore;
        }

        private int EvaluatePartialLines(int[,] board, int player)
        {
            int totalScore = 0;

            int maxX = board.GetLength(0);
            int maxY = board.GetLength(1);

            for (int x = 0; x < maxX; x++)
            {
                int consecutive = 0;
                for (int y = 0; y < maxY; y++)
                {
                    if (board[x, y] == player)
                        consecutive++;
                    else
                    {
                        if (consecutive > 0)
                        {
                            totalScore += ScoreForConsecutive(consecutive);
                            consecutive = 0;
                        }
                    }
                }
                if (consecutive > 0)
                    totalScore += ScoreForConsecutive(consecutive);
            }

            for (int y = 0; y < maxY; y++)
            {
                int consecutive = 0;
                for (int x = 0; x < maxX; x++)
                {
                    if (board[x, y] == player)
                        consecutive++;
                    else
                    {
                        if (consecutive > 0)
                        {
                            totalScore += ScoreForConsecutive(consecutive);
                            consecutive = 0;
                        }
                    }
                }
                if (consecutive > 0)
                    totalScore += ScoreForConsecutive(consecutive);
            }

            for (int startX = 0; startX < maxX; startX++)
            {
                totalScore += EvaluateConsecutiveOnDiagonal(board, player, startX, 0, 1, 1);
            }
            for (int startY = 1; startY < maxY; startY++)
            {
                totalScore += EvaluateConsecutiveOnDiagonal(board, player, 0, startY, 1, 1);
            }

            for (int startX = 0; startX < maxX; startX++)
            {
                totalScore += EvaluateConsecutiveOnDiagonal(board, player, startX, maxY - 1, 1, -1);
            }
            for (int startY = maxY - 2; startY >= 0; startY--)
            {
                totalScore += EvaluateConsecutiveOnDiagonal(board, player, 0, startY, 1, -1);
            }

            return totalScore;
        }

        private int EvaluateConsecutiveOnDiagonal(
            int[,] board, int player,
            int startX, int startY,
            int deltaX, int deltaY)
        {
            int maxX = board.GetLength(0);
            int maxY = board.GetLength(1);

            int x = startX;
            int y = startY;
            int totalScore = 0;
            int consecutive = 0;

            while (x >= 0 && x < maxX && y >= 0 && y < maxY)
            {
                if (board[x, y] == player)
                    consecutive++;
                else
                {
                    if (consecutive > 0)
                    {
                        totalScore += ScoreForConsecutive(consecutive);
                        consecutive = 0;
                    }
                }
                x += deltaX;
                y += deltaY;
            }

            if (consecutive > 0)
            {
                totalScore += ScoreForConsecutive(consecutive);
            }

            return totalScore;
        }

        private int ScoreForConsecutive(int length)
        {
            switch (length)
            {
                case 1: return 2;
                case 2: return 10;
                case 3: return 50;
                case 4: return 500;
                case 5: return 10000;
                default:
                    return 10000 * length;
            }
        }
    }
}
