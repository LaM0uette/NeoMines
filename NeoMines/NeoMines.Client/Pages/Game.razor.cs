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
    protected readonly Cell[,] Grid = new Cell[Rows, Columns];
    
    protected int BombCount { get; private set; }
    protected bool IsGameOver { get; private set; }
    protected bool IsWin { get; private set; }
    protected int CurrentFlagCount { get; private set; }

    #endregion

    #region ComponentBase

    protected override void OnInitialized()
    {
        IsGameOver = false;
        IsWin = false;
        CurrentFlagCount = 0;
        
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
    
    protected void OnLeftClick(int row, int column)
    {
        var cell = Grid[row, column];
        
        if (cell.IsDiscovered || cell.HasFlag) 
            return;
        
        if (cell is BombCell)
        {
            IsGameOver = true;
            RevealAllCells();
        }
        else
        {
            RevealAdjacentCells(row, column);
                    
            if (CheckVictory())
            {
                IsWin = true;
                RevealAllCells();
            }
        }
    }
    
    protected void OnRightClick(int row, int column)
    {
        if (IsGameOver || IsWin)
            return;

        var cell = Grid[row, column];
        
        if (!cell.IsDiscovered)
            cell.HasFlag = !cell.HasFlag;
        
        CurrentFlagCount = Grid.Cast<Cell>().Count(c => c.HasFlag);
    }

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
    
    private void RevealAllCells()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Grid[i, j].HasFlag = false;
                Grid[i, j].IsDiscovered = true;
            }
        }
    }
    
    private void RevealAdjacentCells(int row, int column)
    {
        var cell = Grid[row, column];
        
        if (cell.IsDiscovered || cell.HasFlag)
            return;

        cell.IsDiscovered = true;
        
        if (cell is NumberCell numberCell)
            if (numberCell.Number > 0)
                return;
        
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                var neighborRow = row + x;
                var neighborColumn = column + y;

                if (neighborRow is >= 0 and < Rows && neighborColumn is >= 0 and < Columns)
                    if (!Grid[neighborRow, neighborColumn].IsDiscovered)
                        RevealAdjacentCells(neighborRow, neighborColumn);
            }
        }
    }
    
    private bool CheckVictory()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                var cell = Grid[i, j];
                
                if (!cell.IsDiscovered && cell is not BombCell)
                    return false;
            }
        }
        
        return true;
    }

    #endregion
}