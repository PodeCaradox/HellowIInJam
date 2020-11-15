using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Sound;
using HellowIInJam.Helper.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Main
{
    [With(typeof(EndBoss))]
    internal sealed class BossBattleSystem : AEntitySystem<float>
    {
        private readonly World _world;
        private readonly Entity _map;
        private readonly Entity _sound;
        private World _physicsWorld;
        private Entity _chunk;
        private bool started = false;
        private Entity _anubis;
        internal BossBattleSystem(World world)
             : base(world)
        {
            _world = world;
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
            _sound = world.GetEntities().With<Sound>().AsSet().GetEntities()[0];
            
        }




      
        protected override void Update(float elaspedTime, in Entity entity)
        {
           
            ref var sound = ref _sound.Get<Sound>();



            
            if (sound.ActivSong.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
            {
                if (_anubis.Equals(default)) { 
                    _anubis = _world.GetEntities().With<Anubis>().AsSet().GetEntities()[0];
                    _chunk = _world.GetEntities().With<EndBoss>().AsSet().GetEntities()[0];
                }
                _anubis.Set<FollowPlayer>();
                _anubis.Remove<MoveAndSlide>();
                started = true;
                SoundHelper.ChangeBackgroundMusic("BossFightLoop");
            }
            


        }
    }
}
