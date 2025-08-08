using System.Collections.Generic;
using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.Chess
{
    public class ChessGameState : GameState
    {
        public ChessBoard Board { get; private set; }
        public List<Player> Players { get; private set; }

        public ChessGameState()
        {
            Players = new List<Player>();
        }

        public override void Reset()
        {
            Board = new ChessBoard();
            Board.Initialize();
            IsGameOver = false;
            CurrentPlayer = null;
            Winner = null;
        }

        public override bool CheckWinCondition()
        {
            // Win condition: King is captured (already handled in ChessGame)
            return IsGameOver;
        }

        public override bool MakeMove(Move move)
        {
            return false; // Not directly used
        }

        public override void SwitchPlayer()
        {
            if (Players.Count == 2)
            {
                CurrentPlayer = CurrentPlayer == Players[0] ? Players[1] : Players[0];
            }
        }


        public void SetGameOver(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }

    }
}
