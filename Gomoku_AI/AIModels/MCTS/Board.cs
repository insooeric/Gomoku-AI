namespace Gomoku_AI.AIModels.MCTS
{
    public enum Cell
    {
        Empty = 0,
        Black = 1,
        White = -1
    }

    public class Board
    {
        public int Size { get; } = 15;
        public Cell[,] Grid { get; private set; }

        public Board()
        {
            Grid = new Cell[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Grid[i, j] = Cell.Empty;
        }

        public Board(Cell[,] grid)
        {
            Size = grid.GetLength(0);
            Grid = (Cell[,])grid.Clone();
        }

        public bool IsEmpty(int x, int y) => Grid[x, y] == Cell.Empty;

        public void PlaceStone(int x, int y, Cell player)
        {
            Grid[x, y] = player;
        }

        public Cell GetCell(int x, int y)
        {
            return Grid[x, y];
        }

        public Board Clone()
        {
            return new Board(this.Grid);
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }
    }

}
