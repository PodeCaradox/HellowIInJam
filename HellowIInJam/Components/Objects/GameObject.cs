using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace HellowIInJam.Components.Objects
{
    public struct GameObject
    {
        public int ID;
        public Body PlayerBody;
        public Rectangle SourceRect;
        public Vector2 Offset;
        public float LayerDepth;
        public Vector2 Velocity;
        public Vector2 AnimationOffset;
        public float keinSound;
    }
}
