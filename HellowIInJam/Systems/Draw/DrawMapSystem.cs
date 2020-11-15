using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HellowIInJam.Components.Map;
using Microsoft.Xna.Framework.Content;
using HellowIInJam.Main.Components;
using HellowIInJam.Components.Shared;
using System.Diagnostics;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;

namespace CastleSim.Systems
{

    [With(typeof(Map))]
    internal sealed class DrawMapSystem : AEntitySystem<float>
    {
        private readonly SpriteBatch _batch;
        private readonly World _world;
        private readonly Entity _camera;
        private readonly Entity _player;
 
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        internal DrawMapSystem(SpriteBatch batch, World world)
             : base(world)
        {
            _batch = batch;
            _world = world;
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
            _player = _world.GetEntities().With<Player>().AsSet().GetEntities()[0];
          


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

            _batch.Begin(SpriteSortMode.FrontToBack, samplerState: SS_PointBorder, transformMatrix: _camera.Get<Camera>().Transform);

            for (int i = 0; i < map.ChunksToDraw; i++)
            {
                ref var room = ref map.ToDraw[i].Get<Room>();
                for (int index = 0; index < room.GameObjects.Count; index++)
                {
                    ref GameObject gameObject = ref room.GameObjects[index].Get<GameObject>();
                    ref TextureShared texture = ref room.GameObjects[index].Get<TextureShared>();




                    _batch.Draw(texture: texture.TextureSheet, position: gameObject.PlayerBody.Position - gameObject.Offset, gameObject.SourceRect, color: Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth: gameObject.LayerDepth);

                }

            }

            for (int i = 0; i < map.ChunksToDraw; i++)
            {
                ref var room = ref map.ToDraw[i].Get<Room>();
                for (int index = 0; index < room.Enemys.Count; index++)
                {
                    if (!room.Enemys[index].IsEnabled()) continue;
                    ref GameObject gameObject = ref room.Enemys[index].Get<GameObject>();
                    ref TextureShared texture = ref room.Enemys[index].Get<TextureShared>();




                    _batch.Draw(texture: texture.TextureSheet, position: gameObject.PlayerBody.Position - gameObject.Offset, gameObject.SourceRect, color: Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth: gameObject.LayerDepth);

                }

            }

            for (int i = 0; i < map.ChunksToDraw; i++)
            {
                ref var room = ref map.ToDraw[i].Get<Room>();
                for (int index = 0; index < room.Pots.Count; index++)
                {
                    if (!room.Pots[index].IsEnabled()) continue;
                    ref GameObject gameObject = ref room.Pots[index].Get<GameObject>();
                    ref TextureShared texture = ref room.Pots[index].Get<TextureShared>();




                    _batch.Draw(texture: texture.TextureSheet, position: gameObject.PlayerBody.Position - gameObject.Offset, gameObject.SourceRect, color: Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth: gameObject.LayerDepth);

                }

            }

            if (_player.IsEnabled())
            {
                ref GameObject gameObject1 = ref _player.Get<GameObject>();
                ref TextureShared texture1 = ref _player.Get<TextureShared>();
                ref Player player = ref _player.Get<Player>();
                _batch.Draw(texture: texture1.TextureSheet, position: gameObject1.PlayerBody.Position - gameObject1.Offset + gameObject1.AnimationOffset, gameObject1.SourceRect, color: player.Color, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth: gameObject1.LayerDepth);

            }


            _batch.End();

        }

    }
}


