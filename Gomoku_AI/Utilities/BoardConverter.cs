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
    }
}
