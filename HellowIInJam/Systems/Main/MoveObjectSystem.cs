using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using HellowIInJam.Helper.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Main
{

    [With(typeof(MoveAndSlide))]
    internal sealed class MoveObjectSystem : AEntitySystem<float>
    {
        private readonly World _world;

        internal MoveObjectSystem(World world)
             : base(world)
        {
            _world = world;
        }





        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref var gameObject = ref entity.Get<GameObject>();
            ref var moveAndSlide = ref entity.Get<MoveAndSlide>();
            ref var animated = ref entity.Get<Animated>();

            if (moveAndSlide.Index % 100 < 50)
            {
                gameObject.PlayerBody.ApplyForce(new Vector2(1000000, 0));
                if(animated.Direction != Animated.Directions.Right)
                {
                    animated.Direction = Animated.Directions.Right;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Right.ToString());
                }
            }
            else
            {
                gameObject.PlayerBody.ApplyForce(new Vector2(-1000000, 0));
                if (animated.Direction != Animated.Directions.Left)
                {
                    animated.Direction = Animated.Directions.Left;
                    animated.Sources = animated.Animations.GetValueOrDefault(Animated.Directions.Left.ToString());
                }
            }

        
          
            moveAndSlide.Index++;

        }
    }
}
