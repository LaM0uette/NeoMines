using Microsoft.AspNetCore.Components;
using NeoMines.Model.Game;

namespace NeoMines.Client.Pages;

public class GameBase : ComponentBase
{
    #region Statements

    [Parameter] public int GameModeIndex { get; set; }
    protected GameMode GameMode => (GameMode)GameModeIndex;
    
    protected readonly Cell[,] Grid = new Cell[9, 6];

    #endregion

    #region ComponentBase

    protected override void OnInitialized()
    {
        for (var i = 0; i < Grid.GetLength(0); i++)
        {
            for (var j = 0; j < Grid.GetLength(1); j++)
            {
                Grid[i, j] = new Cell(i, j);
            }
        }
    }

    #endregion
}