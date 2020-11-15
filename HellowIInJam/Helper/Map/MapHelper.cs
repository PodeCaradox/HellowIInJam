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
using HellowIInJam.Components.Objects;
using World = DefaultEcs.World;
using tainicom.Aether.Physics2D.Dynamics;

namespace CastleSim.Systems.HelperClasses
{
    internal static class MapHelper
    {
        private static Entity _map;
        internal static void Init(World world)
        {

        }
        internal static void LoadMap(string mapName,World world,ContentManager Content, tainicom.Aether.Physics2D.Dynamics.World physicsWord)
        {

            ref var gameConfig = ref world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0].Get<GameConfig>();


            var file = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + mapName + ".map");
            var mapData = JsonConvert.DeserializeObject<MapData>(file);
            _map = world.CreateEntity();

            var mapTexture = Content.Load<Texture2D>("dummy");

            Dictionary<int, Entity> objects = InitObjects(world);

          
            var dummy = world.CreateEntity();
            dummy.Set(new TextureShared()
            {
                TextureSheet = mapTexture,
            });

            Random r = new Random();
            var chunks = new Entity[mapData.Size.X  * mapData.Size.Y];
            
            var rooms = world.GetEntities().With<RoomPrefab>().AsSet().GetEntities();

            for (int y = 0; y < mapData.Size.Y; y++)
            {
                for (int x = 0; x < mapData.Size.X ; x++)
                {
                    int i = x + y * mapData.Size.X;
                    
                    chunks[i] = world.CreateEntity();

                    Entity[] tiles = null;
                    List<Entity> gameObjects = new List<Entity>();
                    List<Entity> doors = new List<Entity>();
                    int[] enemys = null;
                    bool visible = false;
                    bool first = false;
                    if (mapData.MapTiles[i] != -1)
                    {
                        visible = true;
                        tiles = new Entity[gameConfig.ChunkSize * gameConfig.ChunkSize];

                        ref var room = ref rooms[mapData.MapTiles[i]].Get<RoomPrefab>();
                        if (mapData.MapTiles[i] == 0) first = true;
                        else first = false;
                         enemys = room.Enemys;
                        for (int yO = 0; yO < gameConfig.ChunkSize; yO++)
                        {
                            for (int xO = 0; xO < gameConfig.ChunkSize; xO++)
                            {
                                int key = yO * gameConfig.ChunkSize + xO;
                                tiles[key] = world.CreateEntity();
                                var pos = new Vector2(x * gameConfig.TileSize.X * gameConfig.ChunkSize + xO * gameConfig.TileSize.X, y * gameConfig.TileSize.Y * gameConfig.ChunkSize + yO * gameConfig.TileSize.Y);
                                tiles[key].Set(new MapTile()
                                {
                                    Position = pos,
                                    TileID = room.Tiles[yO * gameConfig.ChunkSize + xO]
                                });

                                if (room.Tiles[yO * gameConfig.ChunkSize + xO] == 5)
                                {
                                    tiles[key].Set<Trap>();
                                    var collision = physicsWord.CreateRectangle(16, 16, 1f, pos);
                                    collision.BodyType = BodyType.Static;
                                    collision.Mass = 1000;
                                    collision.SleepingAllowed = true;
                                    collision.Tag = tiles[key];
                                    tiles[key].Set(new AnimatedTile() { 
                                    ActualAnimation = 5,
                                    Delay = 80,
                                    WaitingTimeFirstAnimation = r.Next(2050,4000),
                                    IDS = new int[] {5,6,7,8,9 }
                                    });
                                }


                                if (room.Tiles[yO * gameConfig.ChunkSize + xO] == 4 || room.Tiles[yO * gameConfig.ChunkSize + xO] == 2) 
                                {
                                    var collision = physicsWord.CreateRectangle(16 - 12, 12 - 10, 1f, pos + new Vector2(6,4));
                                    collision.BodyType = BodyType.Static;
                                    collision.Mass = 1000;
                                    collision.SleepingAllowed = true;
                                    collision.Tag = tiles[key];
                                    tiles[key].Set<Pit>(); 
                                }


                                tiles[key].SetSameAs<TextureShared>(dummy);

                                if (room.Objects[yO * gameConfig.ChunkSize + xO] != -1)
                                {
                                    var objectEntity = objects[room.Objects[yO * gameConfig.ChunkSize + xO]].CopyTo(world);
                                    objectEntity.Enable();
                                    gameObjects.Add(objectEntity);

                                        var collision = physicsWord.CreateRectangle(16, 16, 1f, new Vector2(x * gameConfig.TileSize.X * gameConfig.ChunkSize + xO * gameConfig.TileSize.X, y * gameConfig.TileSize.Y * gameConfig.ChunkSize + yO * gameConfig.TileSize.Y));
                                        collision.BodyType = BodyType.Static;
                                        collision.Mass = 1000;
                                    collision.SleepingAllowed = true;
                                   
                                        objectEntity.Get<GameObject>().PlayerBody = collision;
                                    objectEntity.Get<GameObject>().ID = room.Objects[yO * gameConfig.ChunkSize + xO];

                                    if (objectEntity.Has<Door>())
                                    {
                                        collision.Tag = objectEntity;
                                        doors.Add(objectEntity);
                                    }



                                    objectEntity.SetSameAs<TextureShared>(dummy);
                                }

                              
                            }
                        }

                 
                    }

                    chunks[i].Set(new Room()
                    {
                        Open = false,
                        Visible = visible,
                        Tiles = tiles,
                        GameObjects = gameObjects,
                        Doors = doors,
                        Enemys1 = enemys,
                        Enemys = new List<Entity>(),
                        Pots = new List<Entity>(),
                    });

                    if (first)
                    {
                        chunks[i].Set<FirstRoom>();
                    }
                    
                  



                    
                }
            }


          

            dummy.Dispose();



            _map.Set(new Map()
            {
                Size = mapData.Size,
                Chunks = chunks,
                ToDraw = new List<Entity>(),
                DefaulObjects = objects

            });
        }

        private static Dictionary<int, Entity> InitObjects(World world)
        {
            var dummy = new Dictionary<int, Entity>();

            #region Waende
            int counter = 0;
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    var gameObject = world.CreateEntity();
                    gameObject.Set(new GameObject()
                    {
                        SourceRect = new Rectangle(0 + x * 16, 16 + y * 16, 16, 16),
                     
                    });
                    gameObject.Disable();


                    dummy.Add(counter, gameObject);
                    counter++;
                }
            }

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    var gameObject = world.CreateEntity();
                

                 
                    gameObject.Set(new GameObject()
                    {
                        SourceRect = new Rectangle(112 + x * 16, 16 + y * 16, 16, 16),
                       
                    });
                    gameObject.Disable();


                    dummy.Add(counter, gameObject);
                    counter++;
                }
            }


            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    var gameObject = world.CreateEntity();


                    if (counter == 57 + 28 || counter == 59 + 28 || counter == 61 + 28 || counter == 66 + 28)
                    {
                        int dir = (counter == 57 + 28) ? 0 : 1;
                        if (counter == 59 + 28) dir = 2;
                        if (counter == 66 + 28) dir = 3;
                        gameObject.Set(new Door()
                        {
                            Direction = dir
                        });

                    }
                    gameObject.Set(new GameObject()
                    {
                        SourceRect = new Rectangle(112 + x * 16, 80 + y * 16, 16, 16),

                    });
                    gameObject.Disable();


                    dummy.Add(counter, gameObject);
                    counter++;
                }
            }
            #endregion




            return dummy;
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
                    if(y == 0 && x == 0) map.MapTiles[x + y * map.Size.X] = 0;
                    else if(y == 0 && x == map.Size.X - 1) map.MapTiles[x + y * map.Size.X] = 1;
                    else if (y == map.Size.Y - 1 && x == 0) map.MapTiles[x + y * map.Size.X] = 2;
                    else if (y == map.Size.Y - 1 && x == map.Size.X - 1) map.MapTiles[x + y * map.Size.X] = 3;
                   
                    else if (y == 0) map.MapTiles[x + y * map.Size.X] = 4;
                    else if (x == 0) map.MapTiles[x + y * map.Size.X] = 6;
                    else if (y == map.Size.Y - 1) map.MapTiles[x + y * map.Size.X] = 7;
                    else if (x == map.Size.Y - 1) map.MapTiles[x + y * map.Size.X] = 5;
                    
                    else map.MapTiles[x + y * map.Size.X] = 8;
                }
            }

           
            var mapData = JsonConvert.SerializeObject(map, Formatting.Indented);
            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + map.Name + ".map", mapData);

        }
        static Random r = new Random();
        internal static void SaveRoom(string roomname,int chunksize,bool left,bool down,bool right,bool up)
        {
            MapData map = new MapData();
            map.Name = roomname;

            map.ID = 0;
            map.Size = new Point(1, 1);
            map.MapTiles = new int[chunksize * chunksize];
            map.ObjectLayer = new int[chunksize * chunksize];
            #region Tiles
            for (int y = 0; y < chunksize; y++)
            {
                for (int x = 0; x < chunksize; x++)
                {
                    map.MapTiles[x + y * chunksize] = 1;
                }
            }
            #endregion

            #region Enemys
            map.Enemys = new int[chunksize * chunksize];
            for (int y = 0; y < chunksize; y++)
            {
                for (int x = 0; x < chunksize; x++)
                {
                    map.Enemys[x + y * chunksize] = -1;
                }
            }

            #endregion

            #region Walls
            for (int y = 0; y < chunksize; y++)
            {
                for (int x = 0; x < chunksize; x++)
                {
                    map.ObjectLayer[x + y * chunksize] = -1;

                    if (y < 2 || y >= chunksize - 2 || x < 2 || x >= chunksize - 2)
                    {
                        #region Linke Kante Oben
                        if (x == 0 && y == 0) map.ObjectLayer[x + y * chunksize] = 0;
                        else if (x == 1 && y == 0) map.ObjectLayer[x + y * chunksize] = 1;
                        else if (x == 0 && y == 1) map.ObjectLayer[x + y * chunksize] = 7;
                        else if (x == 1 && y == 1) map.ObjectLayer[x + y * chunksize] = 8;

                        #endregion

                        #region Rechte Kante Oben
                        if (x == chunksize - 1 && y == 0) map.ObjectLayer[x + y * chunksize] = 6;
                        else if (x == chunksize - 2 && y == 0) map.ObjectLayer[x + y * chunksize] = 5;
                        else if (x == chunksize - 1 && y == 1) map.ObjectLayer[x + y * chunksize] = 13;
                        else if (x == chunksize - 2 && y == 1) map.ObjectLayer[x + y * chunksize] = 12;
                        #endregion

                        #region Linke Kante Unten
                        if (x == 0 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 42;
                        else if (x == 1 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 43;
                        else if (x == 0 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 35;
                        else if (x == 1 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 36;

                        #endregion

                        #region Rechte Kante Unten
                        if (x == chunksize - 1 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 48;
                        else if (x == chunksize - 2 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 47;
                        else if (x == chunksize - 1 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 41;
                        else if (x == chunksize - 2 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 40;
                        #endregion

                        #region Wände
                        if (x > 1 && x < chunksize - 2 && y == 0) map.ObjectLayer[x + y * chunksize] = 3;
                        else if (x > 1 && x < chunksize - 2 && y == 1) map.ObjectLayer[x + y * chunksize] = 9;

                        else if (y > 1 && y < chunksize - 2 && x == 0) map.ObjectLayer[x + y * chunksize] = 14;
                        else if (y > 1 && y < chunksize - 2 && x == 1) map.ObjectLayer[x + y * chunksize] = 15;

                        else if (y > 1 && y < chunksize - 2 && x == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 20;
                        else if (y > 1 && y < chunksize - 2 && x == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 19;

                        else if (x > 1 && x < chunksize - 2 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 45;
                        else if (x > 1 && x < chunksize - 2 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 38;
                        #endregion

                        #region Türen
                        int breite = chunksize / 2 - 1;
                        int hoehe = chunksize / 2 - 1;

                        #region Links
                        if (left)
                        {
                            if (x == 0 && y == hoehe - 1) map.ObjectLayer[x + y * chunksize] = 49 + 28;
                            else if (x == 1 && y == hoehe - 1) map.ObjectLayer[x + y * chunksize] = 50 + 28;
                            else if (x == 0 && y == hoehe) map.ObjectLayer[x + y * chunksize] = 56 + 28;
                            else if (x == 1 && y == hoehe) map.ObjectLayer[x + y * chunksize] = 57 + 28;
                            else if (x == 0 && y == hoehe + 1) map.ObjectLayer[x + y * chunksize] = 63 + 28;
                            else if (x == 1 && y == hoehe + 1) map.ObjectLayer[x + y * chunksize] = 64 + 28;
                        }


                        #endregion

                        #region Oben
                        if (up)
                        {
                            if (x == breite - 1 && y == 0) map.ObjectLayer[x + y * chunksize] = 51 + 28;
                            else if (x == breite - 1 && y == 1) map.ObjectLayer[x + y * chunksize] = 58 + 28;
                            else if (x == breite && y == 0) map.ObjectLayer[x + y * chunksize] = 52 + 28;
                            else if (x == breite && y == 1) map.ObjectLayer[x + y * chunksize] = 59 + 28;
                            else if (x == breite + 1 && y == 0) map.ObjectLayer[x + y * chunksize] = 53 + 28;
                            else if (x == breite + 1 && y == 1) map.ObjectLayer[x + y * chunksize] = 60 + 28;
                        }
                        #endregion

                        #region Rechts
                        if (right)
                        {
                            if (x == chunksize - 1 && y == hoehe - 1) map.ObjectLayer[x + y * chunksize] = 55 + 28;
                            else if (x == chunksize - 2 && y == hoehe - 1) map.ObjectLayer[x + y * chunksize] = 54 + 28;
                            else if (x == chunksize - 1 && y == hoehe) map.ObjectLayer[x + y * chunksize] = 62 + 28;
                            else if (x == chunksize - 2 && y == hoehe) map.ObjectLayer[x + y * chunksize] = 61 + 28;
                            else if (x == chunksize - 1 && y == hoehe + 1) map.ObjectLayer[x + y * chunksize] = 69 + 28;
                            else if (x == chunksize - 2 && y == hoehe + 1) map.ObjectLayer[x + y * chunksize] = 68 + 28;
                        }
                        #endregion

                        #region Unten
                        if (down)
                        {
                            if (x == breite - 1 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 72 + 28;
                            else if (x == breite - 1 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 65 + 28;
                            else if (x == breite && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 73 + 28;
                            else if (x == breite && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 66 + 28;
                            else if (x == breite + 1 && y == chunksize - 1) map.ObjectLayer[x + y * chunksize] = 74 + 28;
                            else if (x == breite + 1 && y == chunksize - 2) map.ObjectLayer[x + y * chunksize] = 67 + 28;
                        }
                        #endregion
                        #endregion


                    }



                }
            }
            #endregion



            var mapData = JsonConvert.SerializeObject(map,Formatting.Indented);
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
                    Tiles = roomData.MapTiles,
                    Objects = roomData.ObjectLayer,
                    Enemys = roomData.Enemys
                });
            }
           
        }
    }
}
