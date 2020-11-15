
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Sound
{
    public struct Sound
    {
       public Dictionary<String, SoundEffectInstance>  Instances;
        internal SoundEffectInstance ActivSong;
    }
}
