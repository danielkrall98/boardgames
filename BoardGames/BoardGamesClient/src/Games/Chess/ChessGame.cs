using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.Chess
{
    public class ChessGame : IGame
    {
        public GameState State { get; private set; }

        public void Initialize()
        {
            State = new ChessGameState();
            State.Reset();
        }

        public bool IsMoveValid(Move move)
        {
            var chessMove = move as ChessMove;
            return chessMove != null && ((ChessGameState)State).Board.IsValidMove(chessMove);
        }

        public void PlayMove(Move move)
        {
            if (!IsMoveValid(move)) return;

            var chessMove = move as ChessMove;
            var gameState = (ChessGameState)State;

            // Apply the move
            gameState.Board.ApplyMove(chessMove);

            // Check if the opponent's King is still present
            string opponentKing = chessMove.Player.Id == "W" ? "BK" : "WK";
            bool opponentKingExists = false;

            foreach (var cell in gameState.Board.BoardState)
            {
                if (cell == opponentKing)
                {
                    opponentKingExists = true;
                    break;
                }
            }

            if (!opponentKingExists)
            {
                gameState.Winner = chessMove.Player;
                gameState.SetGameOver(true);
            }
            else
            {
                gameState.SwitchPlayer();
            }
        }

    }
}
