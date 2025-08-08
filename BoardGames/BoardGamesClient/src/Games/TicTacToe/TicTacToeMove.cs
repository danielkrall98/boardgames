using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.TicTacToe
{
    public class TicTacToeMove : Move
    {
        public int Row { get; }
        public int Column { get; }

        public TicTacToeMove(Player player, int row, int column) : base(player)
        {
            Row = row;
            Column = column;
        }
    }
}