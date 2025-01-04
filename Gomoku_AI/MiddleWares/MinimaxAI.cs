

namespace Gomoku_AI.MiddleWares
{
    public class MinimaxAI
    {
        private readonly int maxDepth;
        private readonly int aiPlayer;
        private readonly int humanPlayer;

        public MinimaxAI(int aiPlayer = 2, int humanPlayer = 1, int maxDepth = 3)
        {
            this.aiPlayer = aiPlayer;
            this.humanPlayer = humanPlayer;
            this.maxDepth = maxDepth;
        }

        public (int row, int col) GetBestMove(List<List<int>> board)
        {
            int bestScore = int.MinValue;
            (int row, int col) bestMove = (-1, -1);

            foreach (var move in GetPossibleMoves(board))
            {
                board[move.row][move.col] = aiPlayer;

                var logic = new GomokuRenju(board);
                var result = logic.CheckRenjuAfterMove(move.row, move.col, aiPlayer);

                if (result.isForbidden)
                {
                    board[move.row][move.col] = 0;
                    continue;
                }

                int score = Minimax(board, maxDepth - 1, false, int.MinValue, int.MaxValue);
                board[move.row][move.col] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(List<List<int>> board, int depth, bool isMaximizing, int alpha, int beta)
        {
            var logic = new GomokuRenju(board);

            var evaluation = EvaluateBoard(board, logic);
            if (Math.Abs(evaluation) == 1000 || depth == 0)
            {
                return evaluation;
            }

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                foreach (var move in GetPossibleMoves(board))
                {
                    board[move.row][move.col] = aiPlayer;

                    var moveResult = logic.CheckRenjuAfterMove(move.row, move.col, aiPlayer);
                    if (moveResult.isForbidden)
                    {
                        board[move.row][move.col] = 0;
                        continue;
                    }

                    int eval = Minimax(board, depth - 1, false, alpha, beta);
                    board[move.row][move.col] = 0;
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in GetPossibleMoves(board))
                {
                    board[move.row][move.col] = humanPlayer;

                    var moveResult = logic.CheckRenjuAfterMove(move.row, move.col, humanPlayer);
                    if (moveResult.isForbidden)
                    {
                        board[move.row][move.col] = 0;
                        continue;
                    }

                    int eval = Minimax(board, depth - 1, true, alpha, beta);
                    board[move.row][move.col] = 0;
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(List<List<int>> board, GomokuRenju logic)
        {
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board[i].Count; j++)
                {
                    if (board[i][j] != 0)
                    {
                        var result = logic.CheckRenjuAfterMove(i, j, board[i][j]);
                        if (result.isVictory)
                        {
                            if (result.winningPlayer == aiPlayer)
                                return 1000;
                            else if (result.winningPlayer == humanPlayer)
                                return -1000;
                        }
                    }
                }
            }

            int aiScore = CountPatterns(board, aiPlayer);
            int humanScore = CountPatterns(board, humanPlayer);
            return aiScore - humanScore;
        }

        private int CountPatterns(List<List<int>> board, int player)
        {
            int score = 0;
            int size = board.Count;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (board[i][j] == player)
                    {
                        if (j <= size - 3 && board[i][j + 1] == player && board[i][j + 2] == player)
                            score += 10;
                        if (i <= size - 3 && board[i + 1][j] == player && board[i + 2][j] == player)
                            score += 10;
                        if (i <= size - 3 && j <= size - 3 &&
                            board[i + 1][j + 1] == player && board[i + 2][j + 2] == player)
                            score += 10;
                        if (i >= 2 && j <= size - 3 &&
                            board[i - 1][j + 1] == player && board[i - 2][j + 2] == player)
                            score += 10;
                    }
                }
            }

            return score;
        }

        private List<(int row, int col)> GetPossibleMoves(List<List<int>> board)
        {
            var moves = new List<(int row, int col)>();
            int size = board.Count;

            var directions = new (int dRow, int dCol)[]
            {
                (-1, -1), (-1, 0), (-1, 1),
                (0, -1),         (0, 1),
                (1, -1),  (1, 0), (1, 1)
            };

            var potentialMoves = new HashSet<(int, int)>();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (board[i][j] != 0)
                    {
                        foreach (var (dRow, dCol) in directions)
                        {
                            int newRow = i + dRow;
                            int newCol = j + dCol;
                            if (newRow >= 0 && newRow < size && newCol >= 0 && newCol < size &&
                                board[newRow][newCol] == 0)
                            {
                                potentialMoves.Add((newRow, newCol));
                            }
                        }
                    }
                }
            }

            if (potentialMoves.Count == 0)
            {
                int center = size / 2;
                return new List<(int, int)> { (center, center) };
            }

            moves = potentialMoves.ToList();
            return moves;
        }
    }
}
