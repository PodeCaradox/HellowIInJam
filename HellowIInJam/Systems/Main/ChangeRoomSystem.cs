using CastleSim.Systems.HelperClasses;
using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Helper.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Main
{

    [With(typeof(ChangeChunk))]
    internal sealed class ChangeRoomSystem : AEntitySystem<float>
    {
        private readonly World _world;
        private readonly Entity _map;

        internal ChangeRoomSystem(World world)
             : base(world)
        {
            _world = world;
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
        }





        protected override void Update(float elaspedTime, in Entity entity)
        {
            
            ref var player = ref entity.Get<GameObject>();
            ref var playerData = ref entity.Get<Player>();
            ref var dir = ref entity.Get<ChangeChunk>();
            var before = PosTransformer.ScreenToChunkKKey(player.PlayerBody.Position);
            var directionPlayer = new Vector2();
            if (dir.Direction == 0)
            {
                directionPlayer.X -= 80;
              

            }
            else if (dir.Direction == 1)
            {
                directionPlayer.X += 80;
            }
            else if (dir.Direction == 2)
            {
                directionPlayer.Y -= 80;
            }
            else
            {
                directionPlayer.Y += 80;
            }

            player.PlayerBody.Position += directionPlayer;
            var after = PosTransformer.ScreenToChunkKKey(player.PlayerBody.Position);
            if(before != after)
                ChunkHelper.ActivateLight(before, after);
            playerData.ChunkBefore = after;

            entity.Remove<ChangeChunk>();

        }
    }
}
