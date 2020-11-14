using DefaultEcs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Map
{
    public struct Room
    {
        public int openingDirection;
        public bool Visible;
        public Entity[] Tiles;
        public List<Entity> GameObjects;
    }
}
