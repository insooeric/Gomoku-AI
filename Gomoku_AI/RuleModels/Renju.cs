using System;

namespace Gomoku_AI.RuleModels
{
    public class Renju : IRule
    {
        private readonly int boardSizeX;
        private readonly int boardSizeY;
        private const int WinningCount = 5;

        public Renju(int boardSizeX, int boardSizeY)
        {
            this.boardSizeX = boardSizeX;
            this.boardSizeY = boardSizeY;
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
                Console.WriteLine("Black has overline");
                return true;
            }

            int openFours = CountOpenFours(board, 1);
            if (openFours >= 2)
            {
                Console.WriteLine("Black has two open fours");
                return true;
            }

            int openThrees = CountOpenThrees(board, 1);
            if (openThrees >= 2)
            {
                Console.WriteLine("Black has two open threes");
                return true;
            }

            return false;
        }

        private bool HasOverline(int[,] board, int player)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                int count = 0;
                for (int y = 0; y < boardSizeY; y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= 6) return true;
                }
            }

            for (int y = 0; y < boardSizeY; y++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= 6) return true;
                }
            }

            for (int d = -boardSizeX + 1; d < boardSizeY; d++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    int y = x + d;
                    if (y >= 0 && y < boardSizeY)
                    {
                        count = (board[x, y] == player) ? count + 1 : 0;
                        if (count >= 6) return true;
                    }
                }
            }

            for (int d = 0; d < boardSizeX + boardSizeY - 1; d++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    int y = d - x;
                    if (y >= 0 && y < boardSizeY)
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

            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y <= boardSizeY - 4; y++)
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
                        bool openRight = (y + 4 < boardSizeY) && (board[x, y + 4] == 0);
                        if (openLeft && openRight)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int y = 0; y < boardSizeY; y++)
            {
                for (int x = 0; x <= boardSizeX - 4; x++)
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
                        bool openBottom = (x + 4 < boardSizeX) && (board[x + 4, y] == 0);
                        if (openTop && openBottom)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 4; x++)
            {
                for (int y = 0; y <= boardSizeY - 4; y++)
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
                        bool openEnd = (x + 4 < boardSizeX) && (y + 4 < boardSizeY) && (board[x + 4, y + 4] == 0);
                        if (openStart && openEnd)
                        {
                            openFours++;
                        }
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 4; x++)
            {
                for (int y = 3; y < boardSizeY; y++)
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
                        bool openStart = (x - 1 >= 0) && (y + 1 < boardSizeY) && (board[x - 1, y + 1] == 0);
                        bool openEnd = (x + 4 < boardSizeX) && (y - 4 >= 0) && (board[x + 4, y - 4] == 0);
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

            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y <= boardSizeY - 3; y++)
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
                        bool openRight = (y + 3 < boardSizeY) && (board[x, y + 3] == 0);
                        if (openLeft && openRight)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int y = 0; y < boardSizeY; y++)
            {
                for (int x = 0; x <= boardSizeX - 3; x++)
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
                        bool openBottom = (x + 3 < boardSizeX) && (board[x + 3, y] == 0);
                        if (openTop && openBottom)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 3; x++)
            {
                for (int y = 0; y <= boardSizeY - 3; y++)
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
                        bool openEnd = (x + 3 < boardSizeX) && (y + 3 < boardSizeY) && (board[x + 3, y + 3] == 0);
                        if (openStart && openEnd)
                        {
                            openThrees++;
                        }
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 3; x++)
            {
                for (int y = 2; y < boardSizeY; y++)
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
                        bool openStart = (x - 1 >= 0) && (y + 1 < boardSizeY) && (board[x - 1, y + 1] == 0);
                        bool openEnd = (x + 3 < boardSizeX) && (y - 3 >= 0) && (board[x + 3, y - 3] == 0);
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

            bool isOpenLeft = prevX >= 0 && prevX < boardSizeX && prevY >= 0 && prevY < boardSizeY && board[prevX, prevY] == 0;
            bool isOpenRight = nextX >= 0 && nextX < boardSizeX && nextY >= 0 && nextY < boardSizeY && board[nextX, nextY] == 0;

            return isOpenLeft && isOpenRight;
        }

        private bool CheckRows(int[,] board, int player)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                int count = 0;
                for (int y = 0; y < boardSizeY; y++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private bool CheckColumns(int[,] board, int player)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                int count = 0;
                for (int x = 0; x < boardSizeX; x++)
                {
                    count = (board[x, y] == player) ? count + 1 : 0;
                    if (count >= WinningCount) return true;
                }
            }
            return false;
        }

        private bool CheckDiagonals(int[,] board, int player)
        {
            for (int startX = 0; startX <= boardSizeX - WinningCount; startX++)
            {
                for (int startY = 0; startY <= boardSizeY - WinningCount; startY++)
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
            for (int startX = 0; startX <= boardSizeX - WinningCount; startX++)
            {
                for (int startY = WinningCount - 1; startY < boardSizeY; startY++)
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
    }
}
