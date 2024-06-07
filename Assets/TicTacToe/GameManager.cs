using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe
{
    public enum GameEndType
    {
        CircleWin,
        CrossWin,
        Draw
    }
    public class GameManager : MonoBehaviour
    {
        private const int Size = 3;
        /// <summary>セルの配列</summary>
        private Image[,] _cells;
        /// <summary>配置済み情報の配列</summary>
        private bool[,] _placedCells;
        /// <summary>勝利パターンの配列</summary>
        private int[,] _victoryPattern;
        private Vector2Int _selectedIndex;
        private bool _isPlayerTurn = true;
        private bool _isGameEnd = false;
        [SerializeField] private Sprite _circleSprite;
        [SerializeField] private Sprite _crossSprite;
        [SerializeField] private Color _selectedColor = Color.grey;
        [SerializeField] private Color _normalColor = Color.white;
        
        public Vector2Int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value.x < 0)
                {
                    value.x = _cells.GetLength(1) - 1;
                }
                else if (value.y < 0)
                {
                    value.y = _cells.GetLength(0) - 1;
                }
                value.x %= _cells.GetLength(1);
                value.y %= _cells.GetLength(0);
                _selectedIndex = value;
            }
        }
        void Start()
        {
            //  GridLayoutGroupの設定を変更する
            if (TryGetComponent(out GridLayoutGroup gridLayoutGroup))
            {
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = Size;
            }
            //  勝利パターンを生成
            CreateVictoryPattern();
            _placedCells = new bool[Size, Size];
            _cells = new Image[Size, Size];
            //  Imageを生成
            for (var r = 0; r < Size; r++)
            {
                for (var c = 0; c < Size; c++)
                {
                    var obj = new GameObject($"Cell({r}, {c})");
                    obj.transform.parent = transform;

                    var image = obj.AddComponent<Image>();
                    if (r == 0 && c == 0)
                    {
                        image.color = _selectedColor; 
                    }
                    else
                    {
                        image.color = _normalColor;
                    }

                    _cells[r, c] = image;
                }
            }
        }
        void MoveSelect()
        {
            Vector2Int inputVector = Vector2Int.zero;
            if (Input.GetButtonDown("Horizontal"))
            {
                inputVector.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            }
            else if (Input.GetButtonDown("Vertical"))
            {
                inputVector.y = -Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            }

            if (inputVector != Vector2Int.zero)
            {
                var prevIndex = SelectedIndex;
                SelectedIndex += inputVector;
                UpdateSelectedColor(prevIndex, SelectedIndex);
            }
        }
        /// <summary>
        /// 移動後のセルの色を変える
        /// </summary>
        void UpdateSelectedColor(Vector2Int prev, Vector2Int next)
        {
            _cells[prev.y, prev.x].color = _normalColor;
            _cells[next.y, next.x].color = _selectedColor;
        }
        /// <summary>
        /// 選択しているセルにプレイヤーのコマを配置する
        /// </summary>
        void PlayerPlace()
        {
            if (Input.GetButtonDown("Jump") && !_placedCells[_selectedIndex.y, _selectedIndex.x])
            {
                _cells[_selectedIndex.y, _selectedIndex.x].sprite = _circleSprite;
                _placedCells[_selectedIndex.y, _selectedIndex.x] = true;
                _isPlayerTurn = false;
                TurnEnd();
            }
        }
        /// <summary>
        /// ランダムな空いているセルに敵のコマを配置する
        /// </summary>
        void EnemyPlace()
        {
            List<Vector2Int> emptyCells = new();
            bool isFound = false;
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    if (!_placedCells[i, j])
                    {
                        emptyCells.Add(new Vector2Int(j, i));
                        isFound = true;
                    }
                }
            }

            if (isFound)
            {
                var randomIndex = emptyCells[Random.Range(0, emptyCells.Count)];
                _placedCells[randomIndex.y, randomIndex.x] = true;
                _cells[randomIndex.y, randomIndex.x].sprite = _crossSprite;
                _isPlayerTurn = true;
                TurnEnd();
            }
        }
        /// <summary>
        /// ゲームを最初から始める
        /// </summary>
        void Restart()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _placedCells = new bool[Size, Size];
                for (var r = 0; r < Size; r++)
                {
                    for (var c = 0; c < Size; c++)
                    {
                        _cells[r, c].color = _normalColor;
                        _cells[r, c].sprite = null;
                    }
                }
                SelectedIndex = Vector2Int.zero;
                _cells[0, 0].color = _selectedColor;
                _isGameEnd = false;
                _isPlayerTurn = true;
            }
        }
        private void Update()
        {
            if (_isGameEnd)
            {   //  ゲーム終了後、左クリックをすると最初から始まる
                Restart();
                return;
            }
            
            if (_isPlayerTurn)
            {
                MoveSelect();
                PlayerPlace();
            }
            else
            {
                EnemyPlace();
            }
        }
        /// <summary>
        /// 勝利判定
        /// </summary>
        void TurnEnd()
        {
            bool isFilled = true;
            for (int i = 0; i < _victoryPattern.GetLength(0); i++)
            {
                int circlePoint = 0;
                int crossPoint = 0;
                for (int j = 0; j < Size; j++)
                {
                    var index = _victoryPattern[i, j];
                    var image = _cells[index / Size, index % Size];
                    if (image.sprite == _circleSprite)
                    {
                        circlePoint++;
                    }
                    else if(image.sprite == _crossSprite)
                    {
                        crossPoint++;
                    }
                    else
                    {
                        isFilled = false;
                    }
                }

                if (circlePoint == Size)
                {
                    GameEnd(GameEndType.CircleWin);
                    return;
                }
                if (crossPoint == Size)
                {
                    GameEnd(GameEndType.CrossWin);
                    return;
                }
            }

            if (isFilled)
            {
                GameEnd(GameEndType.Draw);
            }
        }
        void GameEnd(GameEndType type)
        {
            switch (type)
            {
                case GameEndType.CircleWin:
                    Debug.Log("Circle Win");
                    break;
                case GameEndType.CrossWin:
                    Debug.Log("Cross Win");
                    break;
                case GameEndType.Draw:
                    Debug.Log("Draw");
                    break;
            }

            Debug.Log("左クリックでリスタート");
            _isGameEnd = true;
        }

        /// <summary>
        /// 勝利パターン作成
        /// </summary>
        void CreateVictoryPattern()
        {
            //  ここでは2次元配列を1次元配列として見た際の数字を入れている
            //  1次元配列で見た際のインデックス番号を2次元配列でのインデックス番号に変換する際は、
            //  rows : index / Size, column : index % Size
            //  で変換することができる
            
            //  行と列、斜め2方向の勝利パターンを作成する
            _victoryPattern = new int[Size * 2 + 2, Size];
            //  行の勝利パターン
            int rowVictoryIndex = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _victoryPattern[i, j] = rowVictoryIndex;
                    rowVictoryIndex++;
                }
            }
            //  列の勝利パターン
            int colVictoryIndex = 0;
            for (int i = Size; i < Size * 2; i++)
            {
                var subIndex = colVictoryIndex;
                for (int j = 0; j < Size; j++)
                {
                    _victoryPattern[i, j] = subIndex;
                    subIndex += Size;
                }

                colVictoryIndex++;
            }
            //  斜め右下の勝利パターン
            int diagonalRightVictoryIndex = 0;
            for (int i = 0; i < Size; i++)
            {
                _victoryPattern[Size * 2, i] = diagonalRightVictoryIndex;
                diagonalRightVictoryIndex += Size + 1;
            }
            //  斜め左下の勝利パターン
            int diagonalLeftVictoryIndex = Size - 1;
            for (int i = 0; i < Size; i++)
            {
                _victoryPattern[Size * 2 + 1, i] = diagonalLeftVictoryIndex;
                diagonalLeftVictoryIndex += Size - 1;
            }
        }

        int MiniMax(int depth, int nodeIndex, bool isMax, int[] scores, int h)
        {
            if (depth == h)
            {
                return scores[nodeIndex];
            }

            if (isMax)
            {
                return Mathf.Max(MiniMax(depth+1, nodeIndex*2, false, scores, h), 
                    MiniMax(depth+1, nodeIndex*2 + 1, false, scores, h));
            }
            else
            {
                return Mathf.Min(MiniMax(depth+1, nodeIndex*2, true, scores, h),
                    MiniMax(depth+1, nodeIndex*2 + 1, true, scores, h));
            }
        }
    }
}