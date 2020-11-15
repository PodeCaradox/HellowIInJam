using DefaultEcs;
using HellowIInJam.Components.Sound;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Helper.Main
{
    internal static class SoundHelper
    {
        private static Entity _sound;
        private static World _world;
        public static void Init(World world)
        {
            _sound = world.GetEntities().With<Sound>().AsSet().GetEntities()[0];
            _world = world;

        }
        public static void ChangeBackgroundMusic(string name)
        {

            ref var sound = ref _sound.Get<Sound>();
            if (sound.ActivSong == sound.Instances.GetValueOrDefault("AnubisIntro")) return;
            sound.ActivSong.Stop();

            sound.ActivSong = sound.Instances.GetValueOrDefault(name);
            sound.ActivSong.IsLooped = true;
            sound.ActivSong.Play();
        }

        public static void ChangeBackgroundMusicWithoudLoop(string name)
        {
            ref var sound = ref _sound.Get<Sound>();
            sound.ActivSong.Stop();
            sound.ActivSong = sound.Instances.GetValueOrDefault(name);
            sound.ActivSong.Play();
        }

        public static void StopBackgroundMusic()
        {
            ref var sound = ref _sound.Get<Sound>();
            sound.ActivSong.Stop();
        }

        public static void PlaySound(String name)
        {
            ref var sound = ref _sound.Get<Sound>();
            sound.Instances.GetValueOrDefault(name).Volume = 0.5f;
            sound.Instances.GetValueOrDefault(name).Stop();
            sound.Instances.GetValueOrDefault(name).Play();

        }

    }
}
