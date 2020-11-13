using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Main
{
    public struct GameConfig
    {
        public Point TileSize; 
        public Point TileSizeHalf;


        public GameConfig(Point TileSize)
        {
            this.TileSize = TileSize;
            TileSizeHalf = new Point(TileSize.X/2, TileSize.Y/2);

        }
    }
}
