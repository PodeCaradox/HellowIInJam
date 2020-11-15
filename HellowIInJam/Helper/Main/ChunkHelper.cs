using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using World = DefaultEcs.World;

namespace HellowIInJam.Helper.Main
{
    public static class ChunkHelper
    {
        private static Entity _map;
        private static tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;
        private static World _world;
        public static void Init(World world, tainicom.Aether.Physics2D.Dynamics.World PhysicsWorld)
        {
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
            _physicsWorld = PhysicsWorld;
            _world = world;

        }

        public static void Delete(Entity entity, int key)
        {
            ref var mapData = ref _map.Get<Map>();
            mapData.Chunks[key].Get<Room>().GameObjects.Remove(entity);
        }

        public static void Add(Entity entity, int key)
        {
            ref var mapData = ref _map.Get<Map>();
            mapData.Chunks[key].Get<Room>().GameObjects.Add(entity);

        }

        internal static void ActivateLight(int before, int after)
        {
            ref var mapData = ref _map.Get<Map>();
            if (before != -1 && mapData.Chunks[before].Has<Lightet>()) mapData.Chunks[before].Remove<Lightet>();
            mapData.Chunks[after].Set<Lightet>();

         
            for (int i = 0; i < mapData.Chunks[after].Get<Room>().Tiles.Length; i++)
            {
                if (before != -1 && mapData.Chunks[before].Get<Room>().Tiles[i].Has<Trap>()) mapData.Chunks[before].Get<Room>().Tiles[i].Get<Trap>().RoomActivated = false;
                if (mapData.Chunks[after].Get<Room>().Tiles[i].Has<Trap>()) mapData.Chunks[after].Get<Room>().Tiles[i].Get<Trap>().RoomActivated = true;
            }


            if (!mapData.Chunks[after].Get<Room>().Open)
            {
                for (int i = 0; i < mapData.Chunks[after].Get<Room>().Doors.Count; i++)
                {
                    mapData.Chunks[after].Get<Room>().Doors[i].Get<Door>().Opnened = false;
                }

                Random r = new Random();
                for (int i = 0; i < mapData.Chunks[after].Get<Room>().Enemys1.Length; i++)
                {
                    int number = mapData.Chunks[after].Get<Room>().Enemys1[i];

                    var pos =  mapData.Chunks[after].Get<Room>().Tiles[i].Get<MapTile>().Position;

                    if (number == -1) continue;
                    if (number == 3)
                    {
                        Debug.WriteLine(i);
                        SoundHelper.ChangeBackgroundMusicWithoudLoop("AnubisIntro");
                        mapData.Chunks[after].Set<EndBoss>();
                        var enemy2 = mapData.Enemys[3].CopyTo(_world);
                        enemy2.Set<Anubis>();
                        enemy2.Enable();
                        ref var gameObject2 = ref enemy2.Get<GameObject>();
                        var posi2 = pos;
                        gameObject2.LayerDepth = PosTransformer.ScreenToDepth(posi2);
                        var dummy2 = _physicsWorld.CreateRectangle(32, 32, 1f, posi2);
                        dummy2.BodyType = BodyType.Dynamic;
                        dummy2.Mass = 100;
                        dummy2.Tag = enemy2;
                        gameObject2.PlayerBody = dummy2;
                        mapData.Chunks[after].Get<Room>().Enemys.Add(enemy2);
                    }
                    else if (number == 2)
                    {
                        var enemy1 = mapData.Enemys[2].CopyTo(_world);
                        enemy1.Enable();
                        ref var gameObject = ref enemy1.Get<GameObject>();
                        var posi = pos;
                        gameObject.LayerDepth = PosTransformer.ScreenToDepth(posi);
                        var dummy = _physicsWorld.CreateRectangle(16, 16, 1f, posi);
                        dummy.BodyType = BodyType.Static;
                        dummy.Mass = 100;
                        dummy.Tag = enemy1;
                        gameObject.PlayerBody = dummy;
                        mapData.Chunks[after].Get<Room>().Pots.Add(enemy1);
                    }
                    else
                    {
                        var enemy = mapData.Enemys[number].CopyTo(_world);
                        enemy.Enable();
                        ref var gameObject1 = ref enemy.Get<GameObject>();
                        var posi1 = pos;
                        gameObject1.LayerDepth = PosTransformer.ScreenToDepth(posi1);
                        var dummy1 = _physicsWorld.CreateRectangle(4, 20, 1f, posi1);
                        dummy1.BodyType = BodyType.Dynamic;
                        dummy1.Mass = 100;
                        dummy1.Tag = enemy;
                        gameObject1.PlayerBody = dummy1;

                        mapData.Chunks[after].Get<Room>().Enemys.Add(enemy);
                    }
                }


            }
        }






        internal static void OpenDorrs(int key)
        {
            ref var mapData = ref _map.Get<Map>();
            if (mapData.Chunks[key].Get<Room>().Open) return;
            for (int i = 0; i < mapData.Chunks[key].Get<Room>().Doors.Count; i++)
            {
                mapData.Chunks[key].Get<Room>().Doors[i].Set<Open>();
            }
            mapData.Chunks[key].Get<Room>().Open = true;
            SoundHelper.PlaySound("Level Geschafft");
        }

        internal static bool AllDead(int chunk)
        {
            ref var mapData = ref _map.Get<Map>();
            ref var room = ref mapData.Chunks[chunk].Get<Room>();
            for (int i = 0; i < room.Enemys.Count; i++)
            {
                if (room.Enemys[i].IsEnabled()) return false;
            }
            return true;
        }
    }
}
