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
        public int ChunksToDraw;
        public Entity[] Chunks;
        public List<Entity> ToDraw;
        
        public Dictionary<int, Entity> DefaulObjects;

        public List<Entity> Enemys;
    }
}
