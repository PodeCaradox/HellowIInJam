using CastleSim.Components;
using CastleSim.Systems.HelperClasses;
using CastleSim.Systems.Update;
using DefaultEcs;
using DefaultEcs.System;
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
        private Entity _camera;
        private World _world;

        #region Systems

        SequentialSystem<float> mainSystems;

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
            mainSystems = new SequentialSystem<float>(new CameraSystem(_world));

            InitEntitys();
            InitStaticClasses();

            base.Initialize();
        }

        private void InitStaticClasses()
        {
            CameraHelper.Init(_world);
        }

        private void InitEntitys()
        {
            #region Camera

            _camera = _world.CreateEntity();
            _camera.Set(new Camera
            {
                MaxZoomOut = 0.4f,
                ZoomSteps = 0.2f,
                Zoom = 1f,
                Bounds = _graphics.GraphicsDevice.Viewport.Bounds,
                CameraPosition = new Vector2(0, 0),
                CameraSpeed = 1f,
                ZoomChanged = true,
                Changed = true
            });

            #endregion

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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


            _spriteBatch.Begin(SpriteSortMode.Deferred, transformMatrix: _camera.Get<Camera>().Transform);

            _spriteBatch.Draw(dummy, new Vector2(20, 20), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
