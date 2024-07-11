using UnityEngine;
using UnityEngine.UI;

namespace LifeGame
{
    public class Cell : MonoBehaviour
    {
        //  セルごとに生存・死のColorやGradientのインスタンスを持っておく必要は無いので、ScriptableObjectから値を参照する。
        [SerializeField] private ColorData _colorData;
        [SerializeField] private Image _image = null;
        CellState _state = CellState.Dead;
        private float _gradientStep = 1f;
        /// <param name="interval">0 ~ 1の範囲</param>
        public void FadeOut(float interval)
        {
            if (_state == CellState.Alive) return;
            _gradientStep += interval;
            _gradientStep = _gradientStep > 1 ? 1 : _gradientStep;
            _image.color = _colorData.DeadGradient.Evaluate(_gradientStep);
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
                    _image.color = _colorData.AliveColor;
                }
                else
                {   //  死んだ場合、グラデーションの先頭の色に変更する。
                    _image.color = _colorData.DeadGradient.Evaluate(0);
                    _gradientStep = 0;
                }
                _state = value;
            }
        }
    }
}