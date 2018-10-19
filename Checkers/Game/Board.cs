using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Game
{
    public class Board
    {
        public Tile[,] Tiles
        {
            get { return _tiles; }
        }

        private Tile[,] _tiles;
        public Board()
        {
            _tiles = new Tile[8, 8];
            var color = TileColor.Black;
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                color = color == TileColor.White ? TileColor.Black : TileColor.White;
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    _tiles[i, j] = new Tile(i, j, color);
                    color = color == TileColor.White ? TileColor.Black : TileColor.White;
                }
            }
        }

        public override string ToString()
        {
            var str = "";
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                if (i > 0)
                    str += "\n";
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    str += _tiles[i,j].Color == TileColor.Black ? "B" : "W";
                }
            }
            return str;
        }
    }
}
