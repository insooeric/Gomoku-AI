using System;

namespace Gomoku_AI.RuleModels
{
    public class Renju : IRule
    {
        private const int WinningCount = 5;

        public Renju()
        {
        }

        public bool IsWinning(int[,] board, int player)
        {
            if (player == 1 && IsForbiddenMove(board))
            {
                return false;
            }

            return CheckRows(board, player)
                || CheckColumns(board, player)
                || CheckDiagonals(board, player)
                || CheckAntiDiagonals(board, player);
        }

        public bool IsForbiddenMove(int[,] board)
        {
            if (HasOverline(board, 1))
            {
                return true;
            }

            int openFours = CountOpenFours(board, 1);
            if (openFours >= 2)
            {
                return true;
            }

            int openThrees = CountOpenThrees(board, 1);
            if (openThrees >= 2)
            {
                return true;
            }
            return false;
        }

        private bool HasOverline(int[,] board, int player)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                int count = 0;
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= 6) return true;
                }
            }

            for (int y = 0; y < board.GetLength(1); y++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= 6) return true;
                }
            }

            for (int d = -board.GetLength(0) + 1; d < board.GetLength(1); d++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    int y = x + d;
                    if (y >= 0 && y < board.GetLength(1))
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= 6) return true;
                    }
                }
            }

            for (int d = 0; d < board.GetLength(0) + board.GetLength(1) - 1; d++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    int y = d - x;
                    if (y >= 0 && y < board.GetLength(1))
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= 6) return true;
                    }
                }
            }

            return false;
        }

        private int CountOpenFours(int[,] board, int player)
        {
            int openFours = 0;

            // Define directions: Horizontal, Vertical, Diagonal ↘, Diagonal ↗
            var directions = new (int dx, int dy)[]
            {
        (0, 1),   // Horizontal
        (1, 0),   // Vertical
        (1, 1),   // Diagonal ↘
        (-1, 1)   // Diagonal ↗
            };

            foreach (var (dx, dy) in directions)
            {
                bool hasOpenFourInDirection = false;

                for (int x = 0; x < board.GetLength(0); x++)
                {
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        // Check if a four-in-a-row starts at (x,y) in direction (dx, dy)
                        bool sequence = true;
                        for (int i = 0; i < 4; i++)
                        {
                            int newX = x + i * dx;
                            int newY = y + i * dy;

                            if (!IsWithinBounds(newX, newY, board) || board[newX, newY] != player)
                            {
                                sequence = false;
                                break;
                            }
                        }

                        if (sequence)
                        {
                            // Check if both ends are open
                            int beforeX = x - dx;
                            int beforeY = y - dy;
                            int afterX = x + 4 * dx;
                            int afterY = y + 4 * dy;

                            bool openStart = IsWithinBounds(beforeX, beforeY, board) && board[beforeX, beforeY] == 0;
                            bool openEnd = IsWithinBounds(afterX, afterY, board) && board[afterX, afterY] == 0;

                            if (openStart && openEnd)
                            {
                                hasOpenFourInDirection = true;
                                break; // Only need one open four per direction
                            }
                        }
                    }

                    if (hasOpenFourInDirection)
                        break; // Move to next direction
                }

                if (hasOpenFourInDirection)
                    openFours++;
            }

            return openFours;
        }


        private int CountOpenThrees(int[,] board, int player)
        {
            int openThrees = 0;

            // Define directions: Horizontal, Vertical, Diagonal ↘, Diagonal ↗
            var directions = new (int dx, int dy)[]
            {
        (0, 1),   // Horizontal
        (1, 0),   // Vertical
        (1, 1),   // Diagonal ↘
        (-1, 1)   // Diagonal ↗
            };

            foreach (var (dx, dy) in directions)
            {
                bool hasOpenThreeInDirection = false;

                for (int x = 0; x < board.GetLength(0); x++)
                {
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        // Check if a three-in-a-row starts at (x,y) in direction (dx, dy)
                        bool sequence = true;
                        for (int i = 0; i < 3; i++)
                        {
                            int newX = x + i * dx;
                            int newY = y + i * dy;

                            if (!IsWithinBounds(newX, newY, board) || board[newX, newY] != player)
                            {
                                sequence = false;
                                break;
                            }
                        }

                        if (sequence)
                        {
                            // Check if both ends are open
                            int beforeX = x - dx;
                            int beforeY = y - dy;
                            int afterX = x + 3 * dx;
                            int afterY = y + 3 * dy;

                            bool openStart = IsWithinBounds(beforeX, beforeY, board) && board[beforeX, beforeY] == 0;
                            bool openEnd = IsWithinBounds(afterX, afterY, board) && board[afterX, afterY] == 0;

                            if (openStart && openEnd)
                            {
                                hasOpenThreeInDirection = true;
                                break; // Only need one open three per direction
                            }
                        }
                    }

                    if (hasOpenThreeInDirection)
                        break; // Move to next direction
                }

                if (hasOpenThreeInDirection)
                    openThrees++;
            }

            return openThrees;
        }


        private bool IsThree(int[,] board, int player, int startX, int startY, int deltaX, int deltaY)
        {
            int count = 0;

            for (int i = 0; i < 3; i++)
            {
                int x = startX + i * deltaX;
                int y = startY + i * deltaY;

                if (board[x, y] == player)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count != 3) return false;

            int prevX = startX - deltaX;
            int prevY = startY - deltaY;
            int nextX = startX + 3 * deltaX;
            int nextY = startY + 3 * deltaY;

            bool isOpenLeft = prevX >= 0 && prevX < board.GetLength(0) && prevY >= 0 && prevY < board.GetLength(1) && board[prevX, prevY] == 0;
            bool isOpenRight = nextX >= 0 && nextX < board.GetLength(0) && nextY >= 0 && nextY < board.GetLength(1) && board[nextX, nextY] == 0;

            return isOpenLeft && isOpenRight;
        }

        private bool CheckRows(int[,] board, int player)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                int count = 0;
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private bool CheckColumns(int[,] board, int player)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                int count = 0;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private bool CheckDiagonals(int[,] board, int player)
        {
            for (int startX = 0; startX <= board.GetLength(0) - WinningCount; startX++)
            {
                for (int startY = 0; startY <= board.GetLength(1) - WinningCount; startY++)
                {
                    int count = 0;
                    for (int i = 0; i < WinningCount; i++)
                    {
                        if (board[startX + i, startY + i] == player)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count == WinningCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckAntiDiagonals(int[,] board, int player)
        {
            for (int startX = 0; startX <= board.GetLength(0) - WinningCount; startX++)
            {
                for (int startY = WinningCount - 1; startY < board.GetLength(1); startY++)
                {
                    int count = 0;
                    for (int i = 0; i < WinningCount; i++)
                    {
                        if (board[startX + i, startY - i] == player)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count == WinningCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsWithinBounds(int r, int c, int[,] board)
        {
            return r >= 0 && r < board.GetLength(0) && c >= 0 && c < board.GetLength(1);
        }

        public IRule Clone()
        {
            // Since Renju is stateless, return a new instance
            return new Renju();
        }
    }
}
