using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Main;
using HellowIInJam.Main.Components;

namespace CastleSim.Systems.Update
{
    public sealed class MapSystem : AComponentSystem<float, Map>
    {
        #region Variabels

        private readonly World _world;
        private readonly Entity _gameConfig;
        private readonly Entity _camera;
        #endregion

        #region Constructor

        public MapSystem(World world)
           : base(world)
        {
            _world = world;
            _gameConfig = _world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0];
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
            //PathFinder.Init(_world);
            //PathfinderHelper.Init(_world);


        }

        #endregion

        #region Methods

        protected override void Update(float elaspedTime, ref Map map)
        {

            ref Camera cameraData = ref _camera.Get<Camera>();
            ref GameConfig gameConfig = ref _gameConfig.Get<GameConfig>();
            
            //number of tiles that will get drawn


            int counter = 0;
            for (int y = 0; y < cameraData.RowsToDraw; y++)
            {
                int yIndex = (cameraData.Offset.Y + y);
                if (yIndex >= map.Size.Y || yIndex < 0) continue;
                for (int x = 0; x < cameraData.ColumnsToDraw; x++)
                {
                    int xOffset = cameraData.Offset.X - x;
                    if (xOffset >= map.Size.X || xOffset < 0) continue;

                    if (map.ToDraw.Count <= counter)
                    {
                        map.ToDraw.Add(default);
                    }

                    if (map.Chunks[xOffset + yIndex * map.Size.X].Get<Room>().Visible)
                    {
                        map.ToDraw[counter] = map.Chunks[xOffset + yIndex * map.Size.X ];


                        counter++;
                    }
                }
            }
            map.ChunksToDraw = counter;


        }

      

        #endregion




    }
 
}
