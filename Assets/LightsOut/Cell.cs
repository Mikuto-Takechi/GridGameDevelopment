using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public LightsOut LightsOutManager { get; set; }

    public bool CellColor
    {
        get => _switchColor;
        set => _switchColor = value;
    }

    public Image CellImage
    {
        get => _cellImage;
        set => _cellImage = value;
    }

    private Image _cellImage;
    private bool _switchColor;
    public int Row;
    public int Column;
    public Action<int, int> OnClicked;
    private void Awake()
    {
        CellImage = gameObject.AddComponent<Image>();
    }
    public void SwitchColor()
    {
        CellColor = !CellColor;
        CellImage.color = CellColor ? Color.black : Color.white;
    }

    public void ChainSwitchColor()
    {
        SwitchColor();
        OnClicked?.Invoke(Row, Column);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LightsOutManager.State != GameState.Game) return;
        ChainSwitchColor();
    }
}