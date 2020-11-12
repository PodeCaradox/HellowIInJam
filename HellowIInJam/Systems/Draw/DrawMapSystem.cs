using DefaultEcs;
using DefaultEcs.System;
using CastleSim.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HellowIInJam.Components.Map;
using Microsoft.Xna.Framework.Content;

namespace CastleSim.Systems
{

    [With(typeof(Map))]
    internal sealed class DrawMapSystem : AEntitySystem<float>
    {
        private readonly SpriteBatch _batch;
        private readonly World _world;
        private readonly Entity _camera;
        private readonly Texture2D dummy;
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        internal DrawMapSystem(SpriteBatch batch, World world, ContentManager Content)
             : base(world)
        {
            _batch = batch;
            _world = world;
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];

            dummy = Content.Load<Texture2D>("dummy");

        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref Map map = ref entity.Get<Map>();

            _batch.Begin(SpriteSortMode.Deferred,samplerState: SS_PointBorder, transformMatrix: _camera.Get<Camera>().Transform);

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                ref var tile = ref map.Tiles[i].Get<MapTile>();
                _batch.Draw(dummy, tile.Position, new Rectangle(tile.TileID * 16, 0, 16, 16), Color.White);
            }
            

            _batch.End();

        }

    }
}


