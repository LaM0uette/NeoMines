using Microsoft.AspNetCore.Components;
using NeoMines.Model.Game;

namespace NeoMines.Client.Pages;

public class GameBase : ComponentBase
{
    #region Statements
    
    protected const int Rows = 9;
    protected const int Columns = 6;

    [Parameter] public int GameModeIndex { get; set; }
    
    protected GameMode GameMode => (GameMode)GameModeIndex;
    protected int BombCount { get; set; }
    
    protected readonly Cell[,] Grid = new Cell[Rows, Columns];

    #endregion

    #region ComponentBase

    protected override void OnInitialized()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Grid[i, j] = new Cell(i, j);
            }
        }

        BombCount = GameMode switch
        {
            GameMode.Easy => 4,
            GameMode.Medium => 10,
            GameMode.Hard => 25,
            _ => throw new ArgumentOutOfRangeException()
        };

        CreateNewGame();
    }

    #endregion

    #region Methods

    private void CreateNewGame()
    {
        CreateBombs();
        CreateNumbers();
    }

    private void CreateBombs()
    {
        var random = new Random();
        var bombPlaced = 0;
        
        while (bombPlaced < BombCount)
        {
            var row = random.Next(Rows);
            var column = random.Next(Columns);

            if (Grid[row, column] is not BombCell)
            {
                Grid[row, column] = new BombCell(row, column);
                bombPlaced++;
            }
        }
    }

    private void CreateNumbers()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                if (Grid[i, j] is BombCell)
                    continue;

                var bombCount = 0;

                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        var neighborRow = i + x;
                        var neighborColumn = j + y;

                        if (neighborRow is >= 0 and < Rows && neighborColumn is >= 0 and < Columns && Grid[neighborRow, neighborColumn] is BombCell)
                            bombCount++;
                    }
                }

                Grid[i, j] = new NumberCell(i, j, bombCount);
            }
        }
    }

    #endregion
}