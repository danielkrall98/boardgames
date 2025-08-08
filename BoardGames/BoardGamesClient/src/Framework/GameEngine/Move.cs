namespace BoardGamesClient.src.Framework.GameEngine
{
    public abstract class Move
    {
        public Player Player { get; }
        
        protected Move(Player player)
        {
            Player = player;
        }
    }
}