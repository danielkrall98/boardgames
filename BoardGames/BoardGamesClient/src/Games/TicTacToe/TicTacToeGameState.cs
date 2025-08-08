using System;
using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.TicTacToe
{
    public class TicTacToeGameState : GameState
    {
        public TicTacToeBoard Board { get; private set; }

        private int _turnCounter;

        public TicTacToeGameState()
        {
            Board = new TicTacToeBoard();
            Board.Initialize();
            _turnCounter = 0;
            // Initialize CurrentPlayer to Player X
            CurrentPlayer = new Player("X", "Player X");
        }

        public override void Reset()
        {
            Board.Initialize();
            _turnCounter = 0;
            IsGameOver = false;
            Winner = null;
        }
        
        public override bool CheckWinCondition()
        {
            // Check rows, columns, and diagonals
            string[,] cells = Board.Cells;

            for (int i = 0; i < 3; i++)
            {
                // Check rows
                if (!string.IsNullOrEmpty(cells[i, 0]) && cells[i, 0] == cells[i, 1] && cells[i, 1] == cells[i, 2])
                {
                    Winner = CurrentPlayer;
                    IsGameOver = true;
                    return true;
                }

                // Check columns
                if (!string.IsNullOrEmpty(cells[0, i]) && cells[0, i] == cells[1, i] && cells[1, i] == cells[2, i])
                {
                    Winner = CurrentPlayer;
                    IsGameOver = true;
                    return true;
                }
            }

            // Check diagonals
            if (!string.IsNullOrEmpty(cells[0, 0]) && cells[0, 0] == cells[1, 1] && cells[1, 1] == cells[2, 2])
            {
                Winner = CurrentPlayer;
                IsGameOver = true;
                return true;
            }

            if (!string.IsNullOrEmpty(cells[0, 2]) && cells[0, 2] == cells[1, 1] && cells[1, 1] == cells[2, 0])
            {
                Winner = CurrentPlayer;
                IsGameOver = true;
                return true;
            }

            // Check for draw
            if (++_turnCounter >= 9)
            {
                IsGameOver = true;
                Winner = null; // No winner in a draw
            }

            return false;
        }

        public override bool MakeMove(Move move)
        {
            if (CurrentPlayer == null)
            {
                throw new InvalidOperationException("CurrentPlayer is not set.");
            }

            if (move is TicTacToeMove tttMove)
            {
                if (tttMove.Row < 0 || tttMove.Row >= 3 || tttMove.Column < 0 || tttMove.Column >= 3)
                {
                    return false; // Out of bounds
                }

                if (!string.IsNullOrEmpty(Board.Cells[tttMove.Row, tttMove.Column]))
                {
                    return false; // Cell already occupied
                }

                Board.Cells[tttMove.Row, tttMove.Column] = CurrentPlayer.Id;
                return true;
            }
            return false;
        }

        public override void SwitchPlayer()
        {
            if (CurrentPlayer.Id == "X")
            {
                CurrentPlayer = new Player("O", "Player O"); // Switch to Player O
            }
            else
            {
                CurrentPlayer = new Player("X", "Player X"); // Switch to Player X
            }
        }

    }

}