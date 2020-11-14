using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Objects.Player
{
    public struct Player
    {
        public float ElapsedTime;
        public float ElapsedTimeChangeSpeed;
        public float Speed;
        public bool Transformed;
        public int Demonized;
        public float MaxSpeed;
        public int ChunkBefore;
        public Vector2 Direction;
        public bool Invertiert;
        public Color Color;
        public float werwolfTimer;
    }
}
