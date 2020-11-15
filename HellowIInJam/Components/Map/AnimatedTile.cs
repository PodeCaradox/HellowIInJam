using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Map
{
    public struct AnimatedTile
    {
        public int ID;
        public int[] IDS;
        public float Delay;
        public int ActualAnimation;
        public float ActualDelay;
        public float SoundDelay;
        public int WaitingTimeFirstAnimation;
    }
}
