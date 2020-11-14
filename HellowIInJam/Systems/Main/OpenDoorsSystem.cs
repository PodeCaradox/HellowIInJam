using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Main
{
   
    [With(typeof(Open))]
    internal sealed class OpenDoorsSystem : AEntitySystem<float>
    {
        private readonly World _world;
        private readonly Entity _map;

        internal OpenDoorsSystem(World world)
             : base(world)
        {
            _world = world;
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
        }





        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref var door = ref entity.Get<Door>();
            var chunkEntity = _world.GetEntities().With<Lightet>().AsSet().GetEntities()[0];
            ref var chunk = ref chunkEntity.Get<Room>();
            ref var map = ref _map.Get<Map>();
            door.Opnened = true;
            entity.Remove<Open>();
            if (door.Direction == 0)
            {
                for (int i = 0; i < chunk.GameObjects.Count; i++)
                {
                    ref var gameObject = ref chunk.GameObjects[i].Get<GameObject>();
                    if (gameObject.ID == 49 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(49).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 50 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(50).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 56 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(56).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 57 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(57).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 63 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(63).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 64 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(64).Get<GameObject>().SourceRect;

                }

            }else if (door.Direction == 1)
            {

                for (int i = 0; i < chunk.GameObjects.Count; i++)
                {
                    ref var gameObject = ref chunk.GameObjects[i].Get<GameObject>();
                    if (gameObject.ID == 55 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(55).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 54 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(54).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 62 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(62).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 61 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(61).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 69 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(69).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 68 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(68).Get<GameObject>().SourceRect;

                }
            }
            else if (door.Direction == 2)
            {
    
                for (int i = 0; i < chunk.GameObjects.Count; i++)
                {
                    ref var gameObject = ref chunk.GameObjects[i].Get<GameObject>();
                    if (gameObject.ID == 51 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(51).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 58 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(58).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 52 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(52).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 59 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(59).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 53 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(53).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 60 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(60).Get<GameObject>().SourceRect;

                }
            }
            else if (door.Direction == 3)
            {
                for (int i = 0; i < chunk.GameObjects.Count; i++)
                {
                    ref var gameObject = ref chunk.GameObjects[i].Get<GameObject>();
                    if (gameObject.ID == 72 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(72).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 65 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(65).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 73 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(73).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 66 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(66).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 74 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(74).Get<GameObject>().SourceRect;
                    else if (gameObject.ID == 67 + 28) gameObject.SourceRect = map.DefaulObjects.GetValueOrDefault(67).Get<GameObject>().SourceRect;

                }
            }
        }
    }
}
