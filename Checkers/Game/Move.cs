using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Game
{
    public class Move
    {
        public Move(Tile src, Tile dest, bool overtake)
        {
            Source = src;
            Destination = dest;
            Overtake = overtake;
        }

        public Tile Source { get; set; }
        public Tile Destination { get; set; }
        public bool Overtake { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is Move))
            {
                return false;
            }
            var move = obj as Move;
            return move.Destination.Equals(Destination) && move.Overtake == Overtake && move.Source.Equals(Source);
        }

        public override int GetHashCode()
        {
            var hashCode = 857384843;
            hashCode = hashCode * -1521134295 + EqualityComparer<Tile>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + EqualityComparer<Tile>.Default.GetHashCode(Destination);
            hashCode = hashCode * -1521134295 + Overtake.GetHashCode();
            return hashCode;
        }
    }
}
