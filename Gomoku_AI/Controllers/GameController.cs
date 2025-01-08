using Microsoft.AspNetCore.Mvc;
using Gomoku_AI.Models;
using Gomoku_AI.MiddleWares;
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
            var validationResult = Validator.ValidateInput(request);
            if (validationResult.Item1 != ErrorCode.None)
            {
                return BadRequest(new
                {
                    ErrorCode = validationResult.Item1,
                    Message = validationResult.Item2
                });
            }

            int boardSizeX = request.Board.Count; 
            int boardSizeY = request.Board[0].Count;

            int[,] board = new int[boardSizeX, boardSizeY];
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
                {
                    board[x, y] = request.Board[x][y];
                }
            }

            int blackCount = board.Cast<int>().Count(v => v == 1);
            int whiteCount = board.Cast<int>().Count(v => v == -1);
            int currentPlayer = (blackCount == whiteCount) ? 1 : -1;

            int depth = request.Depth;

            IRule? rule = null;
            if (string.Equals(request.RuleType, "renju"))
            {
                rule = new Renju(boardSizeX, boardSizeY);
            }
            else if (string.Equals(request.RuleType, "freestyle"))
            {
                rule = new FreeStyle(boardSizeX, boardSizeY);
            }

            if(rule == null)
            {
                return BadRequest(new { Message = "Invalid rule type." });
            }

            var logic = new Logic(boardSizeX, boardSizeY, depth, rule);
            var bestMove = logic.GetBestMove(board, currentPlayer);

            if (bestMove.Item1 == -1 && bestMove.Item2 == -1)
            {
                return Ok(new { Message = "No valid moves available." });
            }

            return Ok(new
            {
                X = bestMove.Item1,
                Y = bestMove.Item2,
                Color = (currentPlayer == 1) ? "Black" : "White"
            });
        }
    }
}
