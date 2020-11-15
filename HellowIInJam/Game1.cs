
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
using System;
using System.IO;
using System.Threading;
using HellowIInJam.Components.Sound;
using System.Collections.Generic;
using SharpMath2;
using HellowIInJam.Helper.Main;
using tainicom.Aether.Physics2D.Dynamics;
using World = DefaultEcs.World;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace HellowIInJam
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;
        private tainicom.Aether.Physics2D.Dynamics.World _physicsWord;
        private RenderTarget2D game;
        private RenderTarget2D light;
        private Effect LightEffect;
        private float scale;
        #region Entitys

        private Entity _camera;
        private Entity _gameConfig;
        private Entity _player;
        private Entity _sound;
        #endregion
        private SpriteFont _font;
        private Texture2D _viniette;

        #region Systems

        private DrawDarkness darknessSystem;
        SequentialSystem<float> mainSystems;
        SequentialSystem<float> drawSystems;

        #endregion


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize()
        {


            // TODO: Add your initialization logic here
            _graphics.GraphicsProfile = GraphicsProfile.Reach;
            _graphics.PreferMultiSampling = true;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.SynchronizeWithVerticalRetrace = true;

            _graphics.ApplyChanges();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(16);
            //Wen man Borderless angibt  wird grafikarte nich voll benutzt
            this.Window.IsBorderless = false;
            this.Window.Position = new Point(0, 0);


            _world = new World();
            _physicsWord = new tainicom.Aether.Physics2D.Dynamics.World();
            _physicsWord.Gravity = Vector2.Zero;



            InitEntitys();
            InitStaticClasses();

            game = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            light = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            LightEffect = Content.Load<Effect>("lighteffect");
            base.Initialize();
        }

        private void InitSystems()
        {
            mainSystems = new SequentialSystem<float>(new ChangeRoomSystem(_world), new PlayerInputSystem(_world), new CameraSystem(_world), new MapSystem(_world), new AnimationSystem(_world), new MoveObjectSystem(_world), new OpenDoorsSystem(_world), new FollowPlayerSystem(_world), new DamageSystem(_world), new TileAnimationSystem(_world),new BossBattleSystem(_world)); ; ;
            drawSystems = new SequentialSystem<float>(new DrawMapSystem(_spriteBatch, _world));
            darknessSystem = new DrawDarkness(_spriteBatch, _world, Content);
        }

        private void InitStaticClasses()
        {

            CameraHelper.Init(_world);
            PosTransformer.Init(_world);
            ChunkHelper.Init(_world, _physicsWord);
            SoundHelper.Init(_world);
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
                SourceRect = new Rectangle(0, 0, 16, 32),

            });

            _player.Set(new Collision()
            {
                CollisionRect = ShapeUtils.CreateRectangle(16, 16)
            });



            _player.Set(new Player()
            {
                Color = Color.White,
                ChunkBefore = -1,
                Speed = 600000f,
                MaxSpeed = 200,
                playerLives = 1,
                JumpCD = 20000
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
                   new Point(0,224),
                   new Point(32, 224),
                   new Point(64, 224),
                   new Point(92, 224),
                   new Point(128, 224),
                   new Point(160, 224),
                   new Point(192, 224),
               };

            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.AttackDown.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.AttackTop.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.AttackRight.ToString() + "_Wolf", (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.AttackLeft.ToString() + "_Wolf", (Point[])sources.Clone());

            sources = new Point[]{
                   new Point(128 + 0,0),
                   new Point(128 + 16, 0),
                   new Point(128 + 32, 0),
                   new Point(128 + 48, 0),
                   new Point(128 + 64, 0),
                   new Point(128 + 80, 0),
                   new Point(128 + 96, 0),
                   new Point(128 + 112, 0),
               };

            for (int lvl = 1; lvl < 3; lvl++)
            {
                dummy.Add(Animated.Directions.Down.ToString() + lvl, (Point[])sources.Clone());
                for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
                dummy.Add(Animated.Directions.Up.ToString() + lvl, (Point[])sources.Clone());
                for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
                dummy.Add(Animated.Directions.Left.ToString() + lvl, (Point[])sources.Clone());
                for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
                dummy.Add(Animated.Directions.Right.ToString() + lvl, (Point[])sources.Clone());
                for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            }

            sources = new Point[]{
                   new Point(0, 384),
                   new Point(16, 384),
                   new Point(32, 384),
                   new Point(48, 384),
                   new Point(64, 384),
                   new Point(80, 384),
                   new Point(96, 384),
                   new Point(112, 384),
                   new Point(128, 384),
                   new Point(144, 384),
                   new Point(160, 384),
                   new Point(176, 384),
                   new Point(192, 384),
                   new Point(208, 384),
                   new Point(224, 384),
               };


            dummy.Add(Animated.Directions.Die.ToString() + 0, (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Die.ToString() + 1, (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Die.ToString() + 2, (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dummy.Add(Animated.Directions.Die.ToString() + 3, (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;



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


            var dict = new Dictionary<String, SoundEffectInstance>();

            #region Music

            dict.Add("BgLoop", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Musik" + Path.DirectorySeparatorChar + "BgLoop").CreateInstance());
            dict.Add("Monster Mode 1", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Musik" + Path.DirectorySeparatorChar + "Monster Mode").CreateInstance());
            dict.Add("Monster Mode 2", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Musik" + Path.DirectorySeparatorChar + "Monster Mode 2").CreateInstance());
            dict.Add("Monster Mode 3", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Musik" + Path.DirectorySeparatorChar + "Monster Mode 3").CreateInstance());

            #endregion

            #region Effects

            dict.Add("AnubisAngriff", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "AnubisAngriff").CreateInstance());
            dict.Add("AnubisDeath", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "AnubisDeath").CreateInstance());
            dict.Add("AnubisIntro", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "AnubisIntro").CreateInstance());
            dict.Add("BossFightLoop", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "BossFightLoop").CreateInstance());
            dict.Add("FightForMe", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "FightForMe").CreateInstance());
            dict.Add("Level Geschafft", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Level Geschafft").CreateInstance());
            dict.Add("LochFallebMensch", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "LochFallebMensch").CreateInstance());
            dict.Add("LochFallenWerWolf", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "LochFallenWerWolf").CreateInstance());
            dict.Add("MonsterAngriff", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "MonsterAngriff").CreateInstance());
            dict.Add("MonsterDying", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "MonsterDying").CreateInstance());
            dict.Add("MonsterNormal", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "MonsterNormal").CreateInstance());
            dict.Add("PlayerDeath(Mensch)", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "PlayerDeath(Mensch)").CreateInstance());
            dict.Add("PlayerDeath(Wolf)", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "PlayerDeath(Wolf)").CreateInstance());
            dict.Add("Sandlaufeffekt", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Sandlaufeffekt").CreateInstance());
            dict.Add("Schaden", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Schaden").CreateInstance());
            dict.Add("Spikes", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Spikes").CreateInstance());
            dict.Add("Verwandlung", Content.Load<SoundEffect>("Sounds" + Path.DirectorySeparatorChar + "Verwandlung").CreateInstance());
            #endregion

            var sound = dict.GetValueOrDefault("BgLoop");
            sound.IsLooped = true;
            sound.Play();


            _sound = _world.CreateEntity();
            _sound.Set(new Sound()
            {
                Instances = dict,
                ActivSong = sound
            });

            // audioFile.IsLooped = true;
            // audioFile.Play();
            #endregion

            #region Generate Dungeons and Map




            /*
           #region Corners
           MapHelper.SaveRoom("Room" + 0, chunksize, left: false, down: true, right: true, up: false);
           MapHelper.SaveRoom("Room" + 1, chunksize, left: true, down: true, right: false, up: false);
           MapHelper.SaveRoom("Room" + 2, chunksize, left: false, down: false, right: true, up: true);
           MapHelper.SaveRoom("Room" + 3, chunksize, left: true, down: false, right: false, up: true);
           #endregion

           #region Walls
           MapHelper.SaveRoom("Room" + 4, chunksize, left: true, down: true, right: true, up: false);
           MapHelper.SaveRoom("Room" + 5, chunksize, left: true, down: true, right: false, up: true);
           MapHelper.SaveRoom("Room" + 6, chunksize, left: false, down: true, right: true, up: true);
           MapHelper.SaveRoom("Room" + 7, chunksize, left: true, down: false, right: true, up: true);

           MapHelper.SaveRoom("Room" + 8, chunksize, left: true, down: true, right: true, up: true);
           #endregion


           
           MapHelper.SaveRoom("Room" + "Right" , chunksize, left: false, down: false, right: true, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Right" , chunksize, left: false, down: false, right: true, up: true);
           MapHelper.SaveRoom("Room" + "Down" , chunksize, left: false, down: true, right: false, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Down", chunksize, left: false, down: true, right: false, up: true);
           MapHelper.SaveRoom("Room" + "Up" + "Right" , chunksize, left: false, down: true, right: true, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Right" + "Down", chunksize, left: false, down: true, right: true, up: true);
           MapHelper.SaveRoom("Room" + "Left", chunksize, left: true, down: false, right: false, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Left", chunksize, left: true, down: false, right: false, up: true);
           MapHelper.SaveRoom("Room" + "Up" + "Right", chunksize, left: true, down: false, right: true, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Right" + "Left", chunksize, left: true, down: false, right: true, up: true);
           MapHelper.SaveRoom("Room" + "Down" + "Left", chunksize, left: true, down: true, right: false, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Down" + "Left", chunksize, left: true, down: true, right: false, up: true);
           MapHelper.SaveRoom("Room" + "Right" + "Down" + "Left", chunksize, left: true, down: true, right: true, up: false);
           MapHelper.SaveRoom("Room" + "Up" + "Right" + "Down" + "Left", chunksize, left: true, down: true, right: true, up: true);



           MapHelper.LoadRooms(_world);

            */

            MapHelper.SaveRoom("Room" + "Down" + "Right", chunksize, left: false, down: true, right: true, up: false);
            #endregion
            //MapHelper.SaveMap("dummy");
            MapHelper.LoadRooms(_world);
            MapHelper.LoadMap("OverMap", _world, Content, _physicsWord);




        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitSystems();
            ResetStates();


            _viniette = Content.Load<Texture2D>("venen");
            scale = GraphicsDevice.PresentationParameters.BackBufferWidth / _viniette.Width;









        }




        private void ResetStates()
        {

            _font = Content.Load<SpriteFont>("Arial");
            ref var map = ref _world.GetEntities().With<Map>().AsSet().GetEntities()[0].Get<Map>();

            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            ref var camera = ref _camera.Get<Camera>();
            camera.CameraPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Width / 2 + 100, _graphics.GraphicsDevice.Viewport.Bounds.Height / 2);

            var room = _world.GetEntities().With<FirstRoom>().AsSet().GetEntities()[0];

            

            ref var player = ref _player.Get<GameObject>();
            player.PlayerBody = _physicsWord.CreateRectangle(8, 8, 1f, room.Get<Room>().Tiles[350].Get<MapTile>().Position);
            player.PlayerBody.BodyType = BodyType.Dynamic;
            player.PlayerBody.OnCollision += PlayerCollision;
            player.PlayerBody.Tag = _player;
            player.PlayerBody.LinearDamping = 8;
            player.PlayerBody.AngularDamping = 8;
            player.Offset = new Vector2(4, 18);
            // Give it some bounce and friction

            player.LayerDepth = PosTransformer.ScreenToDepth(player.PlayerBody.Position);

            #region Enemys

            var test = _world.CreateEntity();
            Random r = new Random();
            test.Set(new TextureShared
            {
                TextureSheet = Content.Load<Texture2D>("enemy")
            }
               );
            var dict = InitEnemyAnimationsZombie();

            map.Enemys = new List<Entity>();
            //0
            #region Zombie
            Vector2 posi = new Vector2(-100, -400);


            var _enemy = _world.CreateEntity();
            map.Enemys.Add(_enemy);
            _enemy.Disable();
            _enemy.Set<Enemy>();

            _enemy.Set(new GameObject()
            {
                LayerDepth = 0,
                PlayerBody = null,
                SourceRect = new Rectangle(0, 0, 16, 32),
                Offset = new Vector2(4, 12)
            });

            _enemy.Set(new Animated()
            {
                Animations = dict,
                Sources = dict.GetValueOrDefault(Animated.Directions.Right.ToString()),

                MaxDelayAnimation = 90,
            });

            _enemy.Set(new FollowPlayer());

            _enemy.SetSameAs<TextureShared>(test);


            #endregion

            var dict1 = InitEnemyAnimationsMummy();
            //1
            #region Mumie

            Vector2 posi1 = new Vector2(-100, -100);
            var dummy1 = _physicsWord.CreateRectangle(4, 20, 1f, posi);
            dummy1.BodyType = BodyType.Dynamic;
            dummy1.Mass = 100;

            var _enemy1 = _world.CreateEntity();
            map.Enemys.Add(_enemy1);
            _enemy1.Disable();
            _enemy1.Set<Enemy>();

            _enemy1.Set(new GameObject()
            {
                LayerDepth = 0,
                PlayerBody = null,
                SourceRect = new Rectangle(0, 0, 16, 32),
                Offset = new Vector2(4, 12)
            });

            _enemy1.Set(new Animated()
            {
                Animations = dict1,
                Sources = dict1.GetValueOrDefault(Animated.Directions.Right.ToString()),

                MaxDelayAnimation = 90,
            });

            _enemy1.Set(new MoveAndSlide()
            {
                Index = r.Next(100)
            });

            _enemy1.SetSameAs<TextureShared>(test);



            #endregion

            var dict2 = InitAnimationsPot();
            //2
            #region Pot

            Vector2 posi2 = new Vector2(-100, -100);
            var dummy2 = _physicsWord.CreateRectangle(16, 16, 1f, posi);
            dummy2.BodyType = BodyType.Static;
            dummy2.Mass = 100;

            var _enemy2 = _world.CreateEntity();
            map.Enemys.Add(_enemy2);
            _enemy2.Disable();

            _enemy2.Set(new GameObject()
            {
                LayerDepth = 0,
                PlayerBody = null,
                SourceRect = new Rectangle(0, 0, 16, 16)
            });

            _enemy2.Set<Pot>();

            _enemy2.Set(new Animated()
            {
                Animations = dict2,
                Sources = dict2.GetValueOrDefault(Animated.Directions.Idle.ToString()),

                MaxDelayAnimation = 90,
            });


            _enemy2.SetSameAs<TextureShared>(test);

            #endregion

            var dict3 = InitAnimationsAnubis();

            //3
            #region Anubis

            Vector2 posi3 = new Vector2(-100, -100);
            var dummy3 = _physicsWord.CreateRectangle(4, 20, 1f, posi);
            dummy3.BodyType = BodyType.Dynamic;
            dummy3.Mass = 100;

            var _enemy3 = _world.CreateEntity();
            _enemy3.Set<Anubis>(new Anubis() {Lives = 4 });
            map.Enemys.Add(_enemy3);
            _enemy3.Disable();
            _enemy3.Set<Enemy>();

            _enemy3.Set(new GameObject()
            {
                LayerDepth = 0,
                PlayerBody = null,
                SourceRect = new Rectangle(0, 0, 64, 64),
                Offset = new Vector2(4, 12)
            });

            _enemy3.Set(new Animated()
            {
                Animations = dict3,
                Sources = dict3.GetValueOrDefault(Animated.Directions.Idle.ToString() + "Down"),

                MaxDelayAnimation = 10,
            });

            _enemy3.Set(new MoveAndSlide()
            {
                Index = r.Next(100)
            });

            _enemy3.SetSameAs<TextureShared>(test);

            #endregion




            test.Dispose();



            #endregion

        }

        private Dictionary<String, Point[]> InitAnimationsAnubis()
        {
            var dict = new Dictionary<String, Point[]>();
            List<Point> dummy = new List<Point>();
            for (int i = 0; i < 26; i++)
            {
                dummy.Add(new Point(0 + i * 64, 176));
            }
            var sources = dummy.ToArray();
            dict.Add(Animated.Directions.Down.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 64;
            dict.Add(Animated.Directions.Up.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 64;
            dict.Add(Animated.Directions.Left.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 64;
            dict.Add(Animated.Directions.Right.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 64;

            dict.Add(Animated.Directions.Idle.ToString() + "Down", new Point[] { new Point(0, 432) });
            dict.Add(Animated.Directions.Idle.ToString() + "Up", new Point[] { new Point(64, 432) });
            dict.Add(Animated.Directions.Idle.ToString() + "Left", new Point[] { new Point(128, 432) });
            dict.Add(Animated.Directions.Idle.ToString() + "Right", new Point[] { new Point(192, 432) });
            List<Point> dummy1 = new List<Point>();
            for (int i = 0; i < 10; i++)
            {
                dummy1.Add(new Point(0 + i * 64, 496));
            }

            dict.Add("Attack", dummy1.ToArray());
            return dict;
        }

        private Dictionary<String, Point[]> InitAnimationsPot()
        {
            var dict = new Dictionary<String, Point[]>();
            var sources = new Point[]{
                   new Point(0,160),
                   new Point(16, 160),
                   new Point(32, 160),
                   new Point(48, 160),
                   new Point(64, 160),
                   new Point(80, 160),
               };
            dict.Add(Animated.Directions.Idle.ToString(), sources);
            return dict;
        }

        private Dictionary<String, Point[]> InitEnemyAnimationsZombie()
        {
            var dict = new Dictionary<String, Point[]>();
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
            dict.Add(Animated.Directions.Down.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Up.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Left.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Right.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;

            sources = new Point[]{
                   new Point(0,128),
                   new Point(16, 128),
                   new Point(32, 128),
                   new Point(48, 128),
                   new Point(64, 128),
                   new Point(80, 128),
                   new Point(96, 128),
                   new Point(112, 128),
                   new Point(128, 128),
               };
            dict.Add(Animated.Directions.Die.ToString(), (Point[])sources.Clone());


            return dict;
        }

        private bool PlayerCollision(Fixture sender, Fixture other, Contact contact)
        {
            if (sender.Body.Tag == null && other.Body.Tag == null) return true;

            if (other.Body.Tag != null && ((Entity)other.Body.Tag).Has<Door>())
            {
                ref var door = ref ((Entity)other.Body.Tag).Get<Door>();
                if (door.Opnened)
                {
                    ((Entity)sender.Body.Tag).Set(new ChangeChunk()
                    {
                        Direction = door.Direction
                    });
                }

            }
            else if (other.Body.Tag != null && ((Entity)other.Body.Tag).Has<Enemy>())
            {
                Entity player = (Entity)sender.Body.Tag;

                if (player.Get<Player>().isAttacking) return true;
                if (player.Get<Player>().Transformed)
                    player.Get<Player>().Color = Color.Red;



                //SoundHelper.PlaySound("MonsterAngriff");


                if (player.Has<Damage>()) return true;


                //SoundHelper.PlaySound("Schaden");

                player.Set(new Damage()
                {

                });
            }
            else if (other.Body.Tag != null && ((Entity)other.Body.Tag).Has<Pit>())
            {
                Entity player = (Entity)sender.Body.Tag;
                if (player.Get<Player>().isAttacking) return false;

                player.Get<Player>().playerLives = 1;


                if (player.Get<Player>().Transformed) SoundHelper.PlaySound("LochFallenWerWolf");
                else SoundHelper.PlaySound("LochFallebMensch");

                player.Set(new Damage()
                {
                    SoundPlayed = true
                });
            }
            else if (other.Body.Tag != null && ((Entity)other.Body.Tag).Has<Trap>())
            {
                Entity player = (Entity)sender.Body.Tag;
                Entity trap = (Entity)other.Body.Tag;
                if (!trap.Get<Trap>().Activ) return false;
                player.Set(new Damage()
                {

                });
            }
            else if (other.Body.Tag != null && ((Entity)other.Body.Tag).Has<Pot>())
            {
                Entity player = (Entity)sender.Body.Tag;
                Entity pot = (Entity)other.Body.Tag;
                _physicsWord.RemoveAsync(other.Body);
                pot.Disable();
                player.Get<Player>().Demonized = 0;
            }
            return true;

        }

        private Dictionary<String, Point[]> InitEnemyAnimationsMummy()
        {
            var dict = new Dictionary<String, Point[]>();
            var sources = new Point[]{
                   new Point(128 + 0,0),
                   new Point(128 + 16, 0),
                   new Point(128 + 32, 0),
                   new Point(128 + 48, 0),
                   new Point(128 + 64, 0),
                   new Point(128 + 80, 0),
                   new Point(128 + 96, 0),
                   new Point(128 + 112, 0),
               };
            dict.Add(Animated.Directions.Down.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Up.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Left.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;
            dict.Add(Animated.Directions.Right.ToString(), (Point[])sources.Clone());
            for (int i = 0; i < sources.Length; i++) sources[i].Y += 32;


            sources = new Point[]{
                   new Point(144,128),
                   new Point(144 + 16, 128),
                   new Point(144 + 32, 128),
                   new Point(144 + 48, 128),
                   new Point(144 + 64, 128),
                   new Point(144 + 80, 128),
                   new Point(144 + 96, 128),
                   new Point(144 + 112, 128),
                   new Point(144 + 128, 128),
               };
            dict.Add(Animated.Directions.Die.ToString(), (Point[])sources.Clone());
            return dict;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mainSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            _physicsWord.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                activated = !activated;
            }
            base.Update(gameTime);
        }

        bool activated = false;

        protected override void Draw(GameTime gameTime)
        {



            GraphicsDevice.SetRenderTarget(game);
            GraphicsDevice.Clear(Color.Black);
            drawSystems.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);


            GraphicsDevice.SetRenderTarget(light);

            GraphicsDevice.Clear((_player.Get<Player>().Transformed && _player.Get<Player>().Demonized > 0) ? Color.Black : new Color(255, 255, 255, 128));
            darknessSystem.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);


            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            LightEffect.Parameters["LightMask"].SetValue(light);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);


            LightEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(game, Vector2.Zero, Color.White);




            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState: SS_PointBorder);
            if (_player.Get<Player>().Demonized == 3)
            {
                /*
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                
                if (timer > 50)
                {
                    if (timer > 100)
                    {
                        timer = 0;
                    }
                */
             


                
                _spriteBatch.Draw(_viniette, Vector2.Zero, new Rectangle(0, 0, _viniette.Width, _viniette.Height), Color.White, 0, Vector2.Zero, scale: scale, SpriteEffects.None, 1f);
            
                //}
            }

            _spriteBatch.DrawString(_font, "Steuerung: W/A/S/D", new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(_font, "Verwandelung: F", new Vector2(20, 50), Color.White);
            _spriteBatch.DrawString(_font, "Angreifen Wolf Form: Linke Maustaste", new Vector2(20, 80), Color.White);
            _spriteBatch.DrawString(_font, "Objektiv: Pharao umbringen", new Vector2(20, 110), Color.White);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
        float timer = 0;
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
    }
}
