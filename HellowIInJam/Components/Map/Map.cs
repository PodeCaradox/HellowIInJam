using DefaultEcs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Map
{
    public struct Map
    {
        public Point Size;
        public Entity[] Tiles;
        public int TilesToDraw;
        public List<Entity> ToDraw;
    }
}
