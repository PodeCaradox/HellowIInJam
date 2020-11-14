using DefaultEcs;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Helper.Main
{
    public static class ChunkHelper
    {
        private static Entity _map;
        public static void Init(World world)
        {
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
    
            
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

            if(!mapData.Chunks[after].Get<Room>().Open)
            for (int i = 0; i < mapData.Chunks[after].Get<Room>().Doors.Count; i++)
            {
                mapData.Chunks[after].Get<Room>().Doors[i].Get<Door>().Opnened = false;
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
