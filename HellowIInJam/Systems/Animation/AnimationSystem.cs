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
            animatedData.ActualDelay += elaspedTime;
            if (animatedData.MaxDelayAnimation < animatedData.ActualDelay)
            {
                animatedData.ActualDelay = 0;
                animatedData.ActualAnimationIndex++;

                if (animatedData.Sources.Length <= animatedData.ActualAnimationIndex) animatedData.ActualAnimationIndex = 0;
                gameObject.SourceRect.Location = animatedData.Sources[animatedData.ActualAnimationIndex];
            }
        }

    }
}
