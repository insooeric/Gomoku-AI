using Gomoku_AI.AIModels.MCTS;

namespace Gomoku_AI.Utilities
{
    public class BoardConverter
    {
        public static int[,] ListToArray(List<List<int>> list)
        {
            int[,] array = new int[list.Count, list[0].Count];
            for(int i = 0; i < list.Count; i++)
            {
                for(int j = 0; j < list[i].Count; j++)
                {
                    array[i,j] = list[i][j];
                }
            }

            return array;
        }
        public static Cell[,] ArrayToCell(int[,] arrayBoard)
        {
            int sizeX = arrayBoard.GetLength(0);
            int sizeY = arrayBoard.GetLength(1);
            Cell[,] cellBoard = new Cell[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    cellBoard[x, y] = arrayBoard[x, y] switch
                    {
                        1 => Cell.Black,
                        2 => Cell.White,
                        _ => Cell.Empty
                    };
                }
            }

            return cellBoard;
        }
    }
}
