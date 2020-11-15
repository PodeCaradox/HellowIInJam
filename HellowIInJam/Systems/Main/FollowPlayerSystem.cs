using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using DefaultEcs.System;
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
   
    [With(typeof(FollowPlayer))]
    internal sealed class FollowPlayerSystem : AEntitySystem<float>
    {
        private readonly World _world;
        private readonly Entity _player;

        internal FollowPlayerSystem(World world)
             : base(world)
        {
            _world = world;
            _player = _world.GetEntities().With<Player>().AsSet().GetEntities()[0];
        }


  


        protected override void Update(float elaspedTime, in Entity entity)
        {
            if (entity.Has<Anubis>() && entity.Get<Anubis>().Attacking) return; 
            ref var gameObject = ref entity.Get<GameObject>();
            ref var player = ref _player.Get<GameObject>();
            ref var followPlayer = ref entity.Get<FollowPlayer>();
            ref var animated = ref entity.Get<Animated>();
            gameObject.keinSound += elaspedTime;
            if (followPlayer.cachedposition == Vector2.Zero) {
                Vector2 dir = player.PlayerBody.Position - gameObject.PlayerBody.Position;
                dir.Normalize();
                followPlayer.Direction = dir;
                followPlayer.cachedposition = player.PlayerBody.Position;
            }
            var dummy = Vector2.Distance(gameObject.PlayerBody.Position, followPlayer.cachedposition);
           
            if (dummy < 50) {
                Vector2 dir = player.PlayerBody.Position - gameObject.PlayerBody.Position;
                dir.Normalize();
                followPlayer.Direction = dir;
                followPlayer.cachedposition = player.PlayerBody.Position;
                
                
            }
            if (gameObject.keinSound > 1000) { 
                //SoundHelper.PlaySound("MonsterNormal");
                gameObject.keinSound = 0;
            }
            if (followPlayer.Direction.X > 0.5f)
            {
                if (animated.Direction != Animated.Directions.Right)
                {
                   
                    animated.Direction = Animated.Directions.Right;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Right.ToString());
                }
            }
            else if (followPlayer.Direction.X < -0.5f)
            {
                if (animated.Direction != Animated.Directions.Left)
                {
                    
                    animated.Direction = Animated.Directions.Left;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Left.ToString());
                }
            }
            else if (followPlayer.Direction.Y > 0.5f)
            {
                if (animated.Direction != Animated.Directions.Down)
                {
                    
                    animated.Direction = Animated.Directions.Down;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Down.ToString());
                }
            }
            else if (followPlayer.Direction.Y < -0.5f)
            {
                if (animated.Direction != Animated.Directions.Up)
                {
                   
                    animated.Direction = Animated.Directions.Up;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Up.ToString());
                }
            }



            gameObject.PlayerBody.LinearVelocity = followPlayer.Direction * 50;

            gameObject.LayerDepth = PosTransformer.ScreenToDepth(gameObject.PlayerBody.Position);
        }


    }
}
