﻿using Microsoft.AspNetCore.Mvc;
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
        // optimize stuffs
        [HttpPost("minimax-move")]
        public IActionResult GetMinimaxMove([FromBody] InputModel request)
        {

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

            Validator validator = new Validator(boardSizeX, boardSizeY);
            CheckBoardStatus boardValidator = new CheckBoardStatus(boardSizeX, boardSizeY);

            var validationResult = validator.ValidateInput(request);

            if (validationResult.Item1 != ErrorCode.None)
            {
                return BadRequest(new
                {
                    status = "Error",
                    errorCode = validationResult.Item1,
                    message = validationResult.Item2
                });
            }

            if (boardValidator.CheckWinner(board) == 1)
            {
                return Ok(new
                {
                    status = "Win",
                    x = -1,
                    y = -1,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "Black Wins 🎉"
                });
            }

            if (boardValidator.CheckWinner(board) == -1)
            {
                return Ok(new
                {
                    status = "Win",
                    x = -1,
                    y = -1,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "White Wins 🎉"
                });
            }

            if (boardValidator.IsBoardFull(board))
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


            IRule ? rule = null;
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

            // CheckBoardStatus gameStatus = new CheckBoardStatus(boardSizeX, boardSizeY);



            var logic = new Logic(boardSizeX, boardSizeY, depth, rule);
            var bestMove = logic.GetBestMove(board, currentPlayer);

            int[,] tmpBoard = new int[boardSizeX, boardSizeY];
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
                {
                    tmpBoard[x, y] = board[x, y];
                }
            }

            if (bestMove.Item2 >= 0 && bestMove.Item3 >= 0 && bestMove.Item2 < boardSizeX && bestMove.Item3 < boardSizeY)
            {
                tmpBoard[bestMove.Item2, bestMove.Item3] = currentPlayer;
            }

            if (boardValidator.CheckWinner(tmpBoard) == 1)
            {
                return Ok(new
                {
                    status = "Win",
                    x = bestMove.Item2,
                    y = bestMove.Item3,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "Black Wins 🎉"
                });
            }

            if (boardValidator.CheckWinner(tmpBoard) == -1)
            {
                return Ok(new
                {
                    status = "Win",
                    x = bestMove.Item2,
                    y = bestMove.Item3,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "White Wins 🎉"
                });
            }

            if (boardValidator.IsBoardFull(tmpBoard))
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

            if (bestMove.Item2 == -1 && bestMove.Item3 == -1)
            {
                return Ok(new
                {
                    status = "NoMove",
                    x = bestMove.Item2,
                    y = bestMove.Item3,
                    color = currentPlayer == 1 ? "Black" : "White",
                    message = "No valid move."
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
