using UnityEngine;

namespace LifeGame
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "LifeGame/ColorData")]
    public class ColorData : ScriptableObject
    {
        [SerializeField] private Color _aliveColor;
        [SerializeField] private Gradient _deadGradient;
        public Color AliveColor => _aliveColor;
        public Gradient DeadGradient => _deadGradient;
    }
}