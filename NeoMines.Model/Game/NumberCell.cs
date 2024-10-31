namespace NeoMines.Model.Game;

public class NumberCell(int row, int column, int number) : Cell(row, column)
{
    public int Number { get; set; } = number;
}