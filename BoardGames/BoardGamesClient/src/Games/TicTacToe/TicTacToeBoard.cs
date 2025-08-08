using BoardGamesClient.src.Framework.GameEngine;

namespace BoardGamesClient.src.Games.TicTacToe
{
    public class TicTacToeBoard : Board
    {
        public string[,] Cells { get; private set; }

        public TicTacToeBoard()
        {
            Rows = 3;
            Columns = 3;
            Cells = new string[Rows, Columns];
        }

        public override void Initialize()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Cells[i, j] = string.Empty; // Empty cell
                }
            }
        }

        public override string Display()
        {
            string display = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    display += string.IsNullOrEmpty(Cells[i, j]) ? "-" : Cells[i, j];
                    if (j < Columns - 1) display += "|";
                }
                display += "\n";
            }
            return display;
        }
    }
}