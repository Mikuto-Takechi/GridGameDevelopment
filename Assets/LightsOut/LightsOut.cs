using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameState
{
    Initialize,
    Game,
    Clear,
}
public class LightsOut : MonoBehaviour
{
    [SerializeField] private int _randomPlaceCount = 10;
    [SerializeField] private int _row = 5;
    [SerializeField] private int _column = 5;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _movesText;
    private Cell[,] _cells;
    //  y == row, x == column
    private Vector2Int _selectedIndex;
    private GameState _state = GameState.Initialize;
    private float _time;
    private int _moves;

    public GameState State => _state;
    private void Start()
    {
        if (TryGetComponent(out GridLayoutGroup grid))
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = _column;
        }
        _cells = new Cell[_row, _column];
        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _column; c++)
            {
                var cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                _cells[r,c] = cell.AddComponent<Cell>();
                _cells[r, c].Row = r;
                _cells[r, c].Column = c;
                _cells[r, c].OnClicked += OnClicked;
                _cells[r, c].LightsOutManager = this;
            }
        }
        if (_timeText) _timeText.text = TimeFormat(_time);
        if (_movesText) _movesText.text = _moves.ToString();

        RandomizeBoard();
        
        _state = GameState.Game;
    }

    private void Update()
    {
        if (_state == GameState.Game)
        {
            _time += Time.deltaTime;
            if (_timeText) _timeText.text = TimeFormat(_time);
        }
        else if (_state == GameState.Clear && Input.GetButtonDown("Jump"))
        {
            ReStart();
        }
    }

    void RandomizeBoard()
    {
        List<int> randomRows = Enumerable.Range(0, _row).ToList();
        List<int> randomColumns = Enumerable.Range(0, _column).ToList();
        for (int i = 0; i < _randomPlaceCount; i++)
        {
            if (randomRows.Count <= 0 || randomColumns.Count <= 0) break;
            
            var randomRow = randomRows[Random.Range(0, randomRows.Count)];
            var randomColumn = randomColumns[Random.Range(0, randomColumns.Count)];
            _cells[randomRow, randomColumn].ChainSwitchColor();
            randomRows.Remove(randomRow);
            randomColumns.Remove(randomColumn);
        }
    }

    void OnClicked(int row, int column)
    {
        var rowLeft = row - 1;
        if (rowLeft >= 0)
        {
            _cells[rowLeft, column].SwitchColor();
        }

        var rowRight = row + 1;
        if (rowRight < _cells.GetLength(0))
        {
            _cells[rowRight, column].SwitchColor();
        }

        var columnDown = column - 1;
        if (columnDown >= 0)
        {
            _cells[row, columnDown].SwitchColor();
        }

        var columnUp = column + 1;
        if (columnUp < _cells.GetLength(1))
        {
            _cells[row, columnUp].SwitchColor();
        }

        if (_state != GameState.Game) return;
        _moves++;
        if (_movesText) _movesText.text = _moves.ToString();
        
        if (CheckCells())
        {
            _state = GameState.Clear;
        }
    }

    bool CheckCells()
    {
        int fillCount = 0;
        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _column; c++)
            {
                if (_cells[r, c].CellColor)
                {
                    fillCount++;
                }
            }
        }

        return fillCount == _row * _column;
    }

    void ReStart()
    {
        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _column; c++)
            {
                _cells[r, c].CellColor = false;
                _cells[r, c].CellImage.color = Color.white;
            }
        }
        _time = 0;
        _moves = 0;
        if (_timeText) _timeText.text = TimeFormat(_time);
        if (_movesText) _movesText.text = _moves.ToString();
        RandomizeBoard();
        _state = GameState.Game;
    }

    string TimeFormat(float time)
    {
        return $"{(int)time / 60:00}:{time % 60:00.00}";
    } 
}