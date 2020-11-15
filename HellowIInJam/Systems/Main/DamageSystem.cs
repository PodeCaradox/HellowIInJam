using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Helper.Main;
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
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Down.ToString());
                    entity.Remove<Damage>();
                    entity.Get<GameObject>().PlayerBody.BodyType = tainicom.Aether.Physics2D.Dynamics.BodyType.Dynamic;
                    entity.Get<Player>().Color = Color.White;
                    entity.Get<Player>().Demonized = 0;
                    entity.Get<Player>().Invertiert = false;
                    entity.Get<Player>().Transformed = false;
                    entity.Get<Player>().werwolfTimer = 0;
                    var room1 = _world.GetEntities().With<FirstRoom>().AsSet().GetEntities()[0];
                    entity.Get<GameObject>().PlayerBody.Position = room1.Get<Room>().Tiles[350].Get<MapTile>().Position;
                    player.playerLives = 1;
                }
                return;
            } 
            player.playerLives--;
           
            if (player.playerLives == 0 && animated.Direction != Animated.Directions.Die)
            {
                if (!damage.SoundPlayed)
                {
                    if (player.Transformed) SoundHelper.PlaySound("PlayerDeath(Wolf)");
                    else SoundHelper.PlaySound("PlayerDeath(Mensch)");
                }
                
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
