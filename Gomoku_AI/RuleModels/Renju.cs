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
            if (player == 1)
            {
                if (IsForbiddenMove(board))
                {
                    Console.WriteLine("Forbidden move");
                    return false;
                }
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
            int fourCount = CountOpenOrClosedFours(board, 1);
            if (fourCount >= 2)
            {
                return true;
            }

            int threeCount = CountOpenOrClosedThrees(board, 1);
            if (threeCount >= 2)
            {
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

        private int CountOpenOrClosedFours(int[,] board, int player)
        {
            int foursCount = 0;

            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y <= boardSizeY - 4; y++)
                {
                    if (board[x, y] == player &&
                        board[x, y + 1] == player &&
                        board[x, y + 2] == player &&
                        board[x, y + 3] == player)
                    {
                        foursCount++;
                    }
                }
            }

            for (int y = 0; y < boardSizeY; y++)
            {
                for (int x = 0; x <= boardSizeX - 4; x++)
                {
                    if (board[x, y] == player &&
                        board[x + 1, y] == player &&
                        board[x + 2, y] == player &&
                        board[x + 3, y] == player)
                    {
                        foursCount++;
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 4; x++)
            {
                for (int y = 0; y <= boardSizeY - 4; y++)
                {
                    if (board[x, y] == player &&
                        board[x + 1, y + 1] == player &&
                        board[x + 2, y + 2] == player &&
                        board[x + 3, y + 3] == player)
                    {
                        foursCount++;
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 4; x++)
            {
                for (int y = 3; y < boardSizeY; y++)
                {
                    if (board[x, y] == player &&
                        board[x + 1, y - 1] == player &&
                        board[x + 2, y - 2] == player &&
                        board[x + 3, y - 3] == player)
                    {
                        foursCount++;
                    }
                }
            }

            return foursCount;
        }


        private int CountOpenOrClosedThrees(int[,] board, int player)
        {
            int threesCount = 0;

            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y <= boardSizeY - 3; y++)
                {
                    if (IsThree(board, player, x, y, 0, 1))
                    {
                        threesCount++;
                    }
                }
            }

            for (int y = 0; y < boardSizeY; y++)
            {
                for (int x = 0; x <= boardSizeX - 3; x++)
                {
                    if (IsThree(board, player, x, y, 1, 0)) 
                    {
                        threesCount++;
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 3; x++)
            {
                for (int y = 0; y <= boardSizeY - 3; y++)
                {
                    if (IsThree(board, player, x, y, 1, 1))
                    {
                        threesCount++;
                    }
                }
            }

            for (int x = 0; x <= boardSizeX - 3; x++)
            {
                for (int y = 2; y < boardSizeY; y++)
                {
                    if (IsThree(board, player, x, y, 1, -1)) 
                    {
                        threesCount++;
                    }
                }
            }

            return threesCount;
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

            if (isOpenLeft && isOpenRight) return true;

            if (isOpenLeft || isOpenRight) return true;

            return false;
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
