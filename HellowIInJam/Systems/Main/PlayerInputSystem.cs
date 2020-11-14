using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Components.Sound;
using HellowIInJam.Helper.Main;
using HellowIInJam.Main.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HellowIInJam.Systems.Main
{
    [With(typeof(Player))]
    public sealed class PlayerInputSystem : AEntitySystem<float>
    {
        private Entity _sound;
        private Entity _camera;
        internal PlayerInputSystem(World world)
             : base(world)
        {
            _sound = world.GetEntities().With<Sound>().AsSet().GetEntities()[0];
            _camera = world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
        }


        KeyboardState keyboardStateBefore = Keyboard.GetState();
        protected override void Update(float elaspedTime, in Entity entity)
        {

            ref var component = ref _camera.Get<Camera>();
            ref var gameObject = ref entity.Get<GameObject>();
            ref var player = ref entity.Get<Player>();
            ref var animated = ref entity.Get<Animated>();

            KeyboardState keyboard = Keyboard.GetState();
            gameObject.PlayerBody.LinearVelocity = new Vector2(0, 0);
            bool gedrueckt = false;
            if (keyboard.IsKeyDown(Keys.W))
            {
                gameObject.Velocity += new Vector2(0, -player.Speed);
                if(gameObject.Velocity.Y < -player.MaxSpeed) gameObject.Velocity = new Vector2(0, -100);

                CheckAnimation(Animated.Directions.Up,ref animated, player.Transformed);
                gedrueckt = true;
            }
            else if (keyboard.IsKeyDown(Keys.S))
            {
                gameObject.Velocity += new Vector2(0, player.Speed);
                if (gameObject.Velocity.Y > player.MaxSpeed) gameObject.Velocity = new Vector2(0, 100);

                CheckAnimation(Animated.Directions.Down, ref animated, player.Transformed);
                gedrueckt = true;
            }
            

            if (keyboard.IsKeyDown(Keys.D))
            {
                gameObject.Velocity += new Vector2(player.Speed, 0);
                if (gameObject.Velocity.X > player.MaxSpeed) gameObject.Velocity = new Vector2(100, 0);
                CheckAnimation(Animated.Directions.Right, ref animated, player.Transformed);
                gedrueckt = true;
            }
            else if (keyboard.IsKeyDown(Keys.A))
            {
              
                gameObject.Velocity += new Vector2(-player.Speed, 0);
                if (gameObject.Velocity.X < -player.MaxSpeed) gameObject.Velocity = new Vector2(-100, 0);
                CheckAnimation(Animated.Directions.Left, ref animated, player.Transformed);
                gedrueckt = true;
            }
            
            if(!gedrueckt)
            {
                if (gameObject.Velocity != Vector2.Zero)
                {
                    if (gameObject.Velocity.X > 0) gameObject.Velocity.X -= player.Speed;
                    else if (gameObject.Velocity.X < 0) gameObject.Velocity.X += player.Speed;
                    else if (gameObject.Velocity.Y > 0) gameObject.Velocity.Y -= player.Speed;
                    else if (gameObject.Velocity.Y < 0) gameObject.Velocity.Y += player.Speed;
                    
                    if (gameObject.Velocity.Y > -player.Speed && gameObject.Velocity.Y < player.Speed && gameObject.Velocity.X > -player.Speed && gameObject.Velocity.X < player.Speed) gameObject.Velocity = Vector2.Zero;
                
                }
               
            }
            
            gameObject.PlayerBody.LinearVelocity += gameObject.Velocity;
            if (keyboard.IsKeyDown(Keys.F) && keyboardStateBefore.IsKeyUp(Keys.F))
            {
                player.Transformed = !player.Transformed;
                animated.Sources = animated.Animations.GetValueOrDefault(animated.Direction.ToString() + ((player.Transformed) ? "_Wolf" : ""));
            }

            keyboardStateBefore = keyboard;
            if(component.CameraPosition != gameObject.PlayerBody.Position) component.Changed = true;
            component.CameraPosition = gameObject.PlayerBody.Position;

            gameObject.LayerDepth = PosTransformer.ScreenToDepth(gameObject.PlayerBody.Position);


        }

        private void CheckAnimation(Animated.Directions animationCurrent,ref Animated animated,bool transformed)
        {
            if (animated.Direction != animationCurrent)
            {

                //animated.ActualDelay = 0;
                //animated.ActualAnimationIndex = 0;
                animated.Direction = animationCurrent;
                animated.Sources = animated.Animations.GetValueOrDefault(animationCurrent.ToString() +( (transformed)?"_Wolf":""));
            }
        }
    }
}
