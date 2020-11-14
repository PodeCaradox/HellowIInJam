
using CastleSim.Systems;
using CastleSim.Systems.HelperClasses;
using CastleSim.Systems.Update;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Main;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Components.Shared;
using HellowIInJam.Main.Components;
using HellowIInJam.Systems.Animation;
using HellowIInJam.Systems.Draw;
using HellowIInJam.Systems.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NAudio.Wave;
using SoundTouch;
using System;
using System.IO;
using System.Threading;
using SoundTouch.Net.NAudioSupport;
using HellowIInJam.Components.Sound;
using HellowIInJam.CustomClasses;
using System.Collections.Generic;
using SharpMath2;
using HellowIInJam.Helper.Main;
using tainicom.Aether.Physics2D.Dynamics;
using World = DefaultEcs.World;

namespace HellowIInJam
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;
        private tainicom.Aether.Physics2D.Dynamics.World _physicsWord;


        #region Entitys

        private Entity _camera;
        private Entity _gameConfig;
        private Entity _player;
        private Entity _enemy;
        private Entity _sound;
        #endregion

        #region Systems

        SequentialSystem<float> mainSystems;
        SequentialSystem<float> drawSystems;

        #endregion

        private Texture2D dummy;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferMultiSampling = true;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.SynchronizeWithVerticalRetrace = true;

            _graphics.ApplyChanges();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(16);
            //Wen man Borderless angibt  wird grafikarte nich voll benutzt
            this.Window.IsBorderless = false;
            this.Window.Position = new Point(0, 0);


            _world = new World();
            _physicsWord = new tainicom.Aether.Physics2D.Dynamics.World();
            _physicsWord.Gravity = Vector2.Zero;
            dummy = Content.Load<Texture2D>("dummy");


            InitEntitys();
            InitStaticClasses();



            base.Initialize();
        }

        private void InitSystems()
        {
            mainSystems = new SequentialSystem<float>(new PlayerInputSystem(_world), new CameraSystem(_world), new MapSystem(_world), new AnimationSystem(_world),new MoveObjectSystem(_world));
            drawSystems = new SequentialSystem<float>(new DrawMapSystem(_spriteBatch, _world), new DrawGameObjectSystem(_spriteBatch, _world));
        }

        private void InitStaticClasses()
        {

            CameraHelper.Init(_world);
            PosTransformer.Init(_world);
            ChunkHelper.Init(_world);
            //MapHelper.Init(_world);
        }

        private void InitEntitys()
        {
            int chunksize = 20;
            #region GameConfig

            _gameConfig = _world.CreateEntity();
            _gameConfig.Set(new GameConfig(
                TileSize: new Point(16, 16),
                ChunkSize: chunksize
                ));
            #endregion

            #region Camera

            _camera = _world.CreateEntity();
            _camera.Set(new Camera
            {
                MaxZoomOut = 0.4f,
                ZoomSteps = 0.2f,
                Zoom = 3.0f,
                Bounds = _graphics.GraphicsDevice.Viewport.Bounds,
                CameraPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2, _graphics.GraphicsDevice.Viewport.Bounds.Height / 2),
                CameraSpeed = 1f,
                ZoomChanged = true,
                Changed = true
            });

            #endregion

            #region Player


            _player = _world.CreateEntity();

            _player.Set(new GameObject()
            {
                SourceRect = new Rectangle(0, 0, 16, 32)
            });

            _player.Set(new Collision()
            {
                CollisionRect = ShapeUtils.CreateRectangle(16, 16)
            });
          


            _player.Set(new Player()
            {
                Speed = 5f,
                MaxSpeed = 200
            });
            _player.Set(new TextureShared
            {
                TextureSheet = Content.Load<Texture2D>("player")
            }
            );

            var dummy = new Dictionary<String, Point[]>();
            var sources = new Point[]{
                   new Point(0,0),
                   new Point(16, 0),
                   new Point(32, 0),
                   new Point(48, 0),
                   new Point(64, 0),
                   new Point(80, 0),
                   new Point(96, 0),
                   new Point(112, 0),
               };
            dummy.Add(Animated.Directions.Down.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Up.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Left.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Right.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Down.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Up.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Left.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Right.ToString() + "_Wolf", (Point[])sources.Clone());

            sources = new Point[]{
                   new Point(0,0),
                   new Point(16, 0),
                   new Point(32, 0),
                   new Point(48, 0),
                   new Point(64, 0),
                   new Point(80, 0),
                   new Point(96, 0),
                   new Point(112, 0),
               };

            _player.Set(new Animated()
            {
                Animations = dummy,
                Sources = sources,
                MaxDelayAnimation = 80
            });


            #endregion

         

            #region Sound

            _sound = _world.CreateEntity();
            _sound.Set(new Sound());
            #endregion

            for (int i = 0; i < 3; i++)
            {
                MapHelper.SaveRoom("Room" + i, chunksize);
            }
            MapHelper.LoadRooms(_world);
            MapHelper.SaveMap("Dummy");
            MapHelper.LoadMap("Dummy", _world, Content);

          

      
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitSystems();
            ResetStates();

            var audioFile = new WaveFileReader(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Content" + Path.DirectorySeparatorChar + "BgLoop.wav");

            var test = new LoopStream(audioFile) { };


            var processorStream = new SoundTouchWaveStream(test);

            var waveOut = new WaveOut() {  };
          

            waveOut.Init(processorStream);
            //waveOut.Play();

            ref var sound = ref _sound.Get<Sound>();
            sound.ProcessorStream = processorStream;
            sound.WaveOut = waveOut;


        }

     
       

        private void ResetStates()
        {
            ref var map = ref _world.GetEntities().With<Map>().AsSet().GetEntities()[0].Get<Map>();

            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            ref var camera = ref _camera.Get<Camera>();
            camera.CameraPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2, _graphics.GraphicsDevice.Viewport.Bounds.Height / 2);

            ref var player = ref _player.Get<GameObject>();
            player.PlayerBody = _physicsWord.CreateRectangle(8, 1, 1f, new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2, _graphics.GraphicsDevice.Viewport.Bounds.Height / 2));
            player.PlayerBody.BodyType = BodyType.Dynamic;
            player.PlayerBody.Mass = 10;
            player.Offset = new Vector2(4,16);
            // Give it some bounce and friction
            player.PlayerBody.SetRestitution(1f);
            player.PlayerBody.SetFriction(0f);
            player.LayerDepth = PosTransformer.ScreenToDepth(player.PlayerBody.Position);
            #region Enemys
            
            var test = _world.CreateEntity();
            Random r = new Random();
            test.Set(new TextureShared
            {
                TextureSheet = Content.Load<Texture2D>("enemy")
            }
               );
            for (int i = 0; i < 1000; i++)
            {
                var dummy = _physicsWord.CreateRectangle(12, 12, 1f, new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2 + r.Next(1000), _graphics.GraphicsDevice.Viewport.Bounds.Height / 2 + 40 + r.Next(1000)));
                dummy.BodyType = BodyType.Dynamic;
                dummy.Mass = 1000000;
                // Give it some bounce and friction
                dummy.SetRestitution(0f);
                dummy.SetFriction(0f);

                _enemy = _world.CreateEntity();
                _enemy.Set(new GameObject()
                {
                    LayerDepth = PosTransformer.ScreenToDepth(dummy.Position),
                    PlayerBody = dummy,
                    SourceRect = new Rectangle(0, 0, 16, 32),
                    Offset = new Vector2(4, 22)
                });

                _enemy.Set(new Collision()
                {
                    CollisionRect = ShapeUtils.CreateRectangle(16, 16)
                });


                _enemy.Set(new Animated()
                {
                    Sources = new Point[]{
                   new Point(0,0),
                   new Point(16, 0),
               },
                    MaxDelayAnimation = 80,
                   
                    
                });

                _enemy.Set(new MoveAndSlide()
                {
                    Index =  r.Next(100)
                });

                _enemy.SetSameAs<TextureShared>(test);
            }
            test.Dispose();



            #endregion

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mainSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            _physicsWord.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            drawSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);



            base.Draw(gameTime);
        }
    }
}
