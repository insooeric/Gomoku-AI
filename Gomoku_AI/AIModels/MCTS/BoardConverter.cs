namespace Gomoku_AI.AIModels.MCTS
{
    public static class BoardConverter
    {
        public static Cell[,] ConvertIntToCell(int[,] intBoard)
        {
            int sizeX = intBoard.GetLength(0);
            int sizeY = intBoard.GetLength(1);
            Cell[,] cellBoard = new Cell[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    cellBoard[x, y] = intBoard[x, y] switch
                    {
                        1 => Cell.Black,
                        -1 => Cell.White,
                        _ => Cell.Empty
                    };
                }
            }

            return cellBoard;
        }
    }

}
