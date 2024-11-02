﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NeoMines.Model;

namespace NeoMines.Client.Pages;

public class GameBase : ComponentBase
{
    #region Statements
    
    private const int _mobileRows = 9;
    private const int _mobileColumns = 6;
    private const int _desktopRows = 12;
    private const int _desktopColumns = 22;
    
    protected int Rows;
    protected int Columns;

    [Parameter] public int GameModeIndex { get; set; }
    
    [Inject] private IJSRuntime _jsRuntime { get; set; } = null!;

    protected GameMode GameMode => (GameMode)GameModeIndex;
    protected Cell[,] Grid = null!;
    
    protected int BombCount { get; private set; }
    protected bool IsGameOver { get; private set; }
    protected bool IsWin { get; private set; }
    protected int CurrentFlagCount { get; private set; }
    
    protected string FormattedTime => FormatTime(_secondsElapsed);
    private System.Timers.Timer? _timer;
    private int _secondsElapsed;

    #endregion

    #region ComponentBase
    
    protected override async Task OnInitializedAsync()
    {
        var isSmallScreen = await _jsRuntime.InvokeAsync<bool>("isSmallScreen");

        if (isSmallScreen)
        {
            Rows = _mobileRows;
            Columns = _mobileColumns;
        }
        else
        {
            Rows = _desktopRows;
            Columns = _desktopColumns;
        }
        
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
            
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= UpdateTime;
            }
            
            RevealAllCells();
        }
        else
        {
            RevealAdjacentCells(row, column);
                    
            if (CheckVictory())
            {
                IsWin = true;
                RevealAllCells();
                
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Elapsed -= UpdateTime;
                }
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
        
        StateHasChanged();
    }
    
    protected void RestartGame()
    {
        CreateNewGame();
    }

    private void CreateNewGame()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
        
        _secondsElapsed = 0;
        
        InitGame();
        CreateBombs();
        CreateNumbers();
        
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += UpdateTime;
        _timer.Start();
        
        StateHasChanged();
    }

    private void InitGame()
    {
        IsGameOver = false;
        IsWin = false;
        CurrentFlagCount = 0;
        
        Grid = new Cell[Rows, Columns];
        
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

                        if (IsValidCell(neighborRow, neighborColumn) && Grid[neighborRow, neighborColumn] is BombCell)
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
        
        StateHasChanged();
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

                if (IsValidCell(neighborRow, neighborColumn))
                    if (!Grid[neighborRow, neighborColumn].IsDiscovered)
                        RevealAdjacentCells(neighborRow, neighborColumn);
            }
        }
        
        StateHasChanged();
    }
    
    private bool IsValidCell(int row, int column) 
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
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
    
    private void UpdateTime(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _secondsElapsed++;
        InvokeAsync(StateHasChanged);
    }
    
    private static string FormatTime(int totalSeconds)
    {
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    #endregion
}