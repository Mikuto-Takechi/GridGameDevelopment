using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GridIndex = Minesweeper.GridIndex;

namespace LifeGame
{
    public class GameManager : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private float _fadeOutInterval = 0.25f;
        [SerializeField] private int _rows = 1;
        [SerializeField] private int _columns = 1;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup = null;
        [SerializeField] private Cell _cellPrefab = null;
        [SerializeField, Multiline] private string _data = "";
        [SerializeField] private float _updateInterval = 0.5f;
        private float _timer;
        private bool _isPlaying = true;
        private Cell[,] _cells;
        private Queue<(GridIndex, CellState)> _queue = new();
        private void Start()
        {
            _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayoutGroup.constraintCount = _columns;

            _cells = new Cell[_rows, _columns];
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _columns; c++)
                {
                    _cells[r, c] = Instantiate(_cellPrefab, _gridLayoutGroup.gameObject.transform);
                }
            }
            //  文字列を格納したStringReaderのインスタンスを作成する。
            //  入力された文字列の行と列と_rowsと_columnsがずれていたらエラーになる。
            StringReader sr = new StringReader(_data);
            int i = 0;
            while (sr.Peek() >= 0)
            {
                //  1行読み取り
                var readLine = sr.ReadLine();
                for (int j = 0; j < readLine.Length; j++)
                {
                    
                    if (readLine[j] == '0')
                    {   //  0なら死
                        _cells[i, j].State = CellState.Dead;
                    }
                    else if(readLine[j] == '1')
                    {   //  1なら生存
                        _cells[i, j].State = CellState.Alive;
                    }
                }

                i++;
            }
            sr.Close();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {   //  シミュレーションを一時停止・再生する
                _isPlaying = !_isPlaying;
            }

            if (_isPlaying)
            {   //  再生中なら一定間隔で世代を進める。
                _timer += Time.deltaTime;
                if (_timer > _updateInterval)
                {
                    OnNext();
                    _timer = 0;
                }
            }
            else
            {   //  一時停止中に右矢印を押すと世代を1進める。
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    OnNext();
                }
            }
        }

        private void OnNext()
        {
            //  全てのセルを更新する。
            // その場で変更処理を行わずにqueueに情報を溜めていく。(queueでやる意味は特にない)
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    var aroundAliveCount = CheckAround(new GridIndex(i, j));
                    if (_cells[i, j].State == CellState.Dead 
                        && aroundAliveCount == 3)
                    {   //  誕生
                        _queue.Enqueue((new GridIndex(i, j), CellState.Alive));
                    }
                    else if (_cells[i, j].State == CellState.Alive
                        && aroundAliveCount <= 1 || aroundAliveCount >= 4)
                    {   //  過疎・過密
                        _queue.Enqueue((new GridIndex(i, j), CellState.Dead));
                    }
                    else if(_cells[i, j].State == CellState.Dead)
                    {
                        _cells[i, j].FadeOut(_fadeOutInterval);
                    }
                }
            }

            while (_queue.Count > 0)
            {   //  queueの要素が0になるまで取り出して生存が死の状態に切り替える。
                var tuple = _queue.Dequeue();
                var index = tuple.Item1;
                _cells[index.Row, index.Column].State = tuple.Item2;
            }
        }
        /// <summary>
        /// 指定した周囲8方向を確認して生きているセルの数を返す。
        /// </summary>
        int CheckAround(GridIndex index)
        {
            int aliveCount = 0;
            for (int i = 0; i < 8; i++)
            {
                var row = index[i].Row;
                var col = index[i].Column;
                //  配列の範囲外は確認できないのでカウントしない
                if (row < _rows 
                    && row >= 0 
                    && col < _columns 
                    && col >= 0 
                    && _cells[row, col].State == CellState.Alive)
                {
                    aliveCount++;
                }
            }
            return aliveCount;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var target = eventData.pointerCurrentRaycast.gameObject;
            //  クリックしたセルの状態を切り替える。
            if (target.TryGetComponent<Cell>(out var cell))
            {
                cell.State = cell.State == CellState.Alive ? CellState.Dead : CellState.Alive;
            }
        }
    }
}