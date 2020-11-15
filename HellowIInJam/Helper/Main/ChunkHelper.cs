using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using Microsoft.Xna.Framework;
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

        public static void Delete(Entity entity,int key)
        {
            ref var mapData = ref _map.Get<Map>();
            mapData.Chunks[key].Get<Room>().GameObjects.Remove(entity);
        }

        public static void Add(Entity entity,int key)
        {
            ref var mapData = ref _map.Get<Map>();
            mapData.Chunks[key].Get<Room>().GameObjects.Add(entity);

        }

        internal static void ActivateLight(int before, int after)
        {
            ref var mapData = ref _map.Get<Map>();
            if (before != -1 && mapData.Chunks[before].Has<Lightet>()) mapData.Chunks[before].Remove<Lightet>();
            mapData.Chunks[after].Set<Lightet>();

            if (!mapData.Chunks[after].Get<Room>().Open)
            {
                for (int i = 0; i < mapData.Chunks[after].Get<Room>().Doors.Count; i++)
                {
                    mapData.Chunks[after].Get<Room>().Doors[i].Get<Door>().Opnened = false;
                }

                Vector2 startPosie = mapData.Chunks[after].Get<Room>().Tiles[22].Get<MapTile>().Position;
                for (int i = 0; i < 1; i++)
                {

                    if (2 == 2)
                    {
                        var enemy = mapData.Enemys[2].CopyTo(_world);
                        enemy.Enable();
                        ref var gameObject = ref enemy.Get<GameObject>();
                        var posi = startPosie + new Vector2(20, 60);
                        gameObject.LayerDepth = PosTransformer.ScreenToDepth(posi);
                        var dummy = _physicsWorld.CreateRectangle(16, 16, 1f, posi);
                        dummy.BodyType = BodyType.Static;
                        dummy.Mass = 100;
                        dummy.Tag = enemy;
                        gameObject.PlayerBody = dummy;
                        mapData.Chunks[after].Get<Room>().Pots.Add(enemy);
                    }
                    else
                    {
                        var enemy = mapData.Enemys[2].CopyTo(_world);
                        enemy.Enable();
                        ref var gameObject = ref enemy.Get<GameObject>();
                        var posi = startPosie + new Vector2(20, 60);
                        gameObject.LayerDepth = PosTransformer.ScreenToDepth(posi);
                        var dummy = _physicsWorld.CreateRectangle(4, 20, 1f, posi);
                        dummy.BodyType = BodyType.Dynamic;
                        dummy.Mass = 100;
                        dummy.Tag = enemy;
                        gameObject.PlayerBody = dummy;

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
