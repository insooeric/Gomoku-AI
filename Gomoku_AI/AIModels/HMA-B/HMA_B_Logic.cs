using Gomoku_AI.RuleModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku_AI.AIModels.HMA_B
{
    public class HMA_B_Logic
    {
        private readonly int depth;
        private readonly IRule rule;

        private const int WIN_SCORE = 1000000;
        private const int LOSE_SCORE = -1000000;

        private int realPlayer;

        private readonly HMA_B_Evaluator evaluator;

        public HMA_B_Logic(int depth, IRule rule)
        {
            this.depth = depth;
            this.rule = rule;
            this.evaluator = new HMA_B_Evaluator(rule);
        }

        public (double, int, int) GetBestMove(int[,] board, int currentPlayer)
        {
            int[,] tmpBoard = (int[,])board.Clone();
            bool isWhite = (currentPlayer == -1);

            if (isWhite)
            {
                realPlayer = -1;
            }
            else
            {
                realPlayer = 1;
            }

            // Console.WriteLine("Current player: " + (realPlayer == 1 ? "Black" : "White"));

            int score = 0;
            int bestX = -1;
            int bestY = -1;

            (score, bestX, bestY) = Minimax(
                tmpBoard,
                currentDepth: depth,
                alpha: Int32.MinValue,
                beta: Int32.MaxValue,
                currentPlayer: realPlayer
            );

            string color = isWhite ? "White" : "Black";

            if (bestX == -1 && bestY == -1)
            {
                return (score, bestX, bestY);
            }

            return (score, bestX, bestY);
        }

        private (int score, int x, int y) Minimax(int[,] board, int currentDepth, int alpha, int beta, int currentPlayer)
        {
            int evaluation = evaluator.EvaluateBoard(board);
            // Console.WriteLine($"Depth: {currentDepth}, Player: {(currentPlayer == 1 ? "Black" : "White")}, Evaluation: {evaluation}");
            if (Math.Abs(evaluation) == WIN_SCORE || currentDepth == 0 || IsBoardFull(board))
            {
                return (evaluation, -1, -1);
            }

            int bestX = -1;
            int bestY = -1;

            bool maximizing = (currentPlayer == 1);

            var possibleMoves = GeneratePossibleMoves(board, realPlayer);

            if (maximizing)
            {
                int maxEval = Int32.MinValue;
                foreach ((int moveX, int moveY) in possibleMoves)
                {
                    board[moveX, moveY] = currentPlayer;

                    var (childScore, _, _) = Minimax(board, currentDepth - 1, alpha, beta, -currentPlayer);

                    board[moveX, moveY] = 0;

                    if (childScore > maxEval)
                    {
                        maxEval = childScore;
                        bestX = moveX;
                        bestY = moveY;
                    }

                    alpha = Math.Max(alpha, maxEval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return (maxEval, bestX, bestY);
            }
            else
            {
                int minEval = Int32.MaxValue;
                foreach ((int moveX, int moveY) in possibleMoves)
                {
                    board[moveX, moveY] = currentPlayer;

                    var (childScore, _, _) = Minimax(board, currentDepth - 1, alpha, beta, -currentPlayer);

                    board[moveX, moveY] = 0;

                    if (childScore < minEval)
                    {
                        minEval = childScore;
                        bestX = moveX;
                        bestY = moveY;
                    }

                    beta = Math.Min(beta, minEval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return (minEval, bestX, bestY);
            }
        }

        private List<(int x, int y)> GeneratePossibleMoves(int[,] board, int realPlayer)
        {
            var moves = new List<(int x, int y)>();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] == 0)
                    {
                        if (realPlayer == 1 && rule is Renju renjuRule)
                        {
                            board[x, y] = 1;

                            if (renjuRule.IsForbiddenMove(board))
                            {
                                board[x, y] = 0;
                                continue;
                            }

                            board[x, y] = 0;
                        }

                        if (IsMoveNearStones(board, x, y))
                        {
                            moves.Add((x, y));
                        }
                    }
                }
            }

            moves = moves.OrderByDescending(move => GetMovePriority(board, move.x, move.y)).ToList();

            return moves;
        }

        private bool IsMoveNearStones(int[,] board, int x, int y)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < board.GetLength(0) && ny >= 0 && ny < board.GetLength(1))
                    {
                        if (board[nx, ny] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int GetMovePriority(int[,] board, int x, int y)
        {
            int score = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < board.GetLength(0) && ny >= 0 && ny < board.GetLength(1))
                    {
                        if (board[nx, ny] != 0)
                        {
                            score++;
                        }
                    }
                }
            }
            return score;
        }

        private bool IsBoardFull(int[,] board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
