using DefaultEcs;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json;
using System;
using System.IO;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Main;
using CastleSim.Json;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CastleSim.Systems.HelperClasses
{
    internal static class MapHelper
    {
        private static Entity _map;
        private static Entity _gameConfig;
        private static World _world;
        private static Random rnd;
        internal static void Init(World world)
        {
            _gameConfig = world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0];
            _world = world;
            rnd = new Random();
           
        }
        internal static void LoadMap(string mapName,World world)
        {

            ref var gameConfig = ref world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0].Get<GameConfig>();


            var file = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + mapName + ".map");
            var mapData = JsonConvert.DeserializeObject<MapData>(file);
            _map = world.CreateEntity();


            Entity[] tiles = new Entity[mapData.MapTiles.Length];
            for (int i = 0; i < mapData.MapTiles.Length; i++)
            {
                tiles[i] = world.CreateEntity();
                tiles[i].Set(new MapTile()
                {
                    Position = new Vector2(i % mapData.Size.X * gameConfig.TileSize.X, i / mapData.Size.Y * gameConfig.TileSize.Y),
                    TileID = mapData.MapTiles[i]
                });

            }



            _map.Set(new Map()
            {
                Size = mapData.Size,
                Tiles = tiles,
                ToDraw = new List<Entity>()

            });
        }

        internal static void SaveMap(string mapName)
        {
            MapData map = new MapData();
            map.Name = mapName;

            map.Size = new Point(100,100);
            map.MapTiles = new int[map.Size.X * map.Size.Y];
            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    map.MapTiles[x + y * map.Size.X] = 0;
                }
            }

           
            var mapData = JsonConvert.SerializeObject(map);
            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + map.Name + ".map", mapData);

        }


    }
}
