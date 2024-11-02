namespace NeoMines.Model;

public class Cell(int row, int column)
{
    public int Row { get; set; } = row;
    public int Column { get; set; } = column;
    public bool HasFlag { get; set; }
    public bool IsDiscovered { get; set; }
}