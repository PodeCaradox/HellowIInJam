using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HellowIInJam.Components.Map;
using Microsoft.Xna.Framework.Content;
using HellowIInJam.Main.Components;
using HellowIInJam.Components.Shared;
using System.Diagnostics;

namespace CastleSim.Systems
{

    [With(typeof(Map))]
    internal sealed class DrawMapSystem : AEntitySystem<float>
    {
        private readonly SpriteBatch _batch;
        private readonly World _world;
        private readonly Entity _camera;
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        internal DrawMapSystem(SpriteBatch batch, World world)
             : base(world)
        {
            _batch = batch;
            _world = world;
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];

           

        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref Map map = ref entity.Get<Map>();

            _batch.Begin(SpriteSortMode.Deferred,samplerState: SS_PointBorder, transformMatrix: _camera.Get<Camera>().Transform);
        
            for (int i = 0; i < map.ChunksToDraw; i++)
            {
                ref var room = ref map.ToDraw[i].Get<Room>();
                for (int index = 0; index < room.Tiles.Length; index++)
                {
                    ref var tile = ref room.Tiles[index].Get<MapTile>();
                    ref var texture = ref room.Tiles[index].Get<TextureShared>();
                    _batch.Draw(texture.TextureSheet, tile.Position, new Rectangle(tile.TileID * 16, 0, 16, 16), Color.White);
                }
             
            }
            

            _batch.End();

        }

    }
}


