﻿using System.Xml.Linq;

namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS_Logic
    {
        private readonly int Iterations;
        private readonly double ExplorationConstant;
        private static readonly Random rand = new Random();
        private bool DebugMode = true;
        private const int PriorityDepthDefault = 10;

        public MCTS_Logic(int iterations = 3000, double explorationConstant = 1.414213562373095)
        {
            Iterations = iterations;
            ExplorationConstant = explorationConstant;
        }

        public Move Search(MCTS_Gomoku rootState)
        {
            // Create the root node
            MCTS_Node rootNode = new MCTS_Node(rootState);

            // Main MCTS loop
            for (int i = 0; i < Iterations; i++)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Iteration: {i}");
                }
                MCTS_Node node = rootNode;
                MCTS_Gomoku state = rootState.Clone();

                // 1) Selection
                while (!node.IsTerminal() && node.IsFullyExpanded())
                {
                    node = node.SelectChild(ExplorationConstant);
                    if (node.Move == null)
                        break; // Should rarely happen if your tree logic is correct
                    Console.WriteLine($"Applying Move Row={node.Move?.Row}, Col={node.Move?.Col}:");
                    state.ApplyMove(node.Move);
                }

                // 2) Expansion
                if (!node.IsTerminal())
                {
                    MCTS_Node child = node.Expand();
                    if (child.Move == null)
                        continue;
                    state.ApplyMove(child.Move);
                    node = child;
                }

                // 3) Simulation (heuristic or random)
                int result = Simulate(state, maxDepth: 60, priorityDepth: PriorityDepthDefault);

                if (DebugMode)
                {
                    Console.WriteLine($"Simulated Move: Row={node.Move?.Row} Col={node.Move?.Col}");
                    Console.WriteLine($"Result after Simulation: {result}");
                    Console.WriteLine("--------------------------------------------");
                }


                // 4) Backpropagation
                while (node != null && node.Parent != null)
                {
                    node.Update(result);
                    node = node.Parent;
                }
            }

            // ------------------------------------------------
            // Final Move Selection: never return null
            // ------------------------------------------------

            // 1) Prefer the child with the highest visits
            MCTS_Node? bestChild = null;
            int maxVisits = -1;
            foreach (var c in rootNode.Children)
            {
                if (c.Visits > maxVisits)
                {
                    bestChild = c;
                    maxVisits = c.Visits;
                }
            }

            if (bestChild != null && bestChild.Move != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Best Move: Row={bestChild.Move.Row}, Col={bestChild.Move.Col}");
                }
                return bestChild.Move;
            }
            else if (rootNode.UntriedMoves.Count > 0)
            {
                Move fallback = rootNode.UntriedMoves[0];
                if (DebugMode)
                {
                    Console.WriteLine($"Best Move (fallback untried): Row={fallback.Row}, Col={fallback.Col}");
                }
                return fallback;
            }

            throw new InvalidOperationException("No valid moves remain. The board is likely terminal.");
        }

        /*        private int SimulateHeuristic(MCTS_Gomoku state)
                {
                    int moveLimit = 60;  // limit the playout

                    while (!state.IsGameOver() && moveLimit-- > 0)
                    {
                        var moves = state.GetPossibleMoves();
                        if (moves.Count == 0) break;

                        Move chosen = FindImmediateWinOrBlock(state, moves);
                        if (chosen == null)
                        {
                            chosen = moves[rand.Next(moves.Count)];
                        }

                        state.ApplyMove(chosen);
                    }

                    return state.CheckWinner();
                }*/

        private int Simulate(MCTS_Gomoku state, int maxDepth = 60, int priorityDepth = 6)
        {
            int steps = 0;
            while (!state.IsGameOver() && maxDepth-- > 0)
            {
                var moves = state.GetPossibleMoves();
                if (moves.Count == 0)
                    break;

                // If we always have at least one move from PickPrioritizedMove, just pick it
                var prioritized = Prioritizer.PickPrioritizedMove(state, moves);
                Move chosen = prioritized[0];  // no fallback needed if we're guaranteed at least one

                state.ApplyMove(chosen);
                steps++;
            }
            return state.CheckWinner();
        }





        /*        private Move FindImmediateWinOrBlock(MCTS_Gomoku state, List<Move> moves)
                {
                    int player = state.CurrentPlayer;

                    // 1) immediate win
                    Move? win = FindImmediateWin(state, moves, player);
                    if (win != null) return win;

                    // 2) immediate block
                    int opp = -player;
                    Move? block = FindImmediateWin(state, moves, opp);
                    if (block != null) return block;

                    // else no immediate threat => return null so we pick random
                    return null!;
                }*/

        // ----------------------------------------------------------------------
        // Priority-based move selection with debug messages
        // ----------------------------------------------------------------------
        /*private Move? PickPrioritizedMove(MCTS_Gomoku state, List<Move> moves)
        {
            // Instead of the old "PickPrioritizedMove", we'll add debug lines 
            // each time we detect a pattern

            // 1) Immediate Win?
            var winningMove = FindImmediateWin(state, moves, state.CurrentPlayer);
            if (winningMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Detected Pattern: IMMEDIATE WIN for player {state.CurrentPlayer}");
                }
                return winningMove;
            }

            // 2) Immediate Block Opponent’s Win?
            var blockMove = FindImmediateWin(state, moves, -state.CurrentPlayer);
            if (blockMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine($"Detected Pattern: BLOCK IMMEDIATE WIN for player {state.CurrentPlayer}");
                }
                return blockMove;
            }

            // ----------------------------------------------------
            // Priority #1: Double 4s
            // ----------------------------------------------------
            var doubleFoursMove = FindDoubleFours(state, moves, state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (doubleFoursMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: Double 4s (Two Open 4-Lines)");
                }
                return doubleFoursMove;
            }

            var blockDoubleFours = FindDoubleFours(state, moves, -state.CurrentPlayer, openOnly: true, requiredCount: 2);
            if (blockDoubleFours != null)
            {

                if (DebugMode)
                {
                    Console.WriteLine("Detected Pattern: BLOCK Opponent's Double 4s");
                }
                return blockDoubleFours;
            }

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
            }

            // ----------------------------------------------------
            // Fallback: near the opponent
            // ----------------------------------------------------
            var nearOppMove = FindMoveNearOpponent(state, moves);
            if (nearOppMove != null)
            {
                if (DebugMode)
                {
                    Console.WriteLine("Fallback: nearOppMove");
                }
                return nearOppMove;
            }

            // ----------------------------------------------------
            // Fallback: center
            // ----------------------------------------------------
            var centerMove = FindCenterMove(state, moves);
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
            return null;
        }

        private class LineCheckResult
        {
            public int Count;      // contiguous stones
            public bool OpenLeft;  // left end open?
            public bool OpenRight; // right end open?
        }

        private LineCheckResult CheckLine(int[,] board, int row, int col, int player, int dr, int dc)
        {
            var result = new LineCheckResult();

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

            // Summation
            result.Count = forwardCount + backwardCount;
            result.OpenLeft = forwardOpen;
            result.OpenRight = backwardOpen;
            return result;
        }

        private (int openCount, int semiOpenCount, int totalCount) CountLinesOfLengthX(int[,] board, int row, int col, int player, int X)
        {
            int openLines = 0;
            int semiOpenLines = 0;
            int totalLines = 0;

            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { -1, 1 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];
                var check = CheckLine(board, row, col, player, dr, dc);

                // If we want EXACT X, use == X. If we want "X or more," use >= X.
                if (check.Count == X)
                {
                    totalLines++;
                    bool bothOpen = check.OpenLeft && check.OpenRight;
                    bool oneOpen = check.OpenLeft ^ check.OpenRight; // XOR

                    if (bothOpen) openLines++;
                    else if (oneOpen) semiOpenLines++;
                }
                // If you prefer "X or more," you could do:
                // else if (check.Count >= X) { ... }
            }

            return (openLines, semiOpenLines, totalLines);
        }

        private Move? FindImmediateWin(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindDoubleFours(MCTS_Gomoku state, List<Move> moves, int player, bool openOnly, int requiredCount)
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
        }

        private Move? FindDoubleThrees(MCTS_Gomoku state, List<Move> moves, int player, bool openOnly, int requiredCount)
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

        private Move? FindOpenFour(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindMixedDoubleFour(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindDouble3And4(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? Find4And4(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindOpenThree(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindSemiOpenFour(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindSemiOpenThree(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindOpenTwo(MCTS_Gomoku state, List<Move> moves, int player)
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

        private Move? FindSemiOpenTwo(MCTS_Gomoku state, List<Move> moves, int player)
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
        }

        private Move FindMoveNearOpponent(MCTS_Gomoku state, List<Move> moves)
        {
            if (moves.Count == 0)
                throw new InvalidOperationException("No moves available (game over?).");

            int opponent = -state.CurrentPlayer;

            // Gather opponent positions
            var opponentPositions = new List<(int row, int col)>();
            for (int r = 0; r < state.BoardRow; r++)
            {
                for (int c = 0; c < state.BoardCol; c++)
                {
                    if (state.Board[r, c] == opponent)
                    {
                        opponentPositions.Add((r, c));
                    }
                }
            }

            // If opponent has no stones, fallback to center
            if (opponentPositions.Count == 0)
                return FindCenterMove(state, moves);

            // Pick the single closest move to any opponent stone
            Move bestMove = moves[0];
            double bestDist = double.MaxValue;

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
            }
            return bestMove;
        }

        private Move FindCenterMove(MCTS_Gomoku state, List<Move> moves)
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
