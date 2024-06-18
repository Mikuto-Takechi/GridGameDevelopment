using System;

namespace Minesweeper
{
    public struct GridIndex : IEquatable<GridIndex>
    {
        public static GridIndex Zero = new GridIndex(0, 0);
        public static GridIndex Up = new GridIndex(-1, 0);
        public static GridIndex Down = new GridIndex(1, 0);
        public static GridIndex Right = new GridIndex(0, 1);
        public static GridIndex Left = new GridIndex(0, -1);
        public int Row;
        public int Column;
        public GridIndex(int row, int col)
        {
            Row = row;
            Column = col;
        }
        public GridIndex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0 :
                        return this + Up;
                    case 1 :
                        return this + Down;
                    case 2 :
                        return this + Right;
                    case 3 :
                        return this + Left;
                    case 4 :
                        return this + Right + Up;
                    case 5 :
                        return this + Right + Down;
                    case 6 :
                        return this + Left + Up;
                    case 7 :
                        return this + Left + Down;
                    default:
                        return this + Zero;
                }
            }
        }

        public static GridIndex operator +(GridIndex a, GridIndex b)
        {
            return new GridIndex(a.Row + b.Row, a.Column + b.Column);
        }

        public bool Equals(GridIndex other)
        {
            return Row == other.Row && Column == other.Column;
        }
    }
}