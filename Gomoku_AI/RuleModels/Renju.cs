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

            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y <= board.GetLength(1) - 4; y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (board[x, y + i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openLeft = (y - 1 >= 0) && (board[x, y - 1] == 0);
                        bool openRight = (y + 4 < board.GetLength(1)) && (board[x, y + 4] == 0);
                        if (openLeft && openRight)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x <= board.GetLength(0) - 4; x++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (board[x + i, y] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openTop = (x - 1 >= 0) && (board[x - 1, y] == 0);
                        bool openBottom = (x + 4 < board.GetLength(0)) && (board[x + 4, y] == 0);
                        if (openTop && openBottom)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int x = 0; x <= board.GetLength(0) - 4; x++)
            {
                for (int y = 0; y <= board.GetLength(1) - 4; y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (board[x + i, y + i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openStart = (x - 1 >= 0) && (y - 1 >= 0) && (board[x - 1, y - 1] == 0);
                        bool openEnd = (x + 4 < board.GetLength(0)) && (y + 4 < board.GetLength(1)) && (board[x + 4, y + 4] == 0);
                        if (openStart && openEnd)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int x = 0; x <= board.GetLength(0) - 4; x++)
            {
                for (int y = 3; y < board.GetLength(1); y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (board[x + i, y - i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openStart = (x - 1 >= 0) && (y + 1 < board.GetLength(1)) && (board[x - 1, y + 1] == 0);
                        bool openEnd = (x + 4 < board.GetLength(0)) && (y - 4 >= 0) && (board[x + 4, y - 4] == 0);
                        if (openStart && openEnd)
                        {
                            openFours++;
                        }
                    }
                }
            }

            return openFours;
        }

        private int CountOpenThrees(int[,] board, int player)
        {
            int openThrees = 0;

            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y <= board.GetLength(1) - 3; y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (board[x, y + i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openLeft = (y - 1 >= 0) && (board[x, y - 1] == 0);
                        bool openRight = (y + 3 < board.GetLength(1)) && (board[x, y + 3] == 0);
                        if (openLeft && openRight)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x <= board.GetLength(0) - 3; x++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (board[x + i, y] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openTop = (x - 1 >= 0) && (board[x - 1, y] == 0);
                        bool openBottom = (x + 3 < board.GetLength(0)) && (board[x + 3, y] == 0);
                        if (openTop && openBottom)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int x = 0; x <= board.GetLength(0) - 3; x++)
            {
                for (int y = 0; y <= board.GetLength(1) - 3; y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (board[x + i, y + i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openStart = (x - 1 >= 0) && (y - 1 >= 0) && (board[x - 1, y - 1] == 0);
                        bool openEnd = (x + 3 < board.GetLength(0)) && (y + 3 < board.GetLength(1)) && (board[x + 3, y + 3] == 0);
                        if (openStart && openEnd)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int x = 0; x <= board.GetLength(0) - 3; x++)
            {
                for (int y = 2; y < board.GetLength(1); y++)
                {
                    bool sequence = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (board[x + i, y - i] != player)
                        {
                            sequence = false;
                            break;
                        }
                    }
                    if (sequence)
                    {
                        bool openStart = (x - 1 >= 0) && (y + 1 < board.GetLength(1)) && (board[x - 1, y + 1] == 0);
                        bool openEnd = (x + 3 < board.GetLength(0)) && (y - 3 >= 0) && (board[x + 3, y - 3] == 0);
                        if (openStart && openEnd)
                        {
                            openThrees++;
                        }
                    }
                }
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

        public IRule Clone()
        {
            // Since Renju is stateless, return a new instance
            return new Renju();
        }
    }
}
