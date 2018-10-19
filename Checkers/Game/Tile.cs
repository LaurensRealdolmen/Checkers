using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Game
{
    public class Tile
    {
        public Tile(int x, int y, TileColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public TileColor Color { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is Tile))
            {
                return false;
            }
            else
            {
                var tile = obj as Tile;
                return tile.X == X && tile.Y == Y && tile.Color == Color;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -196163389;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public enum TileColor
    {
        Black, White
    }
}
