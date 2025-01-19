using Gomoku_AI.RuleModels;
using Gomoku_AI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku_AI.AIModels.MCTS
{
    public static class Prioritizer
    {
        private static bool Debug = false;

        public static List<Move> PickPrioritizedMove(int[,] board, IRule rule)
        {
            // If the board is empty, place the stone in the center.
            if (IsBoardAllZeros(board))
            {
                if (Debug)
                {
                    Console.WriteLine("Initial move detected. Placing in the center.");
                }
                int centerRow = board.GetLength(0) / 2;
                int centerCol = board.GetLength(1) / 2;
                return new List<Move> { new Move(centerRow, centerCol) };
            }

            if (IsBoardFull(board))
            {
                if (Debug)
                {
                    Console.WriteLine("Board is full.");
                }
                return new List<Move>();
            }

            // Categorize move lists based on patterns
            List<Move> createOpenFours = new List<Move>();
            List<Move> blockOpenFours = new List<Move>();

            List<Move> createSemiOpenFours = new List<Move>();
            List<Move> blockSemiOpenFours = new List<Move>();

            List<Move> createOpenThrees = new List<Move>();
            List<Move> blockOpenThrees = new List<Move>();

            List<Move> createSemiOpenThrees = new List<Move>();
            List<Move> blockSemiOpenThrees = new List<Move>();

            List<Move> createOpenTwos = new List<Move>();
            List<Move> blockOpenTwos = new List<Move>();

            List<Move> createSemiOpenTwos = new List<Move>();
            List<Move> blockSemiOpenTwos = new List<Move>();

            List<Move> defaultMoves = new List<Move>();

            int currentPlayer = CurrentPlayer.Get(board);
            int opponentPlayer = -currentPlayer;

            if (Debug)
            {
                Console.WriteLine("\nPrioritizing moves.");
            }

            // Iterate through all cells to categorize potential moves
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == 0)
                    {
                        Move currentMove = new Move(row, col);
                        bool patternMatched = false;
/*                        if (Debug)
                        {
                            Console.WriteLine($"\nEvaluating Move: ({currentMove.Row},{currentMove.Col})");
                        }*/

                        // Immediate Win for AI
                        Move? winningMove = FindImmediateWin(board, rule, currentMove, currentPlayer);
                        if (winningMove != null)
                        {
                            return new List<Move>() { winningMove };
                        }

                        // Block Opponent's Immediate Win
                        Move? blockMove = FindImmediateWin(board, rule, currentMove, opponentPlayer);
                        if (blockMove != null)
                        {
                            return new List<Move>() { blockMove };
                        }

                        // Create Four for AI
                        Move? openFourMove = FindOpenFour(board, rule, currentMove, currentPlayer);
                        if (openFourMove != null)
                        {
                            patternMatched = true;
                            createOpenFours.Add(openFourMove);
                        }

                        // Block Opponent's Four
                        Move? blockOpenFourMove = FindOpenFour(board, rule, currentMove, opponentPlayer);
                        if (blockOpenFourMove != null)
                        {
                            patternMatched = true;
                            blockOpenFours.Add(blockOpenFourMove);
                        }

                        Move? semiOpenFourMove = FindSemiOpenFour(board, rule, currentMove, currentPlayer);
                        if (semiOpenFourMove != null)
                        {
                            patternMatched = true;
                            createSemiOpenFours.Add(semiOpenFourMove);
                        }

                        Move? blockSemiOpenFourMove = FindSemiOpenFour(board, rule, currentMove, opponentPlayer);
                        if (blockSemiOpenFourMove != null)
                        {
                            patternMatched = true;
                            blockSemiOpenFours.Add(blockSemiOpenFourMove);
                        }

                        Move? openThreeMove = FindOpenThree(board, rule, currentMove, currentPlayer);
                        if(openThreeMove != null)
                        {
                            patternMatched = true;
                            createOpenThrees.Add(openThreeMove);
                        }

                        Move? blockOpenThreeMove = FindOpenThree(board, rule, currentMove, opponentPlayer);
                        if(blockOpenThreeMove != null)
                        {
                            patternMatched = true;
                            blockOpenThrees.Add(blockOpenThreeMove);
                        }

                        Move? openSemiThreeMove = FindSemiOpenThree(board, rule, currentMove, currentPlayer);
                        if (openSemiThreeMove != null)
                        {
                            patternMatched = true;
                            createSemiOpenThrees.Add(openSemiThreeMove);
                        }

                        Move? blockSemiOpenThreeMove = FindSemiOpenThree(board, rule, currentMove, opponentPlayer);
                        if (blockSemiOpenThreeMove != null)
                        {
                            patternMatched = true;
                            blockSemiOpenThrees.Add(blockSemiOpenThreeMove);
                        }

                        Move? openTwoMove = FindOpenTwo(board, rule, currentMove, currentPlayer);
                        if (openTwoMove != null)
                        {
                            patternMatched = true;
                            createOpenTwos.Add(openTwoMove);
                        }

                        Move? blockOpenTwoMove = FindOpenTwo(board, rule, currentMove, opponentPlayer);
                        if (blockOpenTwoMove != null)
                        {
                            patternMatched = true;
                            blockOpenTwos.Add(blockOpenTwoMove);
                        }

                        Move? openSemiTwoMove = FindSemiOpenTwo(board, rule, currentMove, currentPlayer);
                        if (openSemiTwoMove != null)
                        {
                            patternMatched = true;
                            createSemiOpenTwos.Add(openSemiTwoMove);
                        }

                        Move? blockSemiOpenTwoMove = FindSemiOpenTwo(board, rule, currentMove, opponentPlayer);
                        if (blockSemiOpenTwoMove != null)
                        {
                            patternMatched = true;
                            blockSemiOpenTwos.Add(blockSemiOpenTwoMove);
                        }

                        if (!patternMatched)
                        {
                            defaultMoves.Add(currentMove);
                        }
                    }
                }
            }

            /********************
             * Distinct threats *
             ********************/

            // Create Open Fours
            if (createOpenFours.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createOpenFours)
                    {
                        Console.WriteLine($"Create Open Four: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                return createOpenFours.Distinct().ToList();
            }

            // Block Open Fours
            if (blockOpenFours.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockOpenFours)
                    {
                        Console.WriteLine($"Block Open Four: ({move.Row},{move.Col}) for {opponentPlayer}");
                    }
                }
                return blockOpenFours.Distinct().ToList();
            }

            // Create Semi-Open Fours
            if (createSemiOpenFours.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createSemiOpenFours)
                    {
                        Console.WriteLine($"Create Semi-Open Four: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                return createSemiOpenFours.Distinct().ToList();
            }

            // Block Semi-Open Fours
            if (blockSemiOpenFours.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockSemiOpenFours)
                    {
                        Console.WriteLine($"Block Semi-Open Four: ({move.Row},{move.Col}) for {opponentPlayer}");
                    }
                }
                return blockSemiOpenFours.Distinct().ToList();
            }

            // Create Open Threes
            if (createOpenThrees.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createOpenThrees)
                    {
                        Console.WriteLine($"Create Open Three: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                return createOpenThrees.Distinct().ToList();
            }

            // Block Open Threes
            if (blockOpenThrees.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockOpenThrees)
                    {
                        Console.WriteLine($"Block Open Three: ({move.Row},{move.Col}) for {opponentPlayer}");
                    }
                }
                return blockOpenThrees.Distinct().ToList();
            }


            /***********************
             * No distinct threats *
             ***********************/

            List<Move> availableMoves = new List<Move>();

            // Create Semi-Open Threes
            if (createSemiOpenThrees.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createSemiOpenThrees)
                    {
                        Console.WriteLine($"Create Semi-Open Three: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                // return createSemiOpenThrees.Distinct().ToList();
                availableMoves.AddRange(createSemiOpenThrees);
            }

            // Block Semi-Open Threes
            if (blockSemiOpenThrees.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockSemiOpenThrees)
                    {
                        Console.WriteLine($"Block Semi-Open Three: ({move.Row},{move.Col}) for {opponentPlayer}");
                    }
                }
                // return blockSemiOpenThrees.Distinct().ToList();
                availableMoves.AddRange(blockSemiOpenThrees);
            }

            // Create Open Twos
            if (createOpenTwos.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createOpenTwos)
                    {
                        Console.WriteLine($"Create Open Twos: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                // return createOpenTwos.Distinct().ToList();
                availableMoves.AddRange(createOpenTwos);
            }
            // Block Open Twos
            if (blockOpenTwos.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockOpenTwos)
                    {
                        Console.WriteLine($"Block Open Twos: ({move.Row},{move.Col}) for {opponentPlayer}");
                    }
                }
                // return blockOpenTwos.Distinct().ToList();
                availableMoves.AddRange(blockOpenTwos);
            }

            // Create Semi-Open Twos
            if (createSemiOpenTwos.Any())
            {
                if (Debug)
                {
                    foreach (Move move in createSemiOpenTwos)
                    {
                        Console.WriteLine($"Create Semi-Open Twos: ({move.Row},{move.Col}) for {currentPlayer}");
                    }
                }
                // return createSemiOpenTwos.Distinct().ToList();
                availableMoves.AddRange(createSemiOpenTwos);
            }

            // Block Semi-Open Twos
            if (blockSemiOpenTwos.Any())
            {
                if (Debug)
                {
                    foreach (Move move in blockSemiOpenTwos)
                    {
                        Console.WriteLine($"Block Semi-Open Twos: ({move.Row},{move.Col}) for {opponentPlayer}\n");
                    }
                }
                // return blockSemiOpenTwos.Distinct().ToList();
                availableMoves.AddRange(blockSemiOpenTwos);
            }

            if (availableMoves.Any())
            {
                return availableMoves.Distinct().Take(4).ToList();
            }
            /*
             * if we hit this point, it means none of the moves with existing stones 
             * leads to winning.
             * So, we need to find any empty move, store it into List<Move> then return that.
             */

            if (defaultMoves.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("No prioritized moves found. Selecting default moves.");
                    foreach (Move move in defaultMoves)
                    {
                        Console.WriteLine($"Default Move: ({move.Row},{move.Col})");
                    }
                }
                return defaultMoves.Distinct().ToList();
            }

            Console.WriteLine("Red flag");


            // this shouldn't happen
            throw new NotImplementedException();
        }
        private static bool IsBoardAllZeros(int[,] board)
        {
            foreach (int cell in board)
            {
                if (cell != 0)
                    return false;
            }
            return true;
        }

        private static bool IsBoardFull(int[,] board)
        {
            foreach (int cell in board)
            {
                if (cell == 0)
                    return false;
            }
            return true;
        }

        private static Move? FindImmediateWin(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool isWin = rule.IsWinning(board, player);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (isWin)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Immediate win found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }

        private static Move? FindOpenFour(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasOpenFour = HasLineOfLengthX(board, pMove, player, 4, isOpen: true);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasOpenFour)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Open four found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }

            return null;
        }

        private static Move? FindSemiOpenFour(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasSemiOpenFour = HasLineOfLengthX(board, pMove, player, 4, isOpen: false);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasSemiOpenFour)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Semi-Open four found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }

        private static Move? FindOpenThree(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasOpenThree = HasLineOfLengthX(board, pMove, player, 3, isOpen: true);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasOpenThree)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Open three found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }

        private static Move? FindSemiOpenThree(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasSemiOpenThree = HasLineOfLengthX(board, pMove, player, 3, isOpen: false);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasSemiOpenThree)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Semi-Open three found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }

        private static Move? FindOpenTwo(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasOpenTwo = HasLineOfLengthX(board, pMove, player, 2, isOpen: true);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasOpenTwo)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Open two found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }
        private static Move? FindSemiOpenTwo(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasSemiOpenTwo = HasLineOfLengthX(board, pMove, player, 2, isOpen: false);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasSemiOpenTwo)
            {
/*                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Semi-Open two found at ({pMove.Row},{pMove.Col}) for player {player}");
                }*/
                return pMove;
            }
            return null;
        }

        private static bool HasLineOfLengthX(int[,] board, Move pMove, int player, int X, bool isOpen)
        {
            int rowCount = board.GetLength(0);
            int colCount = board.GetLength(1);

            int[,] directions = new int[,]
            {
                { 0, 1 },
                { 1, 0 },
                { 1, 1 },
                { -1, 1 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];

                LineCheckResult check = CheckLine(board, pMove.Row, pMove.Col, player, dr, dc);

                if (check.Count == X)
                {
                    if (check.OpenFront && check.OpenBack && isOpen)
                    {
                        return true;
                    }
                    else if((check.OpenFront || check.OpenBack) && !isOpen)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private class LineCheckResult
        {
            public int Count;
            public bool OpenFront;
            public bool OpenBack;
        }

        private static LineCheckResult CheckLine(int[,] board, int startRow, int startCol, int player, int dr, int dc)
        {
            int rowCount = board.GetLength(0);
            int colCount = board.GetLength(1);

            int count = 1;

            int r = startRow + dr;
            int c = startCol + dc;
            while (IsWithinBounds(r, c, rowCount, colCount) && board[r, c] == player)
            {
                count++;
                r += dr;
                c += dc;
            }
            bool forwardOpen = IsWithinBounds(r, c, rowCount, colCount) && board[r, c] == 0;

            r = startRow - dr;
            c = startCol - dc;
            while (IsWithinBounds(r, c, rowCount, colCount) && board[r, c] == player)
            {
                count++;
                r -= dr;
                c -= dc;
            }
            bool backwardOpen = IsWithinBounds(r, c, rowCount, colCount) && board[r, c] == 0;

            return new LineCheckResult
            {
                Count = count,
                OpenFront = forwardOpen,
                OpenBack = backwardOpen
            };
        }

        private static bool IsWithinBounds(int r, int c, int rowCount, int colCount)
        {
            return r >= 0 && r < rowCount && c >= 0 && c < colCount;
        }
    }
}
