using Gomoku_AI.Models;

namespace Gomoku_AI.MiddleWares
{
    public class Validator
    {
        public static (ErrorCode, string) ValidateInput(InputModel input)
        {
            if (input.Board == null || input.Board.Count < 8)
            {
                return (ErrorCode.InvalidBoard, "Invalid board. Board size should be larger than 7 x 7.");
            }

            if (input.Depth < 1)
            {
                return (ErrorCode.InvalidDepth, "Invalid depth. Depth should be larger than 0.");
            }

            string txt =
                $"{input.RuleType} is invalid rule.\n Rule should be one of the followings:\n" +
                "- freestyle\n" +
                "- renju\n";

            switch (input.RuleType)
            {
                case "freestyle":
                    break;
                case "renju":
                    break;
                default:
                    return (ErrorCode.InvalidRuleType, txt);
            }

            int blackCount = input.Board.Sum(row => row.Count(cell => cell == 1));
            int whiteCount = input.Board.Sum(row => row.Count(cell => cell == -1));


            if (blackCount - whiteCount < 0 || blackCount - whiteCount > 1)
            {
                return (ErrorCode.InvalidStoneCount, "Invalid stone count. Number of black stones should be equal to or larger by 1 than white stones.");
            }

            foreach (var row in input.Board)
            {
                if (row.Any(cell => cell != 1 && cell != -1 && cell != 0))
                {
                    return (ErrorCode.InvalidBoardValues, "Invalid board values. The board can only contain 1 (Black), -1 (White), or 0 (Empty).");
                }
            }

            return (ErrorCode.None, "Valid input");
        }
    }
}
