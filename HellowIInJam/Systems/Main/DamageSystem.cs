using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HellowIInJam.Systems.Main
{
 
  [With(typeof(Damage))]
    internal sealed class DamageSystem : AEntitySystem<float>
    {
        private readonly World _world;

        internal DamageSystem(World world)
             : base(world)
        {
            _world = world;
          
        }





        protected override void Update(float elaspedTime, in Entity entity)
        {

            ref var damage = ref entity.Get<Damage>();
            ref var player = ref entity.Get<Player>();
            damage.time += elaspedTime;
            damage.damageShown += elaspedTime;
            ref var animated = ref entity.Get<Animated>();
            if (player.playerLives == 0) {
                if (animated.EndReached)
                {
                    entity.Disable();
                }
                return;
            } 
            player.playerLives--;
           
            if (player.playerLives == 0 && animated.Direction != Animated.Directions.Die)
            {
                entity.Get<GameObject>().PlayerBody.BodyType = tainicom.Aether.Physics2D.Dynamics.BodyType.Static;
                player.Color = Color.White;
                animated.ActualAnimationIndex = 0;
                animated.ActualDelay = 0;
                
                animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Die.ToString() + player.Demonized);
                if (player.Transformed) { 
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Die.ToString() + 3);
                    player.Color = Color.White;
                }
                animated.Direction = Animated.Directions.Die;
                animated.EndReached = false;
                return;
            }

            

            if (damage.damageShown > 200)
            {
                damage.damageShown = 0;


                if (player.Color == Color.Red)
                {
                    player.Color = Color.White;
                }
                else
                {
                    player.Color = Color.Red;
                }
               
            }

            if (damage.time > 2000)
            {
                entity.Remove<Damage>();
                player.Color = Color.White;
            }
        }
    }
}
