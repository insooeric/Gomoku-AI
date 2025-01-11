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

            // Priority 1: Winning moves
            var winningMoves = FindMovesCreatingPattern("Five", _currentPlayer);
            if (winningMoves.Any())
                return winningMoves; // Immediate win, highest priority

            // Priority 2: Blocking opponent's winning moves
            var opponentWinningMoves = FindMovesCreatingPattern("Five", _opponent);
            if (opponentWinningMoves.Any())
                prioritizedMoves.AddRange(opponentWinningMoves);

            // Priority 3: Creating open fours
            var openFours = FindMovesCreatingPattern("OpenFour", _currentPlayer);
            if (openFours.Any())
                prioritizedMoves.AddRange(openFours);

            // Priority 4: Blocking opponent's open fours
            var opponentOpenFours = FindMovesCreatingPattern("OpenFour", _opponent);
            if (opponentOpenFours.Any())
                prioritizedMoves.AddRange(opponentOpenFours);

            // Priority 5: Creating open threes
            var openThrees = FindMovesCreatingPattern("OpenThree", _currentPlayer);
            if (openThrees.Any())
                prioritizedMoves.AddRange(openThrees);

            // Priority 6: Blocking opponent's open threes
            var opponentOpenThrees = FindMovesCreatingPattern("OpenThree", _opponent);
            if (opponentOpenThrees.Any())
                prioritizedMoves.AddRange(opponentOpenThrees);

            // If no prioritized moves found, return an empty list
            return prioritizedMoves;
        }

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
                            "OpenFour" => detector.CreatesOpenFour(x, y),
                            "OpenThree" => detector.CreatesOpenThree(x, y),
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
