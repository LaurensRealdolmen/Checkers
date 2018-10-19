using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Game
{
    public class Piece
    {
        public Piece(PieceColor color)
        {
            Color = color;
        }

        public PieceColor Color { get; set; }

    }

    public enum PieceColor
    {
        Black, White
    }
}
