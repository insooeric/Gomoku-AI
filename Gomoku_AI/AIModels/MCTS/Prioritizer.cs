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

            // Categorize move lists based on patterns
/*            List<Move> immediateWins = new List<Move>();
            List<Move> blockImmediateWins = new List<Move>();*/

            List<Move> createOpenFours = new List<Move>();
            List<Move> blockOpenFours = new List<Move>();

            List<Move> createOpenThrees = new List<Move>();
            List<Move> blockOpenThrees = new List<Move>();

            List<Move> createOpenTwos = new List<Move>();
            List<Move> blockOpenTwos = new List<Move>();

            int currentPlayer = CurrentPlayer.Get(board);
            int opponentPlayer = -currentPlayer;

            if (Debug)
            {
                Console.WriteLine("Prioritizing moves based on current strategy mode.");
            }

            // Iterate through all cells to categorize potential moves
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == 0)
                    {
                        Move currentMove = new Move(row, col);
                        if (Debug)
                        {
                            Console.WriteLine($"\nEvaluating Move: ({currentMove.Row},{currentMove.Col})");
                        }

                        // Immediate Win for AI
                        Move? winningMove = FindImmediateWin(board, rule, currentMove, currentPlayer);
                        if (winningMove != null)
                        {
                            //immediateWins.Add(winningMove);
                            return new List<Move>() { winningMove };
                        }

                        // Block Opponent's Immediate Win
                        Move? blockMove = FindImmediateWin(board, rule, currentMove, opponentPlayer);
                        if (blockMove != null)
                        {
                            //blockImmediateWins.Add(blockMove);
                            return new List<Move>() { blockMove };
                        }

                        // Create Four for AI
                        Move? openFourMove = FindFour(board, rule, currentMove, currentPlayer);
                        if (openFourMove != null)
                        {
                            createOpenFours.Add(openFourMove);
                        }

                        // Block Opponent's Four
                        Move? blockOpenFourMove = FindFour(board, rule, currentMove, opponentPlayer);
                        if (blockOpenFourMove != null)
                        {
                            blockOpenFours.Add(blockOpenFourMove);
                        }

                        // Create Three for AI
                        Move? openThreeMove = FindThree(board, rule, currentMove, currentPlayer);
                        if (openThreeMove != null)
                        {
                            createOpenThrees.Add(openThreeMove);
                        }

                        // Block Opponent's Three
                        Move? blockOpenThreeMove = FindThree(board, rule, currentMove, opponentPlayer);
                        if (blockOpenThreeMove != null)
                        {
                            blockOpenThrees.Add(blockOpenThreeMove);
                        }

                        // Create Two for AI
                        Move? openTwoMove = FindTwo(board, rule, currentMove, currentPlayer);
                        if (openTwoMove != null)
                        {
                            createOpenTwos.Add(openTwoMove);
                        }

                        // Block Opponent's Two
                        Move? blockTwoMove = FindTwo(board, rule, currentMove, opponentPlayer);
                        if (blockTwoMove != null)
                        {
                            blockOpenTwos.Add(blockTwoMove);
                        }
                    }
                }
            }

            List<Move> availableMoves = new List<Move>();

/*            // 1. Immediate Wins
            if (immediateWins.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Attacking Mode: Immediate Wins");
                    foreach (Move move in immediateWins)
                    {
                        Console.WriteLine($"Immediate Win: ({move.Row},{move.Col})");
                    }
                }
                return immediateWins.Distinct().Take(2).ToList();
            }

            // 1. Block Immediate Wins
            if (blockImmediateWins.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Blocking Immediate Wins");
                    foreach (Move move in blockImmediateWins)
                    {
                        Console.WriteLine($"Block Immediate Win: ({move.Row},{move.Col})");
                    }
                }
                return blockImmediateWins.Distinct().Take(2).ToList();
            }*/

            // 2. Create Open Fours (to maintain offensive pressure while defending)
            if (createOpenFours.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Creating Open Fours");
                    foreach (Move move in createOpenFours)
                    {
                        Console.WriteLine($"Create Open Four: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(createOpenFours.Distinct());
            }

            // 3. Block Open Fours
            if (blockOpenFours.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Blocking Open Fours");
                    foreach (Move move in blockOpenFours)
                    {
                        Console.WriteLine($"Block Open Four: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(blockOpenFours.Distinct());
            }

            // 4. Create Open Threes
            if (createOpenThrees.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Creating Open Threes");
                    foreach (Move move in createOpenThrees)
                    {
                        Console.WriteLine($"Create Open Three: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(createOpenThrees.Distinct());
            }

            // 5. Block Open Threes
            if (blockOpenThrees.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Blocking Open Threes");
                    foreach (Move move in blockOpenThrees)
                    {
                        Console.WriteLine($"Block Open Three: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(blockOpenThrees.Distinct());
            }

            // 6. Create Open Twos
            if (createOpenTwos.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Creating Open Twos");
                    foreach (Move move in createOpenTwos)
                    {
                        Console.WriteLine($"Create Open Two: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(createOpenTwos.Distinct());
            }

            // 7. Block Open Twos
            if (blockOpenTwos.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Defending Mode: Blocking Open Twos");
                    foreach (Move move in blockOpenTwos)
                    {
                        Console.WriteLine($"Block Open Two: ({move.Row},{move.Col})");
                    }
                }
                availableMoves.AddRange(blockOpenTwos.Distinct());
            }

            // If no prioritized moves found based on current mode, default to all available moves
            if (availableMoves.Any())
            {
                if (Debug)
                {
                    Console.WriteLine("Returning prioritized available moves based on strategy mode.");
                }
                return availableMoves.Distinct().Take(10).ToList(); // Limit to top 10 moves
            }

            // return centermove as default
            return new List<Move>() { new Move(board.GetLength(0)/2, board.GetLength(1)/2) };
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

        private static Move? FindImmediateWin(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool isWin = rule.IsWinning(board, player);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (isWin)
            {
                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Immediate win found at ({pMove.Row},{pMove.Col}) for player {player}");
                }
                return pMove;
            }
            return null;
        }

        private static Move? FindFour(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasFour = HasLineOfLengthX(board, pMove, player, 4);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasFour)
            {
                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Four in a row found at ({pMove.Row},{pMove.Col}) for player {player}");
                }
                return pMove;
            }

            return null;
        }

        private static Move? FindThree(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasThree = HasLineOfLengthX(board, pMove, player, 3);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasThree)
            {
                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Three in a row found at ({pMove.Row},{pMove.Col}) for player {player}");
                }
                return pMove;
            }

            return null;
        }

        private static Move? FindTwo(int[,] board, IRule rule, Move pMove, int player)
        {
            board[pMove.Row, pMove.Col] = player; // Simulate move
            bool hasTwo = HasLineOfLengthX(board, pMove, player, 2);
            board[pMove.Row, pMove.Col] = 0; // Undo simulation

            if (hasTwo)
            {
                if (Debug)
                {
                    Console.WriteLine($"DEBUG: Two in a row found at ({pMove.Row},{pMove.Col}) for player {player}");
                }
                return pMove;
            }

            return null;
        }

        private static bool HasLineOfLengthX(int[,] board, Move pMove, int player, int X)
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

                if (check.Count >= X)
                {
                    if (check.OpenLeft || check.OpenRight)
                        return true;
                }
            }

            return false;
        }

        private class LineCheckResult
        {
            public int Count;
            public bool OpenLeft;
            public bool OpenRight;
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
                OpenLeft = backwardOpen,
                OpenRight = forwardOpen
            };
        }

        private static bool IsWithinBounds(int r, int c, int rowCount, int colCount)
        {
            return r >= 0 && r < rowCount && c >= 0 && c < colCount;
        }
    }
}
