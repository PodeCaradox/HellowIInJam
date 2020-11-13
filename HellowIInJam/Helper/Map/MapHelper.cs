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
using HellowIInJam.Components.Shared;

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
        internal static void LoadMap(string mapName,World world,ContentManager Content)
        {

            ref var gameConfig = ref world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0].Get<GameConfig>();


            var file = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + mapName + ".map");
            var mapData = JsonConvert.DeserializeObject<MapData>(file);
            _map = world.CreateEntity();

            var mapTexture = Content.Load<Texture2D>("dummy");

            Entity[] tiles = new Entity[mapData.MapTiles.Length];
            var dummy = world.CreateEntity();
            dummy.Set(new TextureShared()
            {
                TextureSheet = mapTexture,
            });

            for (int y = 0; y < mapData.Size.Y; y++)
            {
                for (int x = 0; x < mapData.Size.X; x++)
                {
                    int i = x + y * mapData.Size.X;
                    tiles[i] = world.CreateEntity();
                    tiles[i].Set(new MapTile()
                    {
                        Position = new Vector2(x * gameConfig.TileSize.X, y * gameConfig.TileSize.Y),
                        TileID = mapData.MapTiles[i]
                    });
                    tiles[i].SetSameAs<TextureShared>(dummy);
                }
            }


          

            dummy.Dispose();



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

            map.Size = new Point(150,1000);
            map.MapTiles = new int[map.Size.X * map.Size.Y];

            Random r = new Random();
            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    map.MapTiles[x + y * map.Size.X] = r.Next(3);
                }
            }

           
            var mapData = JsonConvert.SerializeObject(map);
            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + map.Name + ".map", mapData);

        }


    }
}
