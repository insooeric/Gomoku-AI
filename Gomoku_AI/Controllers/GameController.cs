using Microsoft.AspNetCore.Mvc;
using Gomoku_AI.Models;
using Gomoku_AI.Utilities;
using Gomoku_AI.RuleModels;
using Gomoku_AI.AIModels.HMA_B;
using Gomoku_AI.AIModels.MCTS;

namespace Gomoku_AI.Controllers
{
    [ApiController]
    [Route("api/gomoku")]
    public class GameController : ControllerBase
    {
        // optimize stuffs
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

            var logic = new HMA_B_Logic(depth, rule);
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

        [HttpPost("mcts-move")]
        public IActionResult GetMCTSMove([FromBody] InputModel request)
        {

            
            // **Convert int[,] to Cell[,]**
            var board = ListToArray.Convert(request.Board);
            int currentPlayer = CurrentPlayer.Get(board);

            var validationResult = Validator.ValidateInput(request);

            if(
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
            if(string.Equals(request.RuleType, "renju"))
            {
                rule = new Renju();
            }
            else if (string.Equals(request.RuleType, "freestyle"))
            {
                rule = new FreeStyle();
            }

            if(rule == null)
            {
                return BadRequest(new {Message = "Invalid rule type."});
            }

            var cellBoard = BoardConverter.ConvertIntToCell(board);

            var gameBoard = new Board(cellBoard);

            Cell currentPlayerValue = currentPlayer == 1 ? Cell.Black : Cell.White;

            // Initialize the MovePrioritizer
            var prioritizer = new MovePrioritizer(gameBoard, currentPlayerValue);
            var prioritizedMoves = prioritizer.GetPrioritizedMoves();

            // If high-priority moves are found, select the first one
            if (prioritizedMoves.Any())
            {
                var selectedMove = prioritizedMoves.First();
                return Ok(new
                {
                    status = "Playing",
                    x = selectedMove.X,
                    y = selectedMove.Y,
                    color = currentPlayerValue == Cell.Black ? "Black" : "White",
                    message = "Playing"
                });
            }

            // If no high-priority moves, proceed with MCTS
            var mcts = new MCTS(iterations: 1000, aiPlayer: currentPlayerValue);
            var bestMove = mcts.FindBestMove(gameBoard);

            // If MCTS couldn't find a move (shouldn't happen), choose a random available move
            if (bestMove == null)
            {
                var availableMoves = GetAvailableMoves(gameBoard);
                if (availableMoves.Any())
                    bestMove = availableMoves.First();
                else
                    return Ok(new
                    {
                        status = "Draw",
                        x = -1,
                        y = -1,
                        color = currentPlayerValue == Cell.Black ? "Black" : "White",
                        message = "Draw"
                    });
            }

            return Ok(new
            {
                status = "Playing",
                x = bestMove.X,
                y = bestMove.Y,
                color = currentPlayerValue == Cell.Black ? "Black" : "White",
                message = "Playing"
            });
        }

        private List<Move> GetAvailableMoves(Board board)
        {
            var moves = new List<Move>();
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    if (board.IsEmpty(x, y))
                        moves.Add(new Move(x, y));
            return moves;
        }
    }
}
