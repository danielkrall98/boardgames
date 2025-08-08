using System;
using System.Text;
using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.Chess
{
    public class ChessBoard : Board
    {
        public string[,] BoardState { get; private set; }

        public ChessBoard()
        {
            Rows = 8;
            Columns = 8;
            Initialize();
        }

        public override void Initialize()
        {
            BoardState = new string[Rows, Columns];

            // Populate the initial board state row by row
            string[,] initialSetup = new string[8, 8]
            {
                { "BR", "BN", "BB", "BQ", "BK", "BB", "BN", "BR" },
                { "BP", "BP", "BP", "BP", "BP", "BP", "BP", "BP" },
                { "", "", "", "", "", "", "", "" },
                { "", "", "", "", "", "", "", "" },
                { "", "", "", "", "", "", "", "" },
                { "", "", "", "", "", "", "", "" },
                { "WP", "WP", "WP", "WP", "WP", "WP", "WP", "WP" },
                { "WR", "WN", "WB", "WQ", "WK", "WB", "WN", "WR" }
            };

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    BoardState[row, col] = initialSetup[row, col];
                }
            }
        }

        public override string Display()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    sb.Append(string.IsNullOrEmpty(BoardState[row, col]) ? "." : BoardState[row, col]);
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public bool IsValidMove(ChessMove move)
        {
            string piece = BoardState[move.SourceRow, move.SourceColumn];

            if (string.IsNullOrEmpty(piece)) return false;

            // Ensure the piece belongs to the current player
            if (piece[0] != move.Player.Id[0]) return false;

            // Delegate validation to the piece-specific logic
            string pieceType = piece[1].ToString(); // Second character of the piece string determines the type
            switch (pieceType)
            {
                case "P":
                    return IsValidPawnMove(move);
                case "R":
                    return IsValidRookMove(move);
                case "N":
                    return IsValidKnightMove(move);
                case "B":
                    return IsValidBishopMove(move);
                case "Q":
                    return IsValidQueenMove(move);
                case "K":
                    return IsValidKingMove(move);
                default:
                    return false;
            }
        }

        public void ApplyMove(ChessMove move)
        {
            BoardState[move.TargetRow, move.TargetColumn] = BoardState[move.SourceRow, move.SourceColumn];
            BoardState[move.SourceRow, move.SourceColumn] = "";
        }

        // CHESS PIECE RULES //

        // Don't jump over other Pieces
        private bool IsPathClear(int sourceRow, int sourceCol, int targetRow, int targetCol)
        {
            int rowDirection = targetRow > sourceRow ? 1 : (targetRow < sourceRow ? -1 : 0);
            int colDirection = targetCol > sourceCol ? 1 : (targetCol < sourceCol ? -1 : 0);

            int currentRow = sourceRow + rowDirection;
            int currentCol = sourceCol + colDirection;

            while (currentRow != targetRow || currentCol != targetCol)
            {
                if (!string.IsNullOrEmpty(BoardState[currentRow, currentCol]))
                    return false;

                currentRow += rowDirection;
                currentCol += colDirection;
            }

            return true;
        }

        private bool IsValidPawnMove(ChessMove move)
        {
            // Direction on board (White up and Black down)
            int direction = move.Player.Id == "W" ? -1 : 1;

            // One forward
            if (move.TargetColumn == move.SourceColumn &&
                move.TargetRow == move.SourceRow + direction &&
                string.IsNullOrEmpty(BoardState[move.TargetRow, move.TargetColumn]))
            {
                return true;
            }

            // Two forward (from starting position)
            if (move.TargetColumn == move.SourceColumn &&
                move.SourceRow == (move.Player.Id == "W" ? 6 : 1) &&
                move.TargetRow == move.SourceRow + 2 * direction &&
                string.IsNullOrEmpty(BoardState[move.TargetRow, move.TargetColumn]) &&
                string.IsNullOrEmpty(BoardState[move.SourceRow + direction, move.SourceColumn]))
            {
                return true;
            }

            // Capture diagnoally
            if (Math.Abs(move.TargetColumn - move.SourceColumn) == 1 &&
                move.TargetRow == move.SourceRow + direction &&
                !string.IsNullOrEmpty(BoardState[move.TargetRow, move.TargetColumn]) &&
                BoardState[move.TargetRow, move.TargetColumn][0] != move.Player.Id[0])
            {
                return true;
            }

            return false;
        }

        private bool IsValidRookMove(ChessMove move)
        {
            if (move.SourceRow != move.TargetRow && move.SourceColumn != move.TargetColumn)
                return false;

            return IsPathClear(move.SourceRow, move.SourceColumn, move.TargetRow, move.TargetColumn);
        }

        private bool IsValidKnightMove(ChessMove move)
        {
            int rowDiff = Math.Abs(move.SourceRow - move.TargetRow);
            int colDiff = Math.Abs(move.SourceColumn - move.TargetColumn);

            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }

        private bool IsValidBishopMove(ChessMove move)
        {
            int rowDiff = Math.Abs(move.SourceRow - move.TargetRow);
            int colDiff = Math.Abs(move.SourceColumn - move.TargetColumn);

            return rowDiff == colDiff && IsPathClear(move.SourceRow, move.SourceColumn, move.TargetRow, move.TargetColumn);
        }

        private bool IsValidQueenMove(ChessMove move)
        {
            return IsValidRookMove(move) || IsValidBishopMove(move);
        }

        private bool IsValidKingMove(ChessMove move)
        {
            int rowDiff = Math.Abs(move.SourceRow - move.TargetRow);
            int colDiff = Math.Abs(move.SourceColumn - move.TargetColumn);

            return rowDiff <= 1 && colDiff <= 1;
        }
    }
}
