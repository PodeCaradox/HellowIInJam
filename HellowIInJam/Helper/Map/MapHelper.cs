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
using System.Diagnostics;

namespace CastleSim.Systems.HelperClasses
{
    internal static class MapHelper
    {
        private static Entity _map;
        internal static void Init(World world)
        {

        }
        internal static void LoadMap(string mapName,World world,ContentManager Content)
        {

            ref var gameConfig = ref world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0].Get<GameConfig>();


            var file = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + mapName + ".map");
            var mapData = JsonConvert.DeserializeObject<MapData>(file);
            _map = world.CreateEntity();

            var mapTexture = Content.Load<Texture2D>("dummy");

          
            var dummy = world.CreateEntity();
            dummy.Set(new TextureShared()
            {
                TextureSheet = mapTexture,
            });


            var chunks = new Entity[mapData.Size.X  * mapData.Size.Y];
            
            var rooms = world.GetEntities().With<RoomPrefab>().AsSet().GetEntities();

            for (int y = 0; y < mapData.Size.Y; y++)
            {
                for (int x = 0; x < mapData.Size.X ; x++)
                {
                    int i = x + y * mapData.Size.X;
                    
                    chunks[i] = world.CreateEntity();

                    Entity[] tiles = null;
                    bool visible = false;

                    if (mapData.MapTiles[i] != -1)
                    {
                        visible = true;
                        tiles = new Entity[gameConfig.ChunkSize * gameConfig.ChunkSize];

                        ref var room = ref rooms[mapData.MapTiles[i]].Get<RoomPrefab>();

                        for (int yO = 0; yO < gameConfig.ChunkSize; yO++)
                        {
                            for (int xO = 0; xO < gameConfig.ChunkSize; xO++)
                            {

                                tiles[yO * gameConfig.ChunkSize + xO] = world.CreateEntity();
                                tiles[yO * gameConfig.ChunkSize + xO].Set(new MapTile()
                                {
                                    Position = new Vector2(x * gameConfig.TileSize.X * gameConfig.ChunkSize + xO * gameConfig.TileSize.X, y * gameConfig.TileSize.Y * gameConfig.ChunkSize + yO * gameConfig.TileSize.Y),
                                    TileID = room.Tiles[yO * gameConfig.ChunkSize + xO]
                                });



                                tiles[yO * gameConfig.ChunkSize + xO].SetSameAs<TextureShared>(dummy);
                            }
                        }

                 
                    }

                    chunks[i].Set(new Room()
                    {
                        Visible = visible,
                        Tiles = tiles,
                        GameObjects = new List<Entity>()
                    }); ;
                    
                  



                    
                }
            }


          

            dummy.Dispose();



            _map.Set(new Map()
            {
                Size = mapData.Size,
                Chunks = chunks,
                ToDraw = new List<Entity>()

            });
        }

        internal static void SaveMap(string mapName)
        {
            MapData map = new MapData();
            map.Name = mapName;

            map.ID = 0;
            map.Size = new Point(10,10);
            map.MapTiles = new int[map.Size.X * map.Size.Y];

           
            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    map.MapTiles[x + y * map.Size.X] = r.Next(4) - 1;
                }
            }

           
            var mapData = JsonConvert.SerializeObject(map);
            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + map.Name + ".map", mapData);

        }
        static Random r = new Random();
        internal static void SaveRoom(string roomname,int chunksize)
        {
            MapData map = new MapData();
            map.Name = roomname;

            map.ID = 0;
            map.Size = new Point(1, 1);
            map.MapTiles = new int[chunksize * chunksize];

           
            for (int y = 0; y < chunksize; y++)
            {
                for (int x = 0; x < chunksize; x++)
                {
                    int dummy = r.Next(3);
                    map.MapTiles[x + y * chunksize] = 1;
                }
            }


            var mapData = JsonConvert.SerializeObject(map);
            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Dungeons" + Path.DirectorySeparatorChar + map.Name + ".room", mapData);

        }

        internal static void LoadRooms(World world)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Dungeons", "*.room");

            foreach (var filePath in files)
            {
                var file = File.ReadAllText(filePath);
                var roomData = JsonConvert.DeserializeObject<MapData>(file);
                var roomPrefab = world.CreateEntity();
                roomPrefab.Set(new RoomPrefab()
                {
                    ID = roomData.ID,
                    Tiles = roomData.MapTiles
                });
            }
           
        }
    }
}
