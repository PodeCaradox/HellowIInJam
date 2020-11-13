using NAudio.Wave;
using SoundTouch.Net.NAudioSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Components.Sound
{
    public struct Sound
    {
        public WaveOut WaveOut;
        public SoundTouchWaveStream ProcessorStream;
    }
}
