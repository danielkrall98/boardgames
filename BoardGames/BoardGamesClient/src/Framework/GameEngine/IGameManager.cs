using System.Collections.Generic;

namespace BoardGamesClient.src.Framework.GameEngine
{
    public interface IGameManager
    {
        // Creates a new game and returns its unique identifier
        string CreateGame<TGame>() where TGame : IGame, new();

        // Retrieves a game instance by its ID
        IGame GetGame(string gameId);

        // Adds a player to a specific game
        bool AddPlayer(string gameId, Player player);

        // Processes a player's move
        bool MakeMove(string gameId, Move move);

        // Checks if a game is finished
        bool IsGameOver(string gameId);

        // Retrieves a list of active games
        IEnumerable<string> GetActiveGames();
    }
}