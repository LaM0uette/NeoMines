using Microsoft.AspNetCore.Components;

namespace NeoMines.Client.Pages;

public class HomeBase : ComponentBase
{
    #region Statements

    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    #endregion

    #region HomeBase

    protected void GoToGamePage()
    {
        _navigationManager.NavigateTo("/Game");
    }

    #endregion
}