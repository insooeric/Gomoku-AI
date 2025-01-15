namespace Gomoku_AI.AIModels.MCTS
{
    public static class Prioritizer
    {
        public static bool DebugMode = true;
        public static List<Move> PickPrioritizedMove(MCTS_Gomoku state, List<Move> moves)
        {
            // Instead of the old "PickPrioritizedMove", we'll add debug lines 
            // each time we detect a pattern


            // ----------------------------------------------------
            // Priority #0: Instant winning move
            // ----------------------------------------------------
            // 1) Immediate Win?
            List<Move> prioritizedMoves = new List<Move>();
            var winningMove = FindImmediateWin(state, moves, state.CurrentPlayer);
            if (winningMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Detected Pattern: IMMEDIATE WIN for player {state.CurrentPlayer}");
                }
                return new List<Move> { winningMove };
            }

            // 2) Immediate Block Opponent’s Win?
            var blockMove = FindImmediateWin(state, moves, -state.CurrentPlayer);
            if (blockMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Detected Pattern: BLOCK IMMEDIATE WIN for player {state.CurrentPlayer}");
                }
                return new List<Move> { blockMove };
            }

            // ----------------------------------------------------
            // Priority #1: Double 4s
            // ----------------------------------------------------
            var doubleFoursMoves = FindDoubleFours(state, moves, state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (doubleFoursMoves != null)
            {
                if (DebugMode && doubleFoursMoves.Count > 0)
                {
                    Console.WriteLine($"Returning {doubleFoursMoves.Count} Double-Fours moves. (Row, Col) => ...");
                    foreach (var m in doubleFoursMoves)
                        Console.WriteLine($"    Double-Four Move: R={m.Row} C={m.Col}");
                }

                return doubleFoursMoves;
            }

            var blockDoubleFours = FindDoubleFours(state, moves, -state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (blockDoubleFours != null)
            {

                if (DebugMode && blockDoubleFours.Count > 0)
                {
                    Console.WriteLine($"Returning {blockDoubleFours.Count} Double-Fours moves. (Row, Col) => ...");
                    foreach (var m in blockDoubleFours)
                        Console.WriteLine($"    Double-Four Move: R={m.Row} C={m.Col}");
                }

                return blockDoubleFours;
            }
            /*

            // ----------------------------------------------------
            // Priority #2: Double 3s
            // ----------------------------------------------------
            var doubleThreesMove = FindDoubleThrees(state, moves, state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (doubleThreesMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Double 3s (Two Open 3-Lines)");
                }
                return doubleThreesMove;
            }

            var blockDoubleThrees = FindDoubleThrees(state, moves, -state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (blockDoubleThrees != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Double 3s");
                }
                return blockDoubleThrees;
            }

            // ----------------------------------------------------
            // Priority #3: Open 4
            // ----------------------------------------------------
            var openFourMove = FindOpenFour(state, moves, state.CurrentPlayer);
            if (openFourMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Open 4-Line");
                }
                return openFourMove;
            }

            var blockOpenFour = FindOpenFour(state, moves, -state.CurrentPlayer);
            if (blockOpenFour != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Open 4-Line");
                }
                return blockOpenFour;
            }

            // ----------------------------------------------------
            // Priority #4: Double 4s (One Open + One Semi-Open)
            // ----------------------------------------------------
            var mixedDouble4Move = FindMixedDoubleFour(state, moves, state.CurrentPlayer);
            if (mixedDouble4Move != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Double 4s (One Open + One Semi-Open)");
                }
                return mixedDouble4Move;
            }

            var blockMixedDouble4 = FindMixedDoubleFour(state, moves, -state.CurrentPlayer);
            if (blockMixedDouble4 != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Double 4s (One Open + One Semi-Open)");
                }
                return blockMixedDouble4;
            }

            // ----------------------------------------------------
            // Priority #5: Double 3 & 4
            // ----------------------------------------------------
            var double3And4Move = FindDouble3And4(state, moves, state.CurrentPlayer);
            if (double3And4Move != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Double 3 & 4");
                }
                return double3And4Move;
            }

            var blockDouble3And4 = FindDouble3And4(state, moves, -state.CurrentPlayer);
            if (blockDouble3And4 != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Double 3 & 4");
                }
                return blockDouble3And4;
            }

            // ----------------------------------------------------
            // Priority #6: 4 & 4
            // ----------------------------------------------------
            var double4And4Move = Find4And4(state, moves, state.CurrentPlayer);
            if (double4And4Move != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create 4 & 4 (Two Separate 4-Lines)");
                }
                return double4And4Move;
            }

            var block4And4 = Find4And4(state, moves, -state.CurrentPlayer);
            if (block4And4 != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's 4 & 4");
                }
                return block4And4;
            }

            // ----------------------------------------------------
            // Priority #7: Open 3
            // ----------------------------------------------------
            var openThreeMove = FindOpenThree(state, moves, state.CurrentPlayer);
            if (openThreeMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Open 3-Line");
                }
                return openThreeMove;
            }

            var blockOpenThree = FindOpenThree(state, moves, -state.CurrentPlayer);
            if (blockOpenThree != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Open 3-Line");
                }
                return blockOpenThree;
            }

            // ----------------------------------------------------
            // Priority #8: Semi-Open 4
            // ----------------------------------------------------
            var semiOpenFourMove = FindSemiOpenFour(state, moves, state.CurrentPlayer);
            if (semiOpenFourMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Semi-Open 4-Line");
                }
                return semiOpenFourMove;
            }

            var blockSemiOpenFour = FindSemiOpenFour(state, moves, -state.CurrentPlayer);
            if (blockSemiOpenFour != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Semi-Open 4-Line");
                }
                return blockSemiOpenFour;
            }

            // ----------------------------------------------------
            // Priority #9: Semi-Open 3
            // ----------------------------------------------------
            var semiOpenThreeMove = FindSemiOpenThree(state, moves, state.CurrentPlayer);
            if (semiOpenThreeMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Semi-Open 3-Line");
                }
                return semiOpenThreeMove;
            }

            var blockSemiOpenThree = FindSemiOpenThree(state, moves, -state.CurrentPlayer);
            if (blockSemiOpenThree != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Semi-Open 3-Line");
                }
                return blockSemiOpenThree;
            }

            // ----------------------------------------------------
            // Priority #10: Open 2
            // ----------------------------------------------------
            var openTwoMove = FindOpenTwo(state, moves, state.CurrentPlayer);
            if (openTwoMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Open 2-Line");
                }
                return openTwoMove;
            }

            var blockOpenTwo = FindOpenTwo(state, moves, -state.CurrentPlayer);
            if (blockOpenTwo != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Open 2-Line");
                }
                return blockOpenTwo;
            }

            // ----------------------------------------------------
            // Priority #11: Semi-Open 2
            // ----------------------------------------------------
            var semiOpenTwoMove = FindSemiOpenTwo(state, moves, state.CurrentPlayer);
            if (semiOpenTwoMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Create Semi-Open 2-Line");
                }
                return semiOpenTwoMove;
            }

            var blockSemiOpenTwo = FindSemiOpenTwo(state, moves, -state.CurrentPlayer);
            if (blockSemiOpenTwo != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Semi-Open 2-Line");
                }
                return blockSemiOpenTwo;
            }*/

            // ----------------------------------------------------
            // Fallback: near the opponent
            // ----------------------------------------------------


            if (prioritizedMoves.Count == 0)
            {
                List<Move> nearOppMoves = FindMoveNearOpponent(state, moves);
                if (nearOppMoves != null)
                {
                    if (DebugMode)
                    {
                        Console.WriteLine("Fallback: nearOppMove");
                    }
                    return nearOppMoves;
                }
            }

            // near the bottom
            // If we reach here, either we have nearOppMoves or none
            if (DebugMode && prioritizedMoves.Count == 0)
            {
                Console.WriteLine("Final fallback: return the full moves list");
            }
            if (prioritizedMoves.Count == 0)
            {
                return moves;
            }
            return prioritizedMoves;

            // ----------------------------------------------------
            // Fallback: center
            // ----------------------------------------------------
/*            var centerMove = FindCenterMove(state, moves);
            if (centerMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Fallback: centerMove");
                }
                return centerMove;
            }

            // ----------------------------------------------------
            // Final fallback
            // ----------------------------------------------------
            if (moves.Count > 0)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Fallback: first available move in the list.");
                }
                return moves[0];
            }

            // No moves => terminal
            return null;*/
        }

        /*        private class LineCheckResult
                {
                    int Count;      // contiguous stones
                    bool OpenLeft;  // left end open?
                    bool OpenRight; // right end open?

                    public LineCheckResult(int Count, bool OpenLeft, bool OpenRight)
                    {
                        this.Count = Count;
                        this.OpenLeft = false;
                        this.OpenRight = false;
                    }

                    public int GetCount()
                    {
                        return Count;
                    }

                    public bool GetOpenLeft()
                    {
                        return OpenLeft;
                    }

                    public bool GetOpenRight()
                    {
                        return OpenRight;
                    }
                }

                private static LineCheckResult CheckLine(int[,] board, int row, int col, int player, int dr, int dc)
                {

                    int rowCount = board.GetLength(0);
                    int colCount = board.GetLength(1);

                    // count forward
                    int forwardCount = 0;
                    int r = row, c = col;
                    while (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == player)
                    {
                        forwardCount++;
                        r += dr;
                        c += dc;
                    }
                    bool forwardOpen = (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == 0);

                    // count backward
                    int backwardCount = 0;
                    r = row - dr;
                    c = col - dc;
                    while (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == player)
                    {
                        backwardCount++;
                        r -= dr;
                        c -= dc;
                    }
                    bool backwardOpen = (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == 0);

                    LineCheckResult result = new LineCheckResult(
                        forwardCount+backwardCount, 
                        forwardOpen, 
                        backwardOpen
                        );
                    // Summation
        *//*            result.Count = forwardCount + backwardCount;
                    result.OpenLeft = forwardOpen;
                    result.OpenRight = backwardOpen;*//*
                    return result;
                }*/


        private static (int openCount, int semiOpenCount, int totalCount)
            CountLinesOfLengthX(int[,] board, int row, int col, int player, int X)
        {
            int openLines = 0;
            int semiOpenLines = 0;
            int totalLines = 0;

            // The 4 principal directions to check
            int[,] directions = new int[,]
            {
        { 0, 1 },  // horizontal
        { 1, 0 },  // vertical
        { 1, 1 },  // diagonal down-right
        { -1, 1 }  // diagonal up-right
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];

                var check = CheckLine(board, row, col, player, dr, dc);

                // If exactly X contiguous stones (including the new stone)
                if (check.Count >= X)
                {
                    totalLines++;
                    bool bothOpen = check.OpenLeft && check.OpenRight;
                    bool oneOpen = check.OpenLeft ^ check.OpenRight; // XOR

                    if (bothOpen) openLines++;
                    else if (oneOpen) semiOpenLines++;
                }
            }

            return (openLines, semiOpenLines, totalLines);
        }

        // Helper struct/class for CheckLine results
        private class LineCheckResult
        {
            public int Count;
            public bool OpenLeft;
            public bool OpenRight;
        }

        /// <summary>
        /// Counts how many contiguous stones for `player` along direction (dr, dc),
        /// plus checks if either end is open (empty).
        /// </summary>
        private static LineCheckResult CheckLine(int[,] board, int row, int col, int player, int dr, int dc)
        {
            int rowCount = board.GetLength(0);
            int colCount = board.GetLength(1);

            // 1) Count forward in (dr, dc)
            int forwardCount = 0;
            int r = row, c = col;
            while (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == player)
            {
                forwardCount++;
                r += dr;
                c += dc;
            }
            bool forwardOpen = (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == 0);

            // 2) Count backward in the opposite direction
            int backwardCount = 0;
            r = row - dr;
            c = col - dc;
            while (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == player)
            {
                backwardCount++;
                r -= dr;
                c -= dc;
            }
            bool backwardOpen = (r >= 0 && r < rowCount && c >= 0 && c < colCount && board[r, c] == 0);

            // Summation
            return new LineCheckResult
            {
                Count = forwardCount + backwardCount,
                OpenLeft = forwardOpen,
                OpenRight = backwardOpen
            };
        }


        public static Move? FindImmediateWin(MCTS_Gomoku state, List<Move> moves, int player)
        {
            foreach (var move in moves)
            {
                state.Board[move.Row, move.Col] = player; // simulate
                bool isWin = state._rule.IsWinning(state.Board, player);
                state.Board[move.Row, move.Col] = 0;      // undo

                if (isWin)
                {
                    if (DebugMode)
                    {
                        Console.WriteLine($"DEBUG: Found immediate win at Row={move.Row}, Col={move.Col} for player={player}");
                    }
                    return move;
                }
            }
            return null;
        }

        public static List<Move> FindDoubleFours(
            MCTS_Gomoku state,
            List<Move> moves,
            int player,
            bool openOnly = true,
            int requiredCount = 2)
        {
            // We'll gather *all* moves that yield double fours
            List<Move> result = new List<Move>();

            foreach (var move in moves)
            {
                // Simulate placing the stone
                state.Board[move.Row, move.Col] = player;

                // Count how many lines of (at least) length 4
                var (open4, semi4, total4) = CountLinesOfLengthX(state.Board, move.Row, move.Col, player, 4);

                // Undo the stone
                state.Board[move.Row, move.Col] = 0;

                // If openOnly == true, we only consider open4
                // If openOnly == false, we consider both open4 + semi4
                int relevantCount = openOnly ? open4 : (open4 + semi4);

                // If we have at least `requiredCount` (e.g., 2) lines of 4,
                // it's a "double four" (or more).
                if (relevantCount >= requiredCount)
                {
/*                    if (DebugMode)
                    {
                        Console.WriteLine(
                            $"[DEBUG] DoubleFour: Move=({move.Row},{move.Col}) => " +
                            $"open4={open4}, semi4={semi4}, total4={total4}");
                    }*/
                    result.Add(move);
                }
            }

            return result;
        }


        /*        private static Move? FindDoubleFours(MCTS_Gomoku state, List<Move> moves, int player, bool openOnly, int requiredCount)
                {
                    foreach (var move in moves)
                    {
                        state.Board[move.Row, move.Col] = player;
                        var (open4, semi4, total4) = CountLinesOfLengthX(state.Board, move.Row, move.Col, player, 4);
                        state.Board[move.Row, move.Col] = 0;

                        int relevantCount = openOnly ? open4 : (open4 + semi4);
                        if (relevantCount >= requiredCount)
                        {
                            if (DebugMode)
                            {
                                Console.WriteLine($"DEBUG: Move Row={move.Row}, Col={move.Col} creates Double 4s: open={open4}, semi={semi4}");
                            }
                            return move;
                        }
                    }
                    return null;
                }*/

        /*        private static Move? FindDoubleThrees(MCTS_Gomoku state, List<Move> moves, int player, bool openOnly, int requiredCount)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open3, semi3, total3) = CountLinesOfLengthX(board, move.Row, move.Col, player, 3);

                        int relevantCount = openOnly ? open3 : (open3 + semi3);

                        if (relevantCount >= requiredCount)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindOpenFour(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open4, semi4, total4) = CountLinesOfLengthX(board, move.Row, move.Col, player, 4);

                        // If we have at least 1 open 4
                        if (open4 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }

                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindMixedDoubleFour(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open4, semi4, total4) = CountLinesOfLengthX(board, move.Row, move.Col, player, 4);

                        // One open + one semi-open => open4>=1 && semi4>=1
                        if (open4 >= 1 && semi4 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindDouble3And4(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;

                        var (open3, semi3, total3) = CountLinesOfLengthX(board, move.Row, move.Col, player, 3);
                        var (open4, semi4, total4) = CountLinesOfLengthX(board, move.Row, move.Col, player, 4);

                        // 1 open-3 + 1 open-4
                        if (open3 >= 1 && open4 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? Find4And4(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open4, semi4, total4) = CountLinesOfLengthX(board, move.Row, move.Col, player, 4);

                        // Need 2 or more total lines of length 4
                        if (total4 >= 2)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindOpenThree(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open3, semi3, total3) = CountLinesOfLengthX(board, move.Row, move.Col, player, 3);

                        if (open3 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindSemiOpenFour(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open4, semi4, total4) = CountLinesOfLengthX(board, move.Row, move.Col, player, 4);

                        if (semi4 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindSemiOpenThree(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open3, semi3, total3) = CountLinesOfLengthX(board, move.Row, move.Col, player, 3);

                        if (semi3 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindOpenTwo(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open2, semi2, total2) = CountLinesOfLengthX(board, move.Row, move.Col, player, 2);

                        if (open2 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }

                private static Move? FindSemiOpenTwo(MCTS_Gomoku state, List<Move> moves, int player)
                {
                    int[,] board = state.Board;
                    foreach (var move in moves)
                    {
                        board[move.Row, move.Col] = player;
                        var (open2, semi2, total2) = CountLinesOfLengthX(board, move.Row, move.Col, player, 2);

                        if (semi2 >= 1)
                        {
                            board[move.Row, move.Col] = 0;
                            return move;
                        }
                        board[move.Row, move.Col] = 0;
                    }
                    return null;
                }*/

        private static List<Move> FindMoveNearOpponent(MCTS_Gomoku state, List<Move> moves)
        {
            if (moves.Count == 0)
                throw new InvalidOperationException("No moves available (game over?).");

            int opponent = -state.CurrentPlayer;

            // Gather opponent positions
            var opponentPositions = new List<Move>();
            for (int r = 0; r < state.BoardRow; r++)
            {
                for (int c = 0; c < state.BoardCol; c++)
                {
                    if (state.Board[r, c] == opponent)
                    {
                        opponentPositions.Add(new Move(r,c));
                    }
                }
            }

            // If opponent has no stones, fallback to center
            if (opponentPositions.Count == 0)
            {
                opponentPositions.Add(new Move(state.BoardRow / 2, state.BoardCol / 2));
                return opponentPositions;
            }

            // Pick the single closest move to any opponent stone
            List<Move> bestMoves = moves;
/*            double bestDist = double.MaxValue;

            foreach (var move in moves)
            {
                foreach (var (oppRow, oppCol) in opponentPositions)
                {
                    double dist = Math.Sqrt(Math.Pow(move.Row - oppRow, 2)
                                          + Math.Pow(move.Col - oppCol, 2));
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestMove = move;
                    }
                }
            }*/
            return bestMoves;
        }

/*        private static Move FindCenterMove(MCTS_Gomoku state, List<Move> moves)
        {
            if (moves.Count == 0)
                throw new InvalidOperationException("No moves available (game over?).");

            int centerRow = state.BoardRow / 2;
            int centerCol = state.BoardCol / 2;

            Move bestMove = moves[0];
            double bestDist = double.MaxValue;

            foreach (var move in moves)
            {
                double dist = Math.Sqrt(Math.Pow(move.Row - centerRow, 2)
                                      + Math.Pow(move.Col - centerCol, 2));
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestMove = move;
                }
            }
            return bestMove;
        }*/
    }
}
