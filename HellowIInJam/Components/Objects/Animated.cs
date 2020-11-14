using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Objects
{
    public struct Animated
    {
        public enum Directions { Down,Up,Left,Right,Idle,AttackLeft, AttackRight, AttackTop, AttackDown }

        public Directions Direction;
        public Dictionary<String, Point[]> Animations;
        public float MaxDelayAnimation;
        public int ActualAnimationIndex;
        public Point[] Sources;
        public float ActualDelay;
        public bool EndReached;
    }
}
