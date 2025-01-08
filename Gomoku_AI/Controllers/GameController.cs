using Microsoft.AspNetCore.Mvc;
using Gomoku_AI.Models;
using Gomoku_AI.MiddleWares;

namespace Gomoku_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpPost("minimax-move")]
        // revision
        public IActionResult GetMinimaxMove([FromBody] MinimaxRequest request)
        {
            if (request == null || request.Board == null || request.Board.Count == 0)
                return BadRequest(new
                {
                    Message = "Something went wrong :(",
                    Status = "Error"
                });

            if (request.Board.Count < 5 || request.Board[0].Count < 5)
                return BadRequest(new
                {
                    Message = "Invalid board dimensions. The board must be at least 5x5.",
                    Status = "Error"
                });

            if (request.Board.Any(row => row.Count != request.Board[0].Count) ||
                request.Board.Any(row => row.Any(cell => cell != 0 && cell != 1 && cell != -1)))
                return BadRequest(new
                {
                    Message = "Invalid board values. Only 0 (empty), 1 (black), and -1 (white) are allowed.",
                    Status = "Error"
                });

            var allValues = request.Board.SelectMany(row => row).Where(value => value != 0);
            int blackCount = allValues.Count(value => value == 1);
            int whiteCount = allValues.Count(value => value == -1);

            if (Math.Abs(blackCount - whiteCount) > 1)
                return BadRequest(new
                {
                    Message = "Invalid game. The difference between black and white stones must be 0 or 1.",
                    Status = "Error"
                });

            if (IsForbiddenMove(request.Board))
                return BadRequest(new
                {
                    Message = "Invalid board state. Black made a forbidden move according to Renju rule.",
                    Status = "Error"
                });

            if (HasMultipleWinners(request.Board))
                return BadRequest(new
                {
                    Message = "Invalid board state. Multiple winners detected.",
                    Status = "Error"
                });

            var logic = new GomokuRenju(request.Board);
            var (isVictory, winningPlayer) = HasWinner(request.Board);
            if (isVictory)
            {
                return Ok(new
                {
                    Player = winningPlayer == 1 ? "Black" : "White",
                    Column = -1,
                    Row = -1,
                    Message = "The game is already over.",
                    Status = "Game Over"
                });
            }

            bool isBoardFull = request.Board.SelectMany(row => row).All(v => v != 0);

            if (isBoardFull)
            {
                return Ok(new
                {
                    Player = "Draw",
                    Column = -1,
                    Row = -1,
                    Message = "The game is a draw.",
                    Status = "Draw"
                });
            }

            int currentPlayer = (blackCount > whiteCount) ? -1 : 1;

            MinimaxAI ai = new MinimaxAI(aiPlayer: currentPlayer, humanPlayer: currentPlayer * -1, maxDepth: 4);
            var (bestX, bestY) = ai.GetBestMove(request.Board);

            if (bestY == -1 || bestX == -1)
                return Ok(new
                {
                    Player = currentPlayer == 1 ? "Black" : "White",
                    Row = bestX,
                    Column = bestY,
                    Message = "No empty space",
                    Status = "Draw"
                });

            return Ok(new
            {
                Player = currentPlayer == 1 ? "Black" : "White",
                Row = bestX,
                Column = bestY,
                Message = "Best move found by Minimax",
                Status = "Playing"
            });
        }

        private bool IsForbiddenMove(List<List<int>> board)
        {
            var renju = new GomokuRenju(board);
            return renju.ContainsForbiddenMove();
        }

        private bool HasMultipleWinners(List<List<int>> board)
        {
            int rows = board.Count;
            int cols = board[0].Count;

            var winningPlayers = new HashSet<int>();

            var logic = new GomokuRenju(board);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int cellValue = board[x][y];
                    if (cellValue == 0) continue;

                    var (isVictory, player, isForbidden) = logic.CheckRenjuAfterMove(x, y, cellValue);

                    if (isVictory)
                    {
                        winningPlayers.Add(player);

                        if (winningPlayers.Count > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return winningPlayers.Count > 1;
        }

        private (bool hasSingleWinner, int winnerPlayer) HasWinner(List<List<int>> board)
        {
            int rows = board.Count;
            int cols = board[0].Count;

            var winningPlayers = new HashSet<int>();

            var logic = new GomokuRenju(board);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int cellValue = board[x][y];
                    if (cellValue == 0) continue;

                    var (isVictory, player, isForbidden) = logic.CheckRenjuAfterMove(x, y, cellValue);

                    if (isVictory)
                    {
                        winningPlayers.Add(player);

                        if (winningPlayers.Count > 1)
                        {
                            return (false, 0);
                        }
                    }
                }
            }

            if (winningPlayers.Count == 1)
            {
                return (true, winningPlayers.First());
            }
            else
            {
                return (false, 0);
            }
        }
    }
}
