using UnityEngine;
using UnityEngine.UI;

public class GridCellSelector : MonoBehaviour
{
    [SerializeField] private int _initialRowCount = 5;
    [SerializeField] private int _initialColumnCount = 5;
    private Image[,] _images;
    private bool[,] _notActiveFlags;
    private Vector2Int _focusedIndex = Vector2Int.zero;
    private GridLayoutGroup _group;
    public Vector2Int FocusedIndex
    {
        get => _focusedIndex;
        set
        {
            if (value.x < 0)
            {
                value.x = _images.GetLength(1) - 1;
            }
            else if (value.y < 0)
            {
                value.y = _images.GetLength(0) - 1;
            }
            value.x %= _images.GetLength(1);
            value.y %= _images.GetLength(0);
            _focusedIndex = value;
        }
    }
    private void Start()
    {
        _group = GetComponent<GridLayoutGroup>();
        _group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _group.constraintCount = _initialColumnCount;
        _images = new Image[_initialRowCount, _initialColumnCount];
        _notActiveFlags = new bool[_initialRowCount, _initialColumnCount];
        for (var r = 0; r < _initialRowCount; r++)
        {
            for (var c = 0; c < _initialColumnCount; c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;

                var image = obj.AddComponent<Image>();
                if (r == 0 && c == 0) { image.color = Color.red; }
                else { image.color = Color.white; }

                _images[r, c] = image;
            }
        }
    }

    void MoveFocus()
    {
        Vector2Int inputVector = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            inputVector.x--;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            inputVector.x++;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            inputVector.y++;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            inputVector.y--;

        if (inputVector != Vector2Int.zero)
        {
            var prevIndex = FocusedIndex;
            FocusedIndex += inputVector;
            int length = inputVector.y != 0 ? _images.GetLength(0) : _images.GetLength(1);
            for (int i = 0; i < length && _notActiveFlags[FocusedIndex.y, FocusedIndex.x]; i++)
            {
                FocusedIndex += inputVector;
            }
            UpdateFocusColor(prevIndex, FocusedIndex);
        }
    }

    void UpdateFocusColor(Vector2Int prev, Vector2Int next)
    {
        if (!_notActiveFlags[prev.y, prev.x])
            _images[prev.y, prev.x].color = Color.white;
        if(!_notActiveFlags[next.y, next.x])
            _images[next.y, next.x].color = Color.red;
    }

    void Delete()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _images[_focusedIndex.y, _focusedIndex.x].color = Color.clear;
            _notActiveFlags[_focusedIndex.y, _focusedIndex.x] = true;
            (int, Vector2Int) minDistance = (int.MaxValue, Vector2Int.zero);
            bool isFound = false;
            for (int i = 0; i < _images.GetLength(0); i++)
            {
                for (int j = 0; j < _images.GetLength(1); j++)
                {
                    if (!_notActiveFlags[i, j])
                    {
                        var distance = _focusedIndex - new Vector2Int(j, i);
                        if (distance.sqrMagnitude < minDistance.Item1)
                        {
                            minDistance = (distance.sqrMagnitude, new Vector2Int(j, i));
                        }

                        isFound = true;
                    }
                }
            }

            if (isFound)
            {
                var prev = FocusedIndex;
                FocusedIndex = minDistance.Item2;
                UpdateFocusColor(prev, FocusedIndex);
            }
            else
            {
                Debug.Log("空いているマスがありません");
            }
        }
    }
    private void Update()
    {
        MoveFocus();
        Delete();
    }
}
