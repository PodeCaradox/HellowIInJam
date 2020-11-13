
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

namespace HellowIInJam
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;



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

            dummy = Content.Load<Texture2D>("dummy");


            InitEntitys();
            InitStaticClasses();



            base.Initialize();
        }

        private void InitSystems()
        {
            mainSystems = new SequentialSystem<float>(new CameraSystem(_world), new MapSystem(_world), new PlayerInputSystem(_world),new AnimationSystem(_world));
            drawSystems = new SequentialSystem<float>(new DrawMapSystem(_spriteBatch, _world), new DrawGameObjectSystem(_spriteBatch, _world));
        }

        private void InitStaticClasses()
        {

            CameraHelper.Init(_world);
            PosTransformer.Init(_world);

            //MapHelper.Init(_world);
        }

        private void InitEntitys()
        {
            #region GameConfig

            _gameConfig = _world.CreateEntity();
            _gameConfig.Set(new GameConfig(
                TileSize: new Point(16, 16)
                ));
            #endregion

            #region Camera

            _camera = _world.CreateEntity();
            _camera.Set(new Camera
            {
                MaxZoomOut = 0.4f,
                ZoomSteps = 0.2f,
                Zoom = 2.0f,
                Bounds = _graphics.GraphicsDevice.Viewport.Bounds,
                CameraPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2, _graphics.GraphicsDevice.Viewport.Bounds.Height / 2),
                CameraSpeed = 1f,
                ZoomChanged = true,
                Changed = true
            });

            #endregion
            MapHelper.SaveMap("Dummy");
            MapHelper.LoadMap("Dummy", _world, Content);

            #region Player

            _player = _world.CreateEntity();
            _player.Set(new GameObject()
            {
                Position = new Vector2(),
                SourceRect = new Rectangle(0, 0, 16, 32)
            });

            _player.Set(new Player()
            {
                Speed = 1,
            });
            _player.Set(new TextureShared
            {
                TextureSheet = Content.Load<Texture2D>("player")
            }
            );

            _player.Set(new Animated()
            {
                AnimationChangeTimer = 80.5f,
                ElapsedTime = 0,
                Index = 0,
                MaxIndex = 7
                

            });


            #endregion

            #region Enemys

            _enemy = _world.CreateEntity();
            _enemy.Set(new GameObject()
            {
                Position = new Vector2(),
                SourceRect = new Rectangle(0, 0, 16, 32)
            });


            _enemy.Set(new Animated()
            {
                AnimationChangeTimer = 80.5f,
                ElapsedTime = 0,
                Index = 0,
                MaxIndex = 1
            });

            _enemy.Set(new TextureShared
            {
                TextureSheet = Content.Load<Texture2D>("enemy")
            }
            );


            #endregion

            #region Sound

            _sound = _world.CreateEntity();
            _sound.Set(new Sound());
            #endregion


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
            waveOut.Play();

            ref var sound = ref _sound.Get<Sound>();
            sound.ProcessorStream = processorStream;
            sound.WaveOut = waveOut;


            


        }

     
       

        private void ResetStates()
        {
            ref var map = ref _world.GetEntities().With<Map>().AsSet().GetEntities()[0].Get<Map>();

            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            ref var camera = ref _camera.Get<Camera>();
            camera.CameraPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2, map.Size.Y * gameConfig.TileSize.Y - _graphics.GraphicsDevice.Viewport.Bounds.Height / 2);

            ref var player = ref _player.Get<GameObject>();
            player.Position = new Vector2(map.Size.X/2 * gameConfig.TileSize.X - player.SourceRect.Width / 2, map.Size.Y * gameConfig.TileSize.Y - 550);

            Random r = new Random();
            int dummy = map.Size.X / 2 * gameConfig.TileSize.X - player.SourceRect.Width / 2;
            int yOffset = map.Size.Y * gameConfig.TileSize.Y - 550;
            for (int i = 0; i < 1000; i++)
            {
                var enemy = _world.CreateEntity();
                enemy.Set(new GameObject()
                {
                    SourceRect = new Rectangle(0, 0, 16, 32),
                    Position = new Vector2(r.Next(dummy - 400, dummy + 400), yOffset - i * 16)
                });
               
                enemy.SetSameAs<TextureShared>(_enemy);
                enemy.SetSameAs<Animated>(_enemy);


            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mainSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);

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
