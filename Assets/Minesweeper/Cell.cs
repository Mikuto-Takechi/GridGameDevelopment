using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minesweeper
{
    [RequireComponent(typeof(Image))]
    public class Cell : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image _view;
        [SerializeField] private CellState _cellState = CellState.None;
        [SerializeField] private SpriteData _spriteData;
        [SerializeField] private bool _isClosed = true;
        [SerializeField] private bool _isChecked = false;
        public Action<GridIndex> OnClicked { get; set; }
        public Action GameOver { get; set; }
        public GridIndex Index { get; set; }
        public bool IsGameOver { get; set; }
        public CellState CellState
        {
            get => _cellState;
            set
            {
                _cellState = value;
                ViewChange();
            }
        }

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                _isClosed = value;
                ViewChange();
            } 
        }
        private void Start()
        {
            ViewChange();
        }
        
        # if UNITY_EDITOR
        private void OnValidate()
        {
            ViewChange();
        }
        #endif

        private void ViewChange()
        {
            if (_view == null) return;
            if (_isClosed)
            {
                _view.sprite = _isChecked ? _spriteData.CheckedClose : _spriteData.Close;
            }
            else
            {
                switch (_cellState)
                {
                    case CellState.Mine :
                        _view.sprite = _spriteData.OpenMine;
                        break;
                    case CellState.None :
                        _view.sprite = _spriteData.OpenNone;
                        break;
                    case CellState.One :
                        _view.sprite = _spriteData.OpenOne;
                        break;
                    case CellState.Two :
                        _view.sprite = _spriteData.OpenTwo;
                        break;
                    case CellState.Three :
                        _view.sprite = _spriteData.OpenThree;
                        break;
                    case CellState.Four :
                        _view.sprite = _spriteData.OpenFour;
                        break;
                    case CellState.Five :
                        _view.sprite = _spriteData.OpenFive;
                        break;
                    case CellState.Six :
                        _view.sprite = _spriteData.OpenSix;
                        break;
                    case CellState.Seven :
                        _view.sprite = _spriteData.OpenSeven;
                        break;
                    case CellState.Eight :
                        _view.sprite = _spriteData.OpenEight;
                        break;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClosed || IsGameOver) return;
            //  開いていないセルを右クリックしたら
            if (eventData.button == PointerEventData.InputButton.Right)
            {   //  チェックを切り替える
                _isChecked = !_isChecked;
                ViewChange();
            }
            else if (eventData.button == PointerEventData.InputButton.Left && !_isChecked)
            {   //  セルを開く
                IsClosed = false;
                if (_cellState == CellState.Mine)
                {
                    GameOver?.Invoke();
                }
                else
                {
                    OnClicked?.Invoke(Index);
                }
            }
        }
    }
}