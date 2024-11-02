namespace NeoMines.Model;

public class GameModeInfo(int rows, int columns, Dictionary<GameMode, int> bombsCount)
{
    public readonly int Rows = rows;
    public readonly int Columns = columns;
    public readonly Dictionary<GameMode, int> BombsCount = bombsCount;
}