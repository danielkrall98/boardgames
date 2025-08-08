using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.TicTacToe
{
    public class TicTacToeGame : IGame
    {
        public GameState State { get; private set; }

        public TicTacToeGame()
        {
            State = new TicTacToeGameState();
        }

        public void Initialize()
        {
            State.Reset();
        }

        public bool IsMoveValid(Move move)
        {
            if (move is TicTacToeMove tttMove)
            {
                var board = ((TicTacToeGameState)State).Board;
                return tttMove.Row >= 0 && tttMove.Row < 3 &&
                       tttMove.Column >= 0 && tttMove.Column < 3 &&
                       string.IsNullOrEmpty(board.Cells[tttMove.Row, tttMove.Column]);
            }
            return false;
        }

        public void PlayMove(Move move)
        {
            if (State.MakeMove(move))
            {
                if (!State.CheckWinCondition())
                {
                    // Switch turns if the game is not over
                    State.SwitchPlayer();
                }
            }
        }
    }
}