using Gomoku_AI.Models;

namespace Gomoku_AI.Utilities
{
    public static class Validator
    {
        public enum ErrorCode
        {
            None = 0,
            InvalidBoard = 1,
            InvalidDepth = 2,
            InvalidRuleType = 3,
            InvalidStoneCount = 4,
            InvalidBoardValues = 5,
            InvalidGameStatus = 6
        }
        public struct ResultModel
        {
            public ErrorCode ErrorCode;
            public string Message;
            public string Status;
        }
        public static ResultModel ValidateInput(InputModel input)
        {
            if (input.Board == null || input.Board.Count < 8)
            {
                return new ResultModel { 
                    ErrorCode = ErrorCode.InvalidBoard, 
                    Message = "Invalid board. Board size should be larger than 7 x 7.", 
                    Status = "Error" 
                };
            }

            if (input.Depth < 1)
            {
                return new ResultModel { 
                    ErrorCode = ErrorCode.InvalidDepth, 
                    Message = "Invalid depth. Depth should be larger than 0.", 
                    Status = "Error" 
                };
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
                    return new ResultModel { 
                        ErrorCode = ErrorCode.InvalidRuleType, 
                        Message = txt, 
                        Status = "Error" 
                    };
            }

            int blackCount = input.Board.Sum(row => row.Count(cell => cell == 1));
            int whiteCount = input.Board.Sum(row => row.Count(cell => cell == -1));


            if (blackCount - whiteCount < 0 || blackCount - whiteCount > 1)
            {
                return new ResultModel { 
                    ErrorCode = ErrorCode.InvalidStoneCount, 
                    Message = "Invalid stone count. Number of black stones should be equal to or larger by 1 then white stones.", 
                    Status = "Error" 
                };
            }

            foreach (var row in input.Board)
            {
                if (row.Any(cell => cell != 1 && cell != -1 && cell != 0))
                {
                    return new ResultModel { 
                        ErrorCode = ErrorCode.InvalidBoardValues, 
                        Message = "Invalid board values. The board can only contain 1 (Black), -1 (White), or 0 (Empty).", 
                        Status = "Error" 
                    };
                }
            }
            int winner = CheckBoardStatus.CheckWinner(BoardConverter.ListToArray(input.Board));
            int currentPlayer = (blackCount == whiteCount) ? 1 : -1;
            if (winner != 0)
            {
                if (winner == currentPlayer)
                {
                    return new ResultModel
                    {
                        ErrorCode = ErrorCode.InvalidGameStatus,
                        Message = $"{(currentPlayer == 1 ? "Black" : "White")} Wins 🎉",
                        Status = "Win"
                    };
                }
                if (winner != currentPlayer)
                {
                    return new ResultModel
                    {
                        ErrorCode = ErrorCode.InvalidGameStatus,
                        Message = $"{(currentPlayer == 1 ? "White" : "Black")} Wins 🎉",
                        Status = "Lose"
                    };
                }
            }

            if(CheckBoardStatus.IsBoardFull(BoardConverter.ListToArray(input.Board)))
            {
                return new ResultModel
                {
                    ErrorCode = ErrorCode.InvalidGameStatus,
                    Message = "Board is full",
                    Status = "It's a Draw 🤝"
                };
            }

            return new ResultModel { ErrorCode = ErrorCode.None, Message = "Valid input", Status = "Success" };
        }
    }
}
