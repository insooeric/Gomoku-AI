namespace Gomoku_AI.AIModels.MCTS
{
    public class MCTS
    {
        private readonly int _iterations;
        private readonly Cell _aiPlayer;
        private readonly Cell _opponent;

        public MCTS(int iterations, Cell aiPlayer)
        {
            _iterations = iterations;
            _aiPlayer = aiPlayer;
            _opponent = aiPlayer == Cell.Black ? Cell.White : Cell.Black;
        }

        public Move FindBestMove(Board rootState)
        {
            var rootNode = new MCTSNode(rootState.Clone(), null, null, _opponent);

            for (int i = 0; i < _iterations; i++)
            {
                // Selection
                var node = rootNode;
                var state = rootState.Clone();

                // Traverse the tree
                while (node.IsFullyExpanded() && node.Children.Any())
                {
                    node = node.SelectChild();
                    state.PlaceStone(node.Move.X, node.Move.Y, node.Player);
                    if (IsTerminal(state, node.Player))
                        break;
                }

                // Expansion
                if (!IsTerminal(state, node.Player))
                {
                    var untriedMoves = node.GetUntriedMoves();
                    if (untriedMoves.Any())
                    {
                        var move = untriedMoves[new Random().Next(untriedMoves.Count)];
                        state.PlaceStone(move.X, move.Y, GetNextPlayer(node.Player));
                        var childNode = node.AddChild(move, state.Clone(), GetNextPlayer(node.Player));
                        node = childNode;
                    }
                }

                // Simulation
                var result = Simulate(state, GetNextPlayer(node.Player));

                // Backpropagation
                while (node != null)
                {
                    node.Update(result == node.Player ? 1 : 0);
                    node = node.Parent;
                }
            }

            // Select the move with the highest visit count
            var bestMove = rootNode.Children.OrderByDescending(c => c.Visits).FirstOrDefault()?.Move;
            return bestMove;
        }

        private Cell GetNextPlayer(Cell current)
        {
            return current == Cell.Black ? Cell.White : Cell.Black;
        }

        private bool IsTerminal(Board state, Cell lastPlayer)
        {
            // Implement win condition check
            var detector = new PatternDetector(state, lastPlayer);
            for (int x = 0; x < state.Size; x++)
                for (int y = 0; y < state.Size; y++)
                    if (state.GetCell(x, y) == lastPlayer)
                        if (detector.CreatesFive(x, y))
                            return true;

            // Check for draw
            return !state.Grid.Cast<Cell>().Any(cell => cell == Cell.Empty);
        }

        private Cell Simulate(Board state, Cell currentPlayer)
        {
            var rnd = new Random();
            while (true)
            {
                var possibleMoves = GetPossibleMoves(state);
                if (!possibleMoves.Any())
                    return Cell.Empty; // Draw

                var move = possibleMoves[rnd.Next(possibleMoves.Count)];
                state.PlaceStone(move.X, move.Y, currentPlayer);

                var detector = new PatternDetector(state, currentPlayer);
                if (detector.CreatesFive(move.X, move.Y))
                    return currentPlayer;

                currentPlayer = GetNextPlayer(currentPlayer);
            }
        }

        private List<Move> GetPossibleMoves(Board board)
        {
            var moves = new List<Move>();
            int[] directions = { -1, 0, 1 };
            var hasNeighbor = new bool[board.Size, board.Size];

            for (int x = 0; x < board.Size; x++)
            {
                for (int y = 0; y < board.Size; y++)
                {
                    if (board.GetCell(x, y) != Cell.Empty)
                    {
                        foreach (var dx in directions)
                        {
                            foreach (var dy in directions)
                            {
                                if (dx == 0 && dy == 0)
                                    continue;

                                int nx = x + dx;
                                int ny = y + dy;

                                if (IsValid(nx, ny, board) && board.IsEmpty(nx, ny))
                                {
                                    hasNeighbor[nx, ny] = true;
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < board.Size; x++)
            {
                for (int y = 0; y < board.Size; y++)
                {
                    if (hasNeighbor[x, y])
                        moves.Add(new Move(x, y));
                }
            }

            // If the board is empty, choose the center
            if (!moves.Any())
                moves.Add(new Move(board.Size / 2, board.Size / 2));

            return moves;
        }

        private bool IsValid(int x, int y, Board board)
        {
            return x >= 0 && x < board.Size && y >= 0 && y < board.Size;
        }
    }

}
