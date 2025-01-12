using Gomoku_AI.RuleModels;
using System.Xml.Linq;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Gomoku
    {
        public int[,] Board { get; set; }
        public int BoardRow;
        public int BoardCol;
        public int CurrentPlayer;
        public IRule _rule;

        public MCTS_Gomoku(int[,] board, int currentPlayer, IRule rule)
        {
            Board = (int[,])board.Clone(); // Deep clone to prevent mutations
            BoardRow = board.GetLength(0);
            BoardCol = board.GetLength(1);
            CurrentPlayer = currentPlayer;
            _rule = rule;
        }


        public List<Move> GetPossibleMoves()
        {
            // Implement move generation with rule considerations if necessary
            List<Move> moves = new List<Move>();

            // Heuristic: consider empty cells adjacent to existing stones
            bool[,] considered = new bool[BoardRow, BoardCol];
            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            for (int x = 0; x < BoardRow; x++)
            {
                for (int y = 0; y < BoardCol; y++)
                {
                    if (Board[x, y] != 0)
                    {
                        foreach (var deltaX in dx)
                        {
                            foreach (var deltaY in dy)
                            {
                                int newX = x + deltaX;
                                int newY = y + deltaY;
                                if (IsInside(newX, newY) && Board[newX, newY] == 0 && !considered[newX, newY])
                                {
                                    moves.Add(new Move(newX, newY));
                                    considered[newX, newY] = true;
                                }
                            }
                        }
                    }
                }
            }

            // If the board is empty, return the center
            if (moves.Count == 0)
            {
                int centerRow = BoardRow / 2;
                int centerCol = BoardCol / 2;
                moves.Add(new Move(centerRow, centerCol));
            }

            // Logging for debugging
            // Console.WriteLine("Generated Possible Moves:");
            foreach (var move in moves)
            {
                // Console.WriteLine($"Row: {move.Row}, Col: {move.Col}");
            }

            return moves;
        }



        public void ApplyMove(Move move)
        {
            // Console.WriteLine($"Applying Move: Row {move.Row}, Col {move.Col}");

            if (IsValidMove(move))
            {
                Board[move.Row, move.Col] = CurrentPlayer;
                CurrentPlayer = -CurrentPlayer; // Switch player
                // Console.WriteLine($"Move applied. New CurrentPlayer: {(CurrentPlayer == 1 ? "Black" : "White")}");
            }
            else
            {
                // Console.WriteLine($"Invalid Move Attempted: Row {move.Row}, Col {move.Col}");
                throw new InvalidOperationException("Invalid Move");
            }
        }


        public bool IsValidMove(Move move)
        {
            return IsInside(move.Row, move.Col) && Board[move.Row, move.Col] == 0;
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < BoardRow && y >= 0 && y < BoardCol;
        }

        public int CheckWinner()
        {
            // Check if Black (1) has won
            if (_rule.IsWinning(Board, 1))
            {
                return 1;
            }

            // Check if White (-1) has won
            if (_rule.IsWinning(Board, -1))
            {
                return -1;
            }

            // No winner
            return 0;
        }


        public bool IsGameOver()
        {
            return CheckWinner() != 0 || GetPossibleMoves().Count == 0;
        }

        public MCTS_Gomoku Clone()
        {
            int[,] clonedBoard = (int[,])Board.Clone(); // Deep clone the board
            return new MCTS_Gomoku(clonedBoard, CurrentPlayer, _rule);
        }

    }
}
