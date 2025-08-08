using System.Numerics;

namespace BoardGamesClient.src.Framework.GameEngine
{
    public interface IGame
    {
        GameState State { get; }
        void Initialize();
        bool IsMoveValid(Move move);
        void PlayMove(Move move);
    }
}