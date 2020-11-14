using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Map
{
    public struct AnimatedTile
    {
        public int ID;
        public Point[] Sources;
        public float Delay;
        public int ActualAnimation;
        public float ActualDelay;
    }
}
