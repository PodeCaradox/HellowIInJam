using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Components.Sound;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Main
{
    [With(typeof(Player))]
    public sealed class PlayerInputSystem : AEntitySystem<float>
    {
        private Entity _sound;
        internal PlayerInputSystem(World world)
             : base(world)
        {
            _sound = world.GetEntities().With<Sound>().AsSet().GetEntities()[0];
        }

        

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref var gameObject = ref entity.Get<GameObject>();
            ref var player = ref entity.Get<Player>();
            KeyboardState keyboard = Keyboard.GetState();
            player.ElapsedTime += elaspedTime/1000;
            player.ElapsedTimeChangeSpeed += elaspedTime / 1000;
            if (keyboard.IsKeyDown(Keys.D) && player.ElapsedTime > 0.2)
            {
                player.ElapsedTime = 0;
                gameObject.Position.X += 16;
            }

            if (keyboard.IsKeyDown(Keys.A) && player.ElapsedTime > 0.2)
            {
                player.ElapsedTime = 0;
                gameObject.Position.X -= 16;
            }

            ref var sound = ref _sound.Get<Sound>();
            sound.ProcessorStream.TempoChange += 0.5;
            gameObject.Position.Y -= player.Speed;

            if (player.ElapsedTimeChangeSpeed > 1)
            {
                player.Speed += 0.5f;
                player.ElapsedTimeChangeSpeed = 0;
                ref var animated = ref entity.Get<Animated>();
                animated.AnimationChangeTimer -= 4;
            }


        }
    }
}
