namespace Gomoku_AI.AIModels.MCTS
{
    public class PatternDetector
    {
        private readonly Board _board;
        private readonly Cell _player;

        public PatternDetector(Board board, Cell player)
        {
            _board = board;
            _player = player;
        }

        // Directions: Horizontal, Vertical, Diagonal (/), Diagonal (\)
        private readonly (int dx, int dy)[] Directions = new (int, int)[]
        {
        (1, 0),  // Horizontal
        (0, 1),  // Vertical
        (1, 1),  // Diagonal \
        (1, -1)  // Diagonal /
        };

        // Check if placing at (x, y) creates a five in a row
        public bool CreatesFive(int x, int y)
        {
            foreach (var (dx, dy) in Directions)
            {
                int count = 1;
                // Check in the positive direction
                int i = 1;
                while (IsValid(x + dx * i, y + dy * i) && _board.GetCell(x + dx * i, y + dy * i) == _player)
                {
                    count++;
                    i++;
                }

                // Check in the negative direction
                i = 1;
                while (IsValid(x - dx * i, y - dy * i) && _board.GetCell(x - dx * i, y - dy * i) == _player)
                {
                    count++;
                    i++;
                }

                if (count >= 5)
                    return true;
            }
            return false;
        }

        // Check if placing at (x, y) creates a four in a row with at least one open end
        public bool CreatesOpenFour(int x, int y)
        {
            foreach (var (dx, dy) in Directions)
            {
                int count = 1;
                bool openEnds = false;

                // Positive direction
                int i = 1;
                while (IsValid(x + dx * i, y + dy * i) && _board.GetCell(x + dx * i, y + dy * i) == _player)
                {
                    count++;
                    i++;
                }
                // Check if the end is open
                if (IsValid(x + dx * i, y + dy * i) && _board.GetCell(x + dx * i, y + dy * i) == Cell.Empty)
                    openEnds = true;

                // Negative direction
                i = 1;
                while (IsValid(x - dx * i, y - dy * i) && _board.GetCell(x - dx * i, y - dy * i) == _player)
                {
                    count++;
                    i++;
                }
                // Check if the end is open
                if (IsValid(x - dx * i, y - dy * i) && _board.GetCell(x - dx * i, y - dy * i) == Cell.Empty)
                    openEnds = true;

                if (count == 4 && openEnds)
                    return true;
            }
            return false;
        }

        // Check if placing at (x, y) creates a three in a row with both ends open
        public bool CreatesOpenThree(int x, int y)
        {
            foreach (var (dx, dy) in Directions)
            {
                int count = 1;
                bool openEnds = false;

                // Positive direction
                int i = 1;
                while (IsValid(x + dx * i, y + dy * i) && _board.GetCell(x + dx * i, y + dy * i) == _player)
                {
                    count++;
                    i++;
                }
                // Check if the end is open
                if (IsValid(x + dx * i, y + dy * i) && _board.GetCell(x + dx * i, y + dy * i) == Cell.Empty)
                    openEnds = true;

                // Negative direction
                i = 1;
                while (IsValid(x - dx * i, y - dy * i) && _board.GetCell(x - dx * i, y - dy * i) == _player)
                {
                    count++;
                    i++;
                }
                // Check if the end is open
                if (IsValid(x - dx * i, y - dy * i) && _board.GetCell(x - dx * i, y - dy * i) == Cell.Empty)
                    openEnds = true;

                if (count == 3 && openEnds)
                    return true;
            }
            return false;
        }

        public bool CreatesOpenTwo(int x, int y)
        {

            foreach (var (dx, dy) in Directions)
            {

            }
            return false;
        }

        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < _board.Size && y >= 0 && y < _board.Size;
        }
    }

}
