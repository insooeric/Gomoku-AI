using Gomoku_AI.RuleModels;

namespace Gomoku_AI.AIModels.HMA_B
{
    public class Logic
    {
        private readonly int boardSizeX;
        private readonly int boardSizeY;
        private readonly int depth;
        private readonly IRule rule;

        private const int WIN_SCORE = 1000000;
        private const int LOSE_SCORE = -1000000;

        private int realPlayer;

        private readonly Evaluator evaluator;

        public Logic(int boardSizeX, int boardSizeY, int depth, IRule rule)
        {
            this.boardSizeX = boardSizeX;
            this.boardSizeY = boardSizeY;
            this.depth = depth;
            this.rule = rule;
            this.evaluator = new Evaluator(boardSizeX, boardSizeY, rule);
        }

        public (double, int, int) GetBestMove(int[,] board, int currentPlayer)
        {
            int[,] tmpBoard = (int[,])board.Clone();

            if(currentPlayer == -1)
            {
                for (int x = 0; x < boardSizeX; x++)
                {
                    for (int y = 0; y < boardSizeY; y++)
                    {
                        tmpBoard[x, y] = -tmpBoard[x, y];
                    }
                }
                realPlayer = -1;
            } else
            {
                realPlayer = 1;
            }

            int score = 0;
            int bestX = -1;
            int bestY = -1;


            (score, bestX, bestY) = Minimax(
                tmpBoard,
                currentDepth: depth,
                alpha: Int32.MinValue,
                beta: Int32.MaxValue,
                currentPlayer: currentPlayer
            );

            return (score, bestX, bestY);
        }

        private (int score, int x, int y) Minimax(int[,] board, int currentDepth, int alpha, int beta, int currentPlayer)
        {
            int evaluation = evaluator.EvaluateBoard(board);
            // Console.WriteLine($"Evaluation: {evaluation}");
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
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
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
                        moves.Add((x, y));
                    }
                }
            }
            return moves;
        }

        private bool IsBoardFull(int[,] board)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
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
