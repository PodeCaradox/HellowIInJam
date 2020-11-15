using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Helper.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Animation
{
  
   [With(typeof(AnimatedTile))]
    internal sealed class TileAnimationSystem : AEntitySystem<float>

    {

        internal TileAnimationSystem(World world)
             : base(world)
        {




        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            
            ref var animatedTile = ref entity.Get<AnimatedTile>();
            ref var tile = ref entity.Get<MapTile>();
            ref var trap = ref entity.Get<Trap>();
            if (!trap.RoomActivated) return;
            animatedTile.ActualDelay += elaspedTime;

            if (animatedTile.ActualAnimation == 0)
            {
                trap.Activ = false;
                if (animatedTile.ActualDelay > animatedTile.WaitingTimeFirstAnimation)
                {
                    animatedTile.ActualDelay = 0;
                    animatedTile.ActualAnimation++;
                    tile.TileID = animatedTile.IDS[animatedTile.ActualAnimation];
                }
            }
            else
            {
                if (animatedTile.ActualAnimation == 2) { 
                    trap.Activ = true;
                    
                }
                else trap.Activ = false;
                if (animatedTile.ActualDelay > animatedTile.Delay)
                {
                    animatedTile.ActualDelay = 0;
                    animatedTile.ActualAnimation++;
                    if(animatedTile.ActualAnimation == 2) SoundHelper.PlaySound("Spikes");
                    if (animatedTile.IDS.Length - 1 < animatedTile.ActualAnimation) animatedTile.ActualAnimation = 0;
                    tile.TileID = animatedTile.IDS[animatedTile.ActualAnimation];
                }
            }
        }

    }
}
