namespace BoardGamesClient.src.Framework.GameEngine
{
    public abstract class GameState
    {
        public bool IsGameOver { get; protected set; }
        public abstract void Reset();
        public abstract bool CheckWinCondition();
        public abstract bool MakeMove(Move move);
         public Player CurrentPlayer { get; set; }
    public Player Winner { get; set; }

     // Switch the current player between two players
    public abstract void SwitchPlayer();

    }
}