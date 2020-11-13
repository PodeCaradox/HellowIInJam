using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Animation
{

    [With(typeof(Animated))]
    internal sealed class AnimationSystem : AEntitySystem<float>
   
    {

        internal AnimationSystem(World world)
             : base(world)
        {
           



        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref var gameObject = ref entity.Get<GameObject>();
            ref var animatedData = ref entity.Get<Animated>();
            animatedData.ElapsedTime += elaspedTime;
            if (animatedData.AnimationChangeTimer < animatedData.ElapsedTime)
            {
                animatedData.ElapsedTime = 0;
                animatedData.Index++;
                if (animatedData.Index > animatedData.MaxIndex) animatedData.Index = 0;
                gameObject.SourceRect.X = animatedData.Index * 16;
            }
        }

    }
}
