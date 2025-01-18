namespace Gomoku_AI.AIModels.MCTS
{
    public static class BoardVisualizer
    {
        public static void PrintBoard(int[,] board)
        {
            Console.Write("  ");
            for (int i = 0; i < board.GetLength(1); i++)
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                Console.Write($"{i} ");
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    switch (board[i, j])
                    {
                        case 0:
                            Console.Write(". ");
                            break;
                        case 1:
                            Console.Write("B ");
                            break;
                        case -1:
                            Console.Write("W ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
