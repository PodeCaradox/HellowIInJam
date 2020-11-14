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

           
            if (moveAndSlide.Index % 100 < 50)
            {
                gameObject.PlayerBody.LinearVelocity = new Vector2(100,0);
            }
            else
            {
                gameObject.PlayerBody.LinearVelocity = new Vector2(-100, 0);
            }

        
          
            moveAndSlide.Index++;

        }
    }
}
