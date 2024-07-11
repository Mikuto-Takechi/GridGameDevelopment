using UnityEngine;
using UnityEngine.UI;

namespace LifeGame
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Color _aliveColor;
        [SerializeField] private Gradient _deadGradient;
        [SerializeField] private Image _image = null;
        [SerializeField] private CellState _state = CellState.Dead;
        private float _gradientStep = 1f;
        /// <param name="interval">0 ~ 1の範囲</param>
        public void FadeOut(float interval)
        {
            if (_state == CellState.Alive) return;
            // Color.RGBToHSV(_image.color, out float h, out float s, out float v);
            // s -= interval;
            // s = s < 0 ? 0 : s;
            _gradientStep += interval;
            _gradientStep = _gradientStep > 1 ? 1 : _gradientStep;
            _image.color = _deadGradient.Evaluate(_gradientStep); //Color.HSVToRGB(h, s, v);
        }
        public float GradientStep { set => _gradientStep = value; }
        public CellState State
        {
            get => _state;
            set
            {
                //  setする値と現在の値が同じならreturnする。
                if (_state == value) return;

                if (value == CellState.Alive)
                {   //  生存の色に変更する。
                    _image.color = _aliveColor;
                }
                else
                {   //  死んだ場合、グラデーションの先頭の色に変更する。
                    _image.color = _deadGradient.Evaluate(0);
                    _gradientStep = 0;
                }
                _state = value;
            }
        }
    }
}