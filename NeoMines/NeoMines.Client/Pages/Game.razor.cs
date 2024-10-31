using Microsoft.AspNetCore.Components;
using NeoMines.Model.Game;

namespace NeoMines.Client.Pages;

public class GameBase : ComponentBase
{
    #region Statements

    [Parameter] public int GameModeIndex { get; set; }
    protected GameMode GameMode => (GameMode)GameModeIndex;

    #endregion
}