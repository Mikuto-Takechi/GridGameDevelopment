using System;
using UnityEngine;

namespace Minesweeper
{
    [Serializable]
    public struct SpriteData
    {
        [SerializeField] private Sprite _close;
        [SerializeField] private Sprite _checkedClose;
        [SerializeField] private Sprite _openNone;
        [SerializeField] private Sprite _openMine;
        [SerializeField] private Sprite _openOne;
        [SerializeField] private Sprite _openTwo;
        [SerializeField] private Sprite _openThree;
        [SerializeField] private Sprite _openFour;
        [SerializeField] private Sprite _openFive;
        [SerializeField] private Sprite _openSix;
        [SerializeField] private Sprite _openSeven;
        [SerializeField] private Sprite _openEight;

        public Sprite Close => _close;
        public Sprite CheckedClose => _checkedClose;
        public Sprite OpenNone => _openNone;
        public Sprite OpenMine => _openMine;
        public Sprite OpenOne => _openOne;
        public Sprite OpenTwo => _openTwo;
        public Sprite OpenThree => _openThree;
        public Sprite OpenFour => _openFour;
        public Sprite OpenFive => _openFive;
        public Sprite OpenSix => _openSix;
        public Sprite OpenSeven => _openSeven;
        public Sprite OpenEight => _openEight;
    }
}