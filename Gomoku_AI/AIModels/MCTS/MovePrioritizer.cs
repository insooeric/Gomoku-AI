namespace Gomoku_AI.AIModels.MCTS
{
    public class MovePrioritizer
    {
        private readonly Board _board;
        private readonly Cell _currentPlayer;
        private readonly Cell _opponent;

        public MovePrioritizer(Board board, Cell currentPlayer)
        {
            _board = board;
            _currentPlayer = currentPlayer;
            _opponent = currentPlayer == Cell.Black ? Cell.White : Cell.Black;
        }

        public List<Move> GetPrioritizedMoves()
        {
            var prioritizedMoves = new List<Move>();

            // Priority 1: My Move - Create a 5-Line (Win)
            var myWinningMoves = FindMovesCreatingPattern("Five", _currentPlayer);
            if (myWinningMoves.Any())
            {
                Console.WriteLine("Detecting My Open Five");
                prioritizedMoves.AddRange(myWinningMoves);
                return prioritizedMoves.OrderBy(m => 1).ToList(); // Highest priority
            }

            // Priority 2: Opponent's Move - Can Create a 5-Line (Block)
            var opponentWinningMoves = FindMovesCreatingPattern("Five", _opponent);
            if (opponentWinningMoves.Any())
            {
                Console.WriteLine("Detecting Opponent's Open Five");
                prioritizedMoves.AddRange(opponentWinningMoves);
            }

            // Priority 3: My Move - Create Double 4s (Two Open 4-Lines)
            var myDoubleFourOpen = FindMovesCreatingPattern("DoubleFourOpen", _currentPlayer);
            if (myDoubleFourOpen.Any())
            {
                Console.WriteLine("Detecting My Double Open Four");
                prioritizedMoves.AddRange(myDoubleFourOpen);
            }

            // Priority 4: Opponent's Move - Can Create Double 4s (Block)
            var opponentDoubleFourOpen = FindMovesCreatingPattern("DoubleFourOpen", _opponent);
            if (opponentDoubleFourOpen.Any())
            {
                Console.WriteLine("Detecting Opponent's Double Open Four");
                prioritizedMoves.AddRange(opponentDoubleFourOpen);
            }

            // Priority 5: My Move - Create Double 3s (Two Open 3-Lines)
            var myDoubleThreeOpen = FindMovesCreatingPattern("DoubleThreeOpen", _currentPlayer);
            if (myDoubleThreeOpen.Any())
            {
                Console.WriteLine("Detecting My Double Open Three");
                prioritizedMoves.AddRange(myDoubleThreeOpen);
            }

            // Priority 6: Opponent's Move - Can Create Double 3s (Block)
            var opponentDoubleThreeOpen = FindMovesCreatingPattern("DoubleThreeOpen", _opponent);
            if (opponentDoubleThreeOpen.Any())
            {
                Console.WriteLine("Detecting Opponent's Double Open Three");
                prioritizedMoves.AddRange(opponentDoubleThreeOpen);
            }

            // Priority 7: My Move - Create an Open 4-Line
            var myOpenFour = FindMovesCreatingPattern("OpenFour", _currentPlayer);
            if (myOpenFour.Any())
            {
                Console.WriteLine("Detecting My Open Four");
                prioritizedMoves.AddRange(myOpenFour);
            }

            // Priority 8: Opponent's Move - Can Create an Open 4-Line (Block)
            var opponentOpenFour = FindMovesCreatingPattern("OpenFour", _opponent);
            if (opponentOpenFour.Any())
            {
                Console.WriteLine("Detecting Opponent's Open Four");
                prioritizedMoves.AddRange(opponentOpenFour);
            }

            // Priority 9: My Move - Create Double 4s (One Open and One Semi-Open 4-Line)
            var myDoubleFourMixed = FindMovesCreatingPattern("DoubleFourMixed", _currentPlayer);
            if (myDoubleFourMixed.Any())
            {
                Console.WriteLine("Detecting My Mixed Double Four");
                prioritizedMoves.AddRange(myDoubleFourMixed);
            }

            // Priority 10: Opponent's Move - Can Create Double 4s (One Open and One Semi-Open 4-Line) (Block)
            var opponentDoubleFourMixed = FindMovesCreatingPattern("DoubleFourMixed", _opponent);
            if (opponentDoubleFourMixed.Any())
            {
                Console.WriteLine("Detecting Opponent's Mixed Double Four");
                prioritizedMoves.AddRange(opponentDoubleFourMixed);
            }

            // Priority 11: My Move - Create Double 3 and 4 (One Open 3-Line and One Open 4-Line)
            var myDoubleThreeAndFour = FindMovesCreatingPattern("DoubleThreeAndFour", _currentPlayer);
            if (myDoubleThreeAndFour.Any())
            {
                Console.WriteLine("Detecting My Double Three and Four");
                prioritizedMoves.AddRange(myDoubleThreeAndFour);
            }

            // Priority 12: Opponent's Move - Can Create Double 3 and 4 (Block)
            var opponentDoubleThreeAndFour = FindMovesCreatingPattern("DoubleThreeAndFour", _opponent);
            if (opponentDoubleThreeAndFour.Any())
            {
                Console.WriteLine("Detecting Opponent's Double Three and Four");
                prioritizedMoves.AddRange(opponentDoubleThreeAndFour);
            }

            // Priority 13: My Move - Create 4 & 4 (Two Separate 4-Lines, Open or Semi-Open)
            var mySeparateDoubleFour = FindMovesCreatingPattern("SeparateDoubleFour", _currentPlayer);
            if (mySeparateDoubleFour.Any())
            {
                Console.WriteLine("Detecting My Separate Double Four");
                prioritizedMoves.AddRange(mySeparateDoubleFour);
            }

            // Priority 14: Opponent's Move - Can Create 4 & 4 (Block)
            var opponentSeparateDoubleFour = FindMovesCreatingPattern("SeparateDoubleFour", _opponent);
            if (opponentSeparateDoubleFour.Any())
            {
                Console.WriteLine("Detecting Opponent's Separate Double Four");
                prioritizedMoves.AddRange(opponentSeparateDoubleFour);
            }

            // Priority 15: My Move - Create an Open 3-Line
            var myOpenThree = FindMovesCreatingPattern("OpenThree", _currentPlayer);
            if (myOpenThree.Any())
            {
                Console.WriteLine("Detecting My Open Three");
                prioritizedMoves.AddRange(myOpenThree);
            }

            // Priority 16: Opponent's Move - Can Create an Open 3-Line (Block)
            var opponentOpenThree = FindMovesCreatingPattern("OpenThree", _opponent);
            if (opponentOpenThree.Any())
            {
                Console.WriteLine("Detecting Opponent's Open Three");
                prioritizedMoves.AddRange(opponentOpenThree);
            }

            // Priority 17: My Move - Create a 4-Line with One Side Closed (Semi-Open 4-Line)
            var mySemiOpenFour = FindMovesCreatingPattern("SemiOpenFour", _currentPlayer);
            if (mySemiOpenFour.Any())
            {
                Console.WriteLine("Detecting My Semi-Open Four");
                prioritizedMoves.AddRange(mySemiOpenFour);
            }

            // Priority 18: Opponent's Move - Can Create a 4-Line with One Side Closed (Block)
            var opponentSemiOpenFour = FindMovesCreatingPattern("SemiOpenFour", _opponent);
            if (opponentSemiOpenFour.Any())
            {
                Console.WriteLine("Detecting Opponent's Semi-Open Four");
                prioritizedMoves.AddRange(opponentSemiOpenFour);
            }

            // Priority 19: My Move - Create a 3-Line with One Side Closed (Semi-Open 3-Line)
            var mySemiOpenThree = FindMovesCreatingPattern("SemiOpenThree", _currentPlayer);
            if (mySemiOpenThree.Any())
            {
                Console.WriteLine("Detecting My Semi-Open Three");
                prioritizedMoves.AddRange(mySemiOpenThree);
            }

            // Priority 20: Opponent's Move - Can Create a 3-Line with One Side Closed (Block)
            var opponentSemiOpenThree = FindMovesCreatingPattern("SemiOpenThree", _opponent);
            if (opponentSemiOpenThree.Any())
            {
                Console.WriteLine("Detecting Opponent's Semi-Open Three");
                prioritizedMoves.AddRange(opponentSemiOpenThree);
            }

            // Priority 21: My Move - Create an Open 2-Line
            var myOpenTwo = FindMovesCreatingPattern("OpenTwo", _currentPlayer);
            if (myOpenTwo.Any())
            {
                Console.WriteLine("Detecting My Open Two");
                prioritizedMoves.AddRange(myOpenTwo);
            }

            // Priority 22: Opponent's Move - Can Create an Open 2-Line (Block)
            var opponentOpenTwo = FindMovesCreatingPattern("OpenTwo", _opponent);
            if (opponentOpenTwo.Any())
            {
                Console.WriteLine("Detecting Opponent's Open Two");
                prioritizedMoves.AddRange(opponentOpenTwo);
            }

            // Priority 23: My Move - Create a 2-Line with One Side Closed (Semi-Open 2-Line)
            var mySemiOpenTwo = FindMovesCreatingPattern("SemiOpenTwo", _currentPlayer);
            if (mySemiOpenTwo.Any())
            {
                Console.WriteLine("Detecting My Semi-Open Two");
                prioritizedMoves.AddRange(mySemiOpenTwo);
            }

            // Priority 24: Opponent's Move - Can Create a 2-Line with One Side Closed (Block)
            var opponentSemiOpenTwo = FindMovesCreatingPattern("SemiOpenTwo", _opponent);
            if (opponentSemiOpenTwo.Any())
            {
                Console.WriteLine("Detecting Opponent's Semi-Open Two");
                prioritizedMoves.AddRange(opponentSemiOpenTwo);
            }

            return prioritizedMoves;
        }

        // it should always return move?
        private List<Move> FindMovesCreatingPattern(string pattern, Cell player)
        {
            var moves = new List<Move>();
            var detector = new PatternDetector(_board, player);

            for (int x = 0; x < _board.Size; x++)
            {
                for (int y = 0; y < _board.Size; y++)
                {
                    if (_board.IsEmpty(x, y))
                    {
                        // Temporarily place the stone
                        _board.PlaceStone(x, y, player);

                        bool matches = pattern switch
                        {
                            "Five" => detector.CreatesFive(x, y),
                            "DoubleFourOpen" => detector.CreatesDoubleFourOpen(x, y),
                            "DoubleThreeOpen" => detector.CreatesDoubleThreeOpen(x, y),
                            "OpenFour" => detector.CreatesOpenFour(x, y),
                            "DoubleFourMixed" => detector.CreatesDoubleFourMixed(x, y),
                            "DoubleThreeAndFour" => detector.CreatesDoubleThreeAndFour(x, y),
                            "SeparateDoubleFour" => detector.CreatesSeparateDoubleFour(x, y),
                            "OpenThree" => detector.CreatesOpenThree(x, y),
                            "SemiOpenFour" => detector.CreatesSemiOpenFour(x, y),
                            "SemiOpenThree" => detector.CreatesSemiOpenThree(x, y),
                            "OpenTwo" => detector.CreatesOpenTwo(x, y),
                            "SemiOpenTwo" => detector.CreatesSemiOpenTwo(x, y),
                            _ => false
                        };

                        // Remove the stone
                        _board.PlaceStone(x, y, Cell.Empty);

                        if (matches)
                            moves.Add(new Move(x, y));
                    }
                }
            }

            return moves;
        }
    }

}
