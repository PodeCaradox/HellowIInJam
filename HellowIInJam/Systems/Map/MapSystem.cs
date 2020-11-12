using DefaultEcs;
using DefaultEcs.System;
using CastleSim.Components;
using CastleSim.Json;
using CastleSim.Systems.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Main;

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

            //number of tiles that will get drawn
            
          
            int counter = 0;
            for (int y = 0; y < cameraData.RowsToDraw; y++)
            {
                int yIndex = (cameraData.Offset.Y + y);
                if (yIndex >= map.Size.Y || yIndex < 0) break;
                for (int x = 0; x < cameraData.ColumnsToDraw; x++)
                {
                    int xOffset = cameraData.Offset.X + x;
                    if (xOffset >= map.Size.X || xOffset < 0) break;

                    if (map.ToDraw.Count <= counter)
                    {
                        map.ToDraw.Add(default);
                    }

                    map.ToDraw[counter] = map.Tiles[xOffset + yIndex * map.Size.X];


                    counter++;
                }
            }
            map.TilesToDraw = counter;


        }

      

        #endregion




    }
 
}
