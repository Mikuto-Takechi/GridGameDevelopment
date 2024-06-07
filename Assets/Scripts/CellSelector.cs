using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellSelector : MonoBehaviour
{
    [SerializeField, Header("初期セル数")] private int _initialElementCount = 5;
    [SerializeField, Header("セルの配置間隔")] private float _interval = 10f;
    private List<Image> _images = new();
    private int _focusedIndex = 0;
    private void Start()
    {
        for (var i = 0; i < _initialElementCount; i++)
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.parent = transform;

            var newImage = obj.AddComponent<Image>();
            _images.Add(newImage);
        }

        UpdateFocusColor();
    }

    void UpdateFocusColor()
    {
        if (_images.Count <= 0) return;
        foreach (var image in _images)
        {
            image.color = Color.white;
        }
        _images[_focusedIndex].color = Color.red;
    }

    void MoveFocus()
    {
        if (_images.Count <= 0) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // 左キーを押した
        {
            _focusedIndex--;
            if (_focusedIndex < 0)
            {
                _focusedIndex = _images.Count - 1;
            }
            UpdateFocusColor();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // 右キーを押した
        {
            _focusedIndex++;
            _focusedIndex %= _images.Count;
            UpdateFocusColor();
        }

    }

    void DeleteCell()
    {
        if (_images.Count <= 0) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(_images[_focusedIndex].gameObject);
            _images.RemoveAt(_focusedIndex);
            if (_focusedIndex >= _images.Count)
            {
                _focusedIndex = _images.Count - 1;
            }
            UpdateFocusColor();
        }
    }
    private void Update()
    {
        MoveFocus();
        DeleteCell();
    }
}
