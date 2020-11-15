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
using tainicom.Aether.Physics2D.Collision;

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
            gameObject.keinSound += elaspedTime;
            if (player.playerLives == 0) return;

            player.JumpCD += elaspedTime;
                KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            gameObject.PlayerBody.LinearVelocity = new Vector2(0, 0);
            Vector2 force = Vector2.Zero;
            bool moved = false;
            player.isAttacking = false;
            if (animated.Direction != Animated.Directions.AttackLeft &&
                animated.Direction != Animated.Directions.AttackRight &&
                animated.Direction != Animated.Directions.AttackDown &&
                animated.Direction != Animated.Directions.AttackTop)
            {
                
                
                if ((keyboard.IsKeyDown(Keys.W) && (!player.Invertiert || !player.Transformed)) || (keyboard.IsKeyDown(Keys.S) && player.Invertiert && player.Transformed))
                {
                    force = new Vector2(0, -player.Speed);
                    moved = true;

                    CheckAnimation(Animated.Directions.Up, ref animated, player.Transformed, player.Demonized);
                    
                }
                else if ((keyboard.IsKeyDown(Keys.S) && (!player.Invertiert || !player.Transformed)) || (keyboard.IsKeyDown(Keys.W) && player.Invertiert && player.Transformed))
                {
                    force = new Vector2(0, player.Speed);
                    moved = true;
                    CheckAnimation(Animated.Directions.Down, ref animated, player.Transformed, player.Demonized);
                   
                }


                if ((keyboard.IsKeyDown(Keys.D) && (!player.Invertiert || !player.Transformed)) || (keyboard.IsKeyDown(Keys.A) && player.Invertiert && player.Transformed))
                {
                    moved = true;
                    force = new Vector2(player.Speed, 0);
                    CheckAnimation(Animated.Directions.Right, ref animated, player.Transformed, player.Demonized);
                    
                }
                else if ((keyboard.IsKeyDown(Keys.A) && (!player.Invertiert || !player.Transformed)) || (keyboard.IsKeyDown(Keys.D) && player.Invertiert && player.Transformed))
                {
                    moved = true;
                    force = new Vector2(-player.Speed, 0);
                    CheckAnimation(Animated.Directions.Left, ref animated, player.Transformed, player.Demonized);
                   
                }
                /*
                if (keyboard.IsKeyDown(Keys.Space) && keyboardStateBefore.IsKeyUp(Keys.Space) && player.JumpCD > 1000)
                {
                    player.JumpCD = 0;
                    var dummy = force;
                    dummy.Normalize();
                    gameObject.PlayerBody.LinearVelocity += dummy * 10000000;


                }
                */
                #region Idle
                if (moved)
                {
                    if (gameObject.keinSound > 500)
                    {
                        SoundHelper.PlaySound("Sandlaufeffekt");
                        gameObject.keinSound = 0;
                    }
                    //CheckAnimation(Animated.Directions.Left, ref animated, player.Transformed, player.Demonized);
                }
                #endregion


                if (mouse.LeftButton == ButtonState.Pressed && player.Transformed && animated.EndReached)
                {
                    animated.EndReached = false;
                    animated.ActualAnimationIndex = -1;
                    animated.ActualDelay = animated.MaxDelayAnimation + 1;
                    if (animated.Direction == Animated.Directions.Down)
                    {
                        force = new Vector2(0, player.Speed * 5);
                        CheckAnimation(Animated.Directions.AttackDown, ref animated, player.Transformed, player.Demonized,true);
                    }
                    else if (animated.Direction == Animated.Directions.Up)
                    {
                        force = new Vector2(0, -player.Speed * 5);
                        CheckAnimation(Animated.Directions.AttackTop, ref animated, player.Transformed, player.Demonized, true);
                    }
                    else if (animated.Direction == Animated.Directions.Right)
                    {
                        force = new Vector2(player.Speed * 5, 0);
                        CheckAnimation(Animated.Directions.AttackRight, ref animated, player.Transformed, player.Demonized, true);
                    }
                    else if (animated.Direction == Animated.Directions.Left)
                    {
                        force = new Vector2(-player.Speed * 5, 0);
                        CheckAnimation(Animated.Directions.AttackLeft, ref animated, player.Transformed, player.Demonized, true);
                    }
                    else
                    {
                       
                        CheckAnimation(Animated.Directions.AttackDown, ref animated, player.Transformed, player.Demonized, true);
                    }

                    gameObject.AnimationOffset = new Vector2(-8, 0);
                    var dummy = gameObject.SourceRect;
                    dummy.Width = 32;
                    gameObject.SourceRect = dummy;

                }


            }
            else
            {
                int range = 10;
                player.isAttacking = true;
                if (animated.Direction == Animated.Directions.AttackDown)
                {
                    var dummy = gameObject.PlayerBody.Position + new Vector2(0, 10);// new Vector2(8, 10)
                    gameObject.PlayerBody.World.QueryAABB(CallbackQuery,aabb:new AABB(dummy, dummy + new Vector2(range, range)));
                }
                else if (animated.Direction == Animated.Directions.AttackTop)
                {
                    var dummy = gameObject.PlayerBody.Position + new Vector2(0, -20);// new Vector2(8, 10)
                    gameObject.PlayerBody.World.QueryAABB(CallbackQuery, aabb: new AABB(dummy, dummy + new Vector2(range, range)));
                }
                else if (animated.Direction == Animated.Directions.AttackRight)
                {
                    var dummy = gameObject.PlayerBody.Position + new Vector2(8, 0);// new Vector2(8, 10)
                    gameObject.PlayerBody.World.QueryAABB(CallbackQuery, aabb: new AABB(dummy, dummy + new Vector2(range, range)));
                }
                else if (animated.Direction == Animated.Directions.AttackLeft)
                {
                    var dummy = gameObject.PlayerBody.Position + new Vector2(-20, 0);// new Vector2(8, 10)
                    gameObject.PlayerBody.World.QueryAABB(CallbackQuery, aabb: new AABB(dummy, dummy + new Vector2(range, range)));
                }

               
                

                if (animated.ActualAnimationIndex > 1 && animated.ActualAnimationIndex < 5)
                {
                    if (animated.Direction == Animated.Directions.AttackDown)
                    {
                        force *= new Vector2(0, player.Speed * 10);
                    }
                    else if (animated.Direction == Animated.Directions.AttackTop)
                    {
                        force = new Vector2(0, -player.Speed * 10);
                    }
                    else if (animated.Direction == Animated.Directions.AttackRight)
                    {
                        force = new Vector2(player.Speed * 10, 0);
                    }
                    else if (animated.Direction == Animated.Directions.AttackLeft)
                    {
                        force = new Vector2(-player.Speed * 10, 0);
                    }
                }
               

                if (animated.EndReached)
                {
                    gameObject.AnimationOffset = new Vector2(0, 0);
                    animated.ActualAnimationIndex = -1;
                    animated.ActualDelay = animated.MaxDelayAnimation + 1;
                    var dummy = gameObject.SourceRect;
                    dummy.Width = 16;
                    gameObject.SourceRect = dummy;
                    if (animated.Direction == Animated.Directions.AttackDown)
                    {
                        CheckAnimation(Animated.Directions.Down, ref animated, player.Transformed, player.Demonized);
                    }
                    else if (animated.Direction == Animated.Directions.AttackTop)
                    {
                        CheckAnimation(Animated.Directions.Up, ref animated, player.Transformed, player.Demonized);
                    }
                    else if (animated.Direction == Animated.Directions.AttackRight)
                    {
                        CheckAnimation(Animated.Directions.Right, ref animated, player.Transformed, player.Demonized);
                    }
                    else if (animated.Direction == Animated.Directions.AttackLeft)
                    {
                        CheckAnimation(Animated.Directions.Left, ref animated, player.Transformed, player.Demonized);
                    }
                   
                    
                }
            }

            #region Timer
            if (player.Transformed && player.Demonized != 3) player.werwolfTimer += elaspedTime;
            // 4 sekunden
            if (player.werwolfTimer > 5000)
            {
                player.werwolfTimer = 0;
                if (player.Demonized < 3) player.Demonized++;
                if (!player.Transformed)
                    if (player.Demonized == 2) player.Invertiert = true;
               
                ref var sound = ref _sound.Get<Sound>();
                SoundHelper.ChangeBackgroundMusic("Monster Mode " + player.Demonized);
            }
            #endregion



            player.Direction = force;
            gameObject.PlayerBody.ApplyForce(force);
            if (keyboard.IsKeyDown(Keys.F) && keyboardStateBefore.IsKeyUp(Keys.F))
            {
                
                if ( !player.isAttacking) {
                    player.Transformed = !player.Transformed;

                    if (player.Transformed) SoundHelper.PlaySound("Verwandlung");
                    if (!player.Transformed)
                        if (player.Demonized == 2) player.Invertiert = true;
                   

                    if (player.Demonized!= 0)
                    {
                        SoundHelper.ChangeBackgroundMusic("Monster Mode " + player.Demonized);
                       
                    }

                    if (!player.Transformed)
                    {
                        SoundHelper.ChangeBackgroundMusic("BgLoop");
                    
                    }
                   
                    String dummy = animated.Direction.ToString() + ((player.Transformed) ? "_Wolf" : "") + ((!player.Transformed && player.Demonized > 0) ? player.Demonized.ToString() : "");
                    animated.Sources = animated.Animations.GetValueOrDefault(dummy);

                }
              
            }

           

               
            if(component.CameraPosition != gameObject.PlayerBody.Position) component.Changed = true;
            component.CameraPosition = gameObject.PlayerBody.Position;

            gameObject.LayerDepth = PosTransformer.ScreenToDepth(gameObject.PlayerBody.Position);
            var chunk = PosTransformer.ScreenToChunkKKey(gameObject.PlayerBody.Position);
            if(player.ChunkBefore != chunk)
                ChunkHelper.ActivateLight(player.ChunkBefore, chunk);
                player.ChunkBefore = chunk;

           
            keyboardStateBefore = keyboard;

           
                if (ChunkHelper.AllDead(chunk))
                {
                    ChunkHelper.OpenDorrs(chunk);
                }
            
        }

       

        private bool CallbackQuery(tainicom.Aether.Physics2D.Dynamics.Fixture fixture)
        {
            if (fixture.Body.Tag != null)
            {
                if (((Entity)fixture.Body.Tag).Has<Enemy>())
                {
                    var entity = ((Entity)fixture.Body.Tag);
                    if (entity.Has<Anubis>())
                    {
                        entity.Get<Anubis>().Lives--;
                        if (entity.Get<Anubis>().Lives > 0) return false;
                    }

                    SoundHelper.PlaySound("MonsterDying");
                   
                    fixture.Body.IgnoreCCD = true;
                    fixture.Body.World.Remove(fixture.Body);
                    ref var animation = ref entity.Get<Animated>();
                    if (entity.Has<MoveAndSlide>()) entity.Remove<MoveAndSlide>();
                    if (entity.Has<FollowPlayer>()) entity.Remove<FollowPlayer>();
                    animation.ActualAnimationIndex = 0;
                    animation.ActualDelay = 0;
                    animation.Direction = Animated.Directions.Die;
                    animation.Sources = animation.Animations.GetValueOrDefault(Animated.Directions.Die.ToString());
                }
            }
            return true;
        }

        private void CheckAnimation(Animated.Directions animationCurrent,ref Animated animated,bool transformed, int demonLevel,bool isAttack = false)
        {
            if (animated.Direction != animationCurrent || isAttack)
            {

                //animated.ActualDelay = 0;
                //animated.ActualAnimationIndex = 0;
                animated.Direction = animationCurrent;
                animated.Sources = animated.Animations.GetValueOrDefault(animationCurrent.ToString() +( (transformed)?"_Wolf":"") + ((!transformed && demonLevel>0)? demonLevel.ToString() : ""));
            }
        }
    }
}
