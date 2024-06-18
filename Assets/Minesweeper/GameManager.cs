using System;
using System.Collections.Generic;
using GridGameDevelopment;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Minesweeper
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _rows = 7;
        [SerializeField] private int _columns = 7;
        [SerializeField] private int _mineCount = 7;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup = null;
        [SerializeField] private Cell _cellPrefab = null;
        private Cell[,] _cells;
        /// <summary>地雷生成時の地雷の個数。<br/>
        /// _mineCountが途中で書き換えられた場合クリア判定が出来ない。<br/>
        /// SerializeFieldで書き換えられない変数で保存しておく必要があった。</summary>
        private int _initialMineCount;

        private float _timer = 0;
        private GameState _gameState = GameState.Initialize;
        private void Start()
        {
            _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayoutGroup.constraintCount = _columns;

            _cells = new Cell[_rows, _columns];
            var parent = _gridLayoutGroup.transform;
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    _cells[r, c] = Instantiate(_cellPrefab, parent);
                    _cells[r, c].Index = new GridIndex(r, c);
                    _cells[r, c].OnClicked += FloodFill;
                    _cells[r, c].OnClicked += Initialize;
                    _cells[r, c].GameOver += GameOver;
                }
            }
        }

        private void Update()
        {
            if (_gameState == GameState.InGame)
            {
                _timer += Time.deltaTime;
            }
        }

        void Initialize(GridIndex startIndex)
        {
            if (_gameState != GameState.Initialize) return;
            CreateMine(startIndex);
            SetAroundMinesCount();
            _gameState = GameState.InGame;
            FloodFill(startIndex);
        }

        void GameOver()
        {
            Debug.Log($"ゲームオーバー {Utility.TimeFormat(_timer)}");
            _gameState = GameState.EndGame;
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    _cells[r, c].IsGameOver = true;
                }
            }
        }

        bool CheckGameClear()
        {
            var openCount = 0;
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    if (!_cells[r, c].IsClosed)
                    {
                        openCount++;
                    }
                }
            }

            return openCount == _rows * _columns - _initialMineCount;
        }
        void GameClear()
        {
            if (!CheckGameClear()) return;
            
            Debug.Log($"ゲームクリア {Utility.TimeFormat(_timer)}");
            _gameState = GameState.EndGame;
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    _cells[r, c].IsGameOver = true;
                }
            }
        }
        /// <summary>
        /// 地雷生成
        /// </summary>
        void CreateMine(GridIndex startIndex)
        {
            var clampedMineCount = Mathf.Clamp(_mineCount, 0, _rows * _columns);
            var remainingCells = new List<Cell>();
            //  全てのセルをリストに追加していく
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    var isStartIndexAround = false;
                    for (int i = 0; i < 9; i++)
                    {   //  周囲8方向と最初にクリックした地点合わせて9回ループさせる。
                        if (startIndex[i].Equals(new GridIndex(r, c)))
                        {
                            isStartIndexAround = true;
                            break;
                        }
                    }
                    //  r, cが最初にクリックしたセルかその周りでなければ
                    //  地雷を置けるようにする。
                    if(!isStartIndexAround) remainingCells.Add(_cells[r, c]);
                }
            }

            int createdMineCount = 0;
            for (var i = 0; i < clampedMineCount; i++)
            {
                if (remainingCells.Count <= 0) break;   //  空いているセルが無くなればbreakする。
                
                //  セルのリストからランダムに選んで地雷を設置、リストから削除する。
                var randomIndex = Random.Range(0, remainingCells.Count);
                remainingCells[randomIndex].CellState = CellState.Mine;
                remainingCells.RemoveAt(randomIndex);
                createdMineCount++;
            }
            _initialMineCount = createdMineCount;
        }
        /// <summary>
        /// セルの周りの地雷の数を数えてセルの見た目を変える
        /// </summary>
        void SetAroundMinesCount()
        {
            for (var r = 0; r < _rows; r++)
            {
                for(var c = 0; c < _columns; c++)
                {
                    if(_cells[r, c].CellState == CellState.Mine) continue;
                    int mineCount = 0;
                    var gridIndex = new GridIndex(r, c);
                    for (int i = 0; i < 8; i++)
                    {
                        mineCount += CheckMine(gridIndex[i]);
                    }
                    _cells[r, c].CellState = (CellState)Enum.ToObject(typeof(CellState), mineCount);
                }
            }
        }
        /// <summary>
        /// 地雷があるかチェックする
        /// </summary>
        int CheckMine(GridIndex gridIndex)
        {
            if (0 <= gridIndex.Row &&
                0 <= gridIndex.Column &&
                _rows > gridIndex.Row &&
                _columns > gridIndex.Column &&
                _cells[gridIndex.Row, gridIndex.Column].CellState == CellState.Mine)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 閉じられているかチェックする
        /// </summary>
        bool CheckClose(GridIndex gridIndex)
        {
            return 0 <= gridIndex.Row &&
                   0 <= gridIndex.Column &&
                   _rows > gridIndex.Row &&
                   _columns > gridIndex.Column &&
                   _cells[gridIndex.Row, gridIndex.Column].IsClosed;
        }
        /// <summary>
        /// 塗りつぶし
        /// </summary>
        void FloodFill(GridIndex startIndex)
        {
            if (_gameState == GameState.Initialize) return;
            Queue<GridIndex> queue = new Queue<GridIndex>();
            queue.Enqueue(startIndex);
            while (queue.Count > 0)
            {
                var currentIndex = queue.Dequeue();
                if(_cells[currentIndex.Row, currentIndex.Column].CellState != CellState.None) continue;

                for (int i = 0; i < 8; i++)
                {
                    var aroundIndex = currentIndex[i];
                    if (CheckClose(aroundIndex))
                    {
                        _cells[aroundIndex.Row, aroundIndex.Column].IsClosed = false;
                        queue.Enqueue(aroundIndex);
                    }
                }
            }
            GameClear();
        }
    }
}