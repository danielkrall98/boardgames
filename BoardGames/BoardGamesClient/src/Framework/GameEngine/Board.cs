namespace BoardGamesClient.src.Framework.GameEngine
{
    public abstract class Board
    {
        public int Rows { get; protected set; }
        public int Columns { get; protected set; }
        
        public abstract void Initialize();
        public abstract string Display();
    }
}