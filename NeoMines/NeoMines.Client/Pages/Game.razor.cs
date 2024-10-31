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

    #endregion
}