using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.Chess
{
    public class ChessMove : Move
    {
        public int SourceRow { get; }
        public int SourceColumn { get; }
        public int TargetRow { get; }
        public int TargetColumn { get; }

        public ChessMove(Player player, int sourceRow, int sourceColumn, int targetRow, int targetColumn) 
            : base(player)
        {
            SourceRow = sourceRow;
            SourceColumn = sourceColumn;
            TargetRow = targetRow;
            TargetColumn = targetColumn;
        }
    }
}
