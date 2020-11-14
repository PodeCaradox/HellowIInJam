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

        internal static void ActivateCollision(int before, int after)
        {
            ref var mapData = ref _map.Get<Map>();
            if (mapData.Chunks[before].Has<NeedsToCheckCollision>()) mapData.Chunks[before].Remove<NeedsToCheckCollision>();
            mapData.Chunks[after].Set<NeedsToCheckCollision>();

        }
    }
}
