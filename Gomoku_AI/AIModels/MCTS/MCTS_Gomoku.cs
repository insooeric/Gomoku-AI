using Gomoku_AI.RuleModels;
using Gomoku_AI.Utilities;
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
        private bool Debug = true;

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
            List<Move> moves = new List<Move>();

            // Instead of dx, dy from {-1,0,1}, expand to {-2,-1,0,1,2}:
            /*            int[] dx = { -2, -1, 0, 1, 2 };
                        int[] dy = { -2, -1, 0, 1, 2 };*/

            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            bool[,] considered = new bool[BoardRow, BoardCol];
            bool boardIsEmpty = true;

            for (int x = 0; x < BoardRow; x++)
            {
                for (int y = 0; y < BoardCol; y++)
                {
                    if (Board[x, y] != 0)
                    {
                        boardIsEmpty = false;
                        // Consider neighbors within 2 steps
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

            // If board is completely empty, you can place multiple 'center-ish' moves
            if (boardIsEmpty)
            {
                // e.g., put a small cluster of possible center moves
                int cR = BoardRow / 2;
                int cC = BoardCol / 2;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int r = cR + i;
                        int c = cC + j;
                        if (IsInside(r, c))
                        {
                            moves.Add(new Move(r, c));
                        }
                    }
                }
            }

/*            foreach (var move in moves)
            {
                if (Debug)
                {
                    Console.WriteLine($"Possible Move: Row {move.Row}, Col {move.Col}");
                }
            }*/

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
