using CastleSim.Components;
using CastleSim.Systems;
using CastleSim.Systems.HelperClasses;
using CastleSim.Systems.Update;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
            mainSystems = new SequentialSystem<float>(new CameraSystem(_world),new MapSystem(_world));
            drawSystems = new SequentialSystem<float>(new DrawMapSystem(_spriteBatch, _world,Content));
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
                TileSize:new Point(16, 16)
                ));
            #endregion

            #region Camera

            _camera = _world.CreateEntity();
            _camera.Set(new Camera
            {
                MaxZoomOut = 0.4f,
                ZoomSteps = 0.2f,
                Zoom = 1f,
                Bounds = _graphics.GraphicsDevice.Viewport.Bounds,
                CameraPosition = new Vector2(200, 200),
                CameraSpeed = 1f,
                ZoomChanged = true,
                Changed = true
            });

            #endregion

            MapHelper.LoadMap("Dummy",_world);



        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitSystems();

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            drawSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);



            base.Draw(gameTime);
        }
    }
}
