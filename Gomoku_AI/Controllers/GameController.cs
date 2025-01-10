using Microsoft.AspNetCore.Mvc;
using Gomoku_AI.Models;
using Gomoku_AI.Utilities;
using Gomoku_AI.RuleModels;
using Gomoku_AI.AIModels.HMA_B;

namespace Gomoku_AI.Controllers
{
    [ApiController]
    [Route("api/gomoku")]
    public class GameController : ControllerBase
    {
        [HttpPost("minimax-move")]
        public IActionResult GetMinimaxMove([FromBody] InputModel request)
        {
            int[,] board = ListToArray.Convert(request.Board);
            int currentPlayer = CurrentPlayer.Get(board);

            int depth = request.Depth;

            var validationResult = Validator.ValidateInput(request);

            if (
                validationResult.ErrorCode != Validator.ErrorCode.None && 
                validationResult.ErrorCode != Validator.ErrorCode.InvalidGameStatus
                )
            {
                return BadRequest(new
                {
                    status = validationResult.Status,
                    x = -1,
                    y = -1,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = validationResult.Message
                });
            }

            if(validationResult.ErrorCode == Validator.ErrorCode.InvalidGameStatus)
            {
                return Ok(new
                {
                    status = validationResult.Status,
                    x = -1,
                    y = -1,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = validationResult.Message
                });
            }

            IRule? rule = null;
            if (string.Equals(request.RuleType, "renju"))
            {
                rule = new Renju();
            }
            else if (string.Equals(request.RuleType, "freestyle"))
            {
                rule = new FreeStyle();
            }

            if(rule == null)
            {
                return BadRequest(new { Message = "Invalid rule type." });
            }

            var logic = new Logic(depth, rule);
            var bestMove = logic.GetBestMove(board, currentPlayer);

            int[,] tmpBoard = new int[board.GetLength(0), board.GetLength(1)];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    tmpBoard[i, j] = board[i, j];
                }
            }

            if (bestMove.Item2 >= 0 && bestMove.Item3 >= 0 && bestMove.Item2 < tmpBoard.GetLength(0) && bestMove.Item3 < tmpBoard.GetLength(1))
            {
                tmpBoard[bestMove.Item2, bestMove.Item3] = currentPlayer;
            }

            if (CheckBoardStatus.CheckWinner(tmpBoard) == currentPlayer)
            {
                return Ok(new
                {
                    status = "Win",
                    x = bestMove.Item2,
                    y = bestMove.Item3,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = $"{(currentPlayer == 1 ? "Black" : "White")} Wins 🎉"
                });
            }

            if(CheckBoardStatus.IsBoardFull(tmpBoard))
            {
                return Ok(new
                {
                    status = "Draw",
                    x = -1,
                    y = -1,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "It's a Draw 🤝"
                });
            }

            return Ok(new
            {
                status = "Playing",
                x = bestMove.Item2,
                y = bestMove.Item3,
                color = currentPlayer == 1 ? "Black" : "White",
                message = "Playing"
            });
        }


    }
}
