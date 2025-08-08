using System.Collections.Generic;
using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.Chess
{
    public class ChessGameManager : IGameManager
    {
        private readonly Dictionary<string, IGame> _games = new();

        public string CreateGame<TGame>() where TGame : IGame, new()
        {
            var gameId = System.Guid.NewGuid().ToString();
            var game = new TGame();
            game.Initialize();
            _games[gameId] = game;
            return gameId;
        }

        public IGame GetGame(string gameId)
        {
            return _games.ContainsKey(gameId) ? _games[gameId] : null;
        }

        public bool AddPlayer(string gameId, Player player)
        {
            var game = GetGame(gameId);
            if (game == null) return false;

            var state = game.State as ChessGameState;
            if (state == null || state.Players.Count >= 2) return false;

            state.Players.Add(player);
            return true;
        }

        public bool MakeMove(string gameId, Move move)
        {
            var game = GetGame(gameId);
            if (game == null || game.State.IsGameOver) return false;

            game.PlayMove(move);
            return true;
        }

        public bool IsGameOver(string gameId)
        {
            var game = GetGame(gameId);
            return game?.State.IsGameOver ?? false;
        }

        public IEnumerable<string> GetActiveGames()
        {
            return _games.Keys;
        }
    }
}
