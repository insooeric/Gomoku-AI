namespace Gomoku_AI.MiddleWares
{
    public class GomokuRenju
    {
        private readonly List<List<int>> _board;
        private const int Black = 1;
        private const int White = -1;

        private int _maxRows;
        private int _maxCols;

        public GomokuRenju(List<List<int>> board)
        {
            _board = board;
            _maxRows = _board.Count;
            _maxCols = _board[0].Count;
        }

        public (bool isVictory, int winningPlayer, bool isForbidden) CheckRenjuAfterMove(int lastX, int lastY, int player)
        {
            if (!IsOnBoard(lastX, lastY))
                return (false, 0, false);

            if (_board[lastX][lastY] == 0)
                return (false, 0, false);

            bool isFiveOrMore = IsFiveOrMore(lastX, lastY, player);
            if (isFiveOrMore)
            {
                bool isOverline = IsOverline(lastX, lastY, player);
                if (player == Black && isOverline)
                {
                    return (false, 0, true);
                }
                else
                {
                    return (true, player, false);
                }
            }

            if (player == Black)
            {
                if (IsOverline(lastX, lastY, Black))
                {
                    return (false, 0, true);
                }
                int fourCount = CountFours(lastX, lastY, Black);
                if (fourCount >= 2)
                {
                    return (false, 0, true);
                }
                int threeCount = CountThrees(lastX, lastY, Black);
                if (threeCount >= 2)
                {
                    return (false, 0, true);
                }
            }

            return (false, 0, false);
        }

        private bool IsFiveOrMore(int x, int y, int player)
        {
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),
                (0, 1),
                (1, 1),
                (1, -1),
            };

            foreach (var (dx, dy) in directions)
            {
                int length = CountConsecutiveStones(x, y, dx, dy, player);
                if (length >= 5)
                    return true;
            }
            return false;
        }

        private bool IsOverline(int x, int y, int player)
        {
            if (player == White) return false;

            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),
                (0, 1),
                (1, 1),
                (1, -1),
            };

            foreach (var (dx, dy) in directions)
            {
                int length = CountConsecutiveStones(x, y, dx, dy, player);
                if (length >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        private int CountFours(int x, int y, int player)
        {
            int count = 0;

            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),
                (0, 1),
                (1, 1),
                (1, -1),
            };

            foreach (var (dx, dy) in directions)
            {
                if (IsFourShape(x, y, dx, dy, player))
                    count++;
            }

            return count;
        }

        private bool IsFourShape(int x, int y, int dx, int dy, int player)
        {
            int continuousCount = CountConsecutiveStones(x, y, dx, dy, player);

            if (continuousCount >= 5)
                return false;

            (int startX, int startY) = GetEdge(x, y, dx, dy, player, true);
            (int endX, int endY) = GetEdge(x, y, dx, dy, player, false);


            bool leftOpen = false, rightOpen = false;

            int leftCheckX = startX - dx;
            int leftCheckY = startY - dy;
            if (IsOnBoard(leftCheckX, leftCheckY) && _board[leftCheckX][leftCheckY] == 0)
            {
                leftOpen = true;
            }

            int rightCheckX = endX + dx;
            int rightCheckY = endY + dy;
            if (IsOnBoard(rightCheckX, rightCheckY) && _board[rightCheckX][rightCheckY] == 0)
            {
                rightOpen = true;
            }

            return (continuousCount == 4) && (leftOpen || rightOpen);
        }

        private int CountThrees(int x, int y, int player)
        {
            int count = 0;
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),
                (0, 1),
                (1, 1),
                (1, -1),
            };

            foreach (var (dx, dy) in directions)
            {
                if (IsThreeShape(x, y, dx, dy, player))
                    count++;
            }

            return count;
        }

        private bool IsThreeShape(int x, int y, int dx, int dy, int player)
        {
            int continuousCount = CountConsecutiveStones(x, y, dx, dy, player);

            if (continuousCount != 3)
                return false;

            (int startX, int startY) = GetEdge(x, y, dx, dy, player, true);
            (int endX, int endY) = GetEdge(x, y, dx, dy, player, false);

            int leftCheckX = startX - dx;
            int leftCheckY = startY - dy;
            int rightCheckX = endX + dx;
            int rightCheckY = endY + dy;

            bool leftOpen = (IsOnBoard(leftCheckX, leftCheckY) && _board[leftCheckX][leftCheckY] == 0);
            bool rightOpen = (IsOnBoard(rightCheckX, rightCheckY) && _board[rightCheckX][rightCheckY] == 0);


            return (leftOpen || rightOpen);
        }

        private int CountConsecutiveStones(int x, int y, int dx, int dy, int player)
        {
            int total = 1;

            int step = 1;
            while (true)
            {
                int nx = x + dx * step;
                int ny = y + dy * step;
                if (!IsOnBoard(nx, ny) || _board[nx][ny] != player)
                    break;
                total++;
                step++;
            }

            step = 1;
            while (true)
            {
                int nx = x - dx * step;
                int ny = y - dy * step;
                if (!IsOnBoard(nx, ny) || _board[nx][ny] != player)
                    break;
                total++;
                step++;
            }

            return total;
        }

        private (int edgeX, int edgeY) GetEdge(int x, int y, int dx, int dy, int player, bool isForward)
        {
            int step = 1;
            int curX = x, curY = y;

            int sign = isForward ? 1 : -1;

            while (true)
            {
                int nx = x + sign * dx * step;
                int ny = y + sign * dy * step;

                if (!IsOnBoard(nx, ny) || _board[nx][ny] != player)
                    break;

                curX = nx;
                curY = ny;
                step++;
            }

            return (curX, curY);
        }

        private bool IsOnBoard(int x, int y)
        {
            return (x >= 0 && x < _maxRows && y >= 0 && y < _maxCols);
        }

        public bool ContainsForbiddenMove()
        {
            for (int x = 0; x < _maxRows; x++)
            {
                for (int y = 0; y < _maxCols; y++)
                {
                    int player = _board[x][y];
                    if (player != Black) continue;

                    var (isVictory, winningPlayer, isForbidden) = CheckRenjuAfterMove(x, y, player);
                    if (isForbidden)
                        return true;
                }
            }
            return false;
        }
    }
}
