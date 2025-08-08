using System;
using System.Collections.Generic;
using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.TicTacToe
{
    public class TicTacToeGameManager : IGameManager
    {
        private readonly Dictionary<string, IGame> _games = new();
        private readonly Dictionary<string, List<Player>> _gamePlayers = new();

        // Creates a new TicTacToe game and returns its unique gameId
        public string CreateGame<TGame>() where TGame : IGame, new()
        {
            // Create a new instance of the game
            var game = new TGame();

            // Generate a unique game ID (for simplicity, use a timestamp or GUID)
            string gameId = Guid.NewGuid().ToString();

            // Add the game to the dictionary of active games
            _games[gameId] = game;

            // Return the game ID
            return gameId;
        }

        // Retrieve a game instance by its gameId
        public IGame GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        // Adds a player to an existing game by gameId
        public bool AddPlayer(string gameId, Player player)
        {
            if (_gamePlayers.ContainsKey(gameId) && _gamePlayers[gameId].Count < 2)  // Tic-Tac-Toe supports 2 players
            {
                _gamePlayers[gameId].Add(player);
                return true;
            }
            return false;  // Game is already full or does not exist
        }

        // Processes a player's move in a game
        public bool MakeMove(string gameId, Move move)
        {
            var game = GetGame(gameId);
            if (game == null || !game.IsMoveValid(move)) return false;

            game.PlayMove(move);  // Executes the move and checks game state

            return true;
        }

        // Checks if the game with the given gameId is over
        public bool IsGameOver(string gameId)
        {
            var game = GetGame(gameId);
            return game?.State.IsGameOver ?? false;
        }

        // Retrieves the winner of a game, if any
        public Player GetWinner(string gameId)
        {
            var game = GetGame(gameId);
            return game?.State.Winner;
        }

        // Retrieves a list of active games (gameIds)
        public IEnumerable<string> GetActiveGames()
        {
            return _games.Keys;
        }

        // Allows players to leave a game
        public bool RemovePlayer(string gameId, Player player)
        {
            if (_gamePlayers.ContainsKey(gameId))
            {
                return _gamePlayers[gameId].Remove(player);
            }
            return false;
        }

        // Resets the game by its gameId (e.g., after a game ends or when a player wants to restart)
        public void ResetGame(string gameId)
        {
            var game = GetGame(gameId);
            if (game != null)
            {
                game.Initialize();  // Resets game state
            }
        }

        public int GetPlayerCount(string gameId)
        {
            return _gamePlayers.ContainsKey(gameId) ? _gamePlayers[gameId].Count : 0;
        }

        public List<Player> GetPlayers(string gameId)
        {
            return _gamePlayers.ContainsKey(gameId) ? _gamePlayers[gameId] : new List<Player>();
        }
    }
}