using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Objects.Player;
using HellowIInJam.Components.Shared;
using HellowIInJam.Main.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Draw
{
    [With(typeof(Lightet))]
    internal sealed class DrawDarkness : AEntitySystem<float>
    {
        private readonly SpriteBatch _batch;
        private readonly World _world;
        private readonly Entity _camera;
        private readonly Entity _player;
        private readonly Texture2D _viniette;
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        internal DrawDarkness(SpriteBatch batch, World world, ContentManager Content)
             : base(world)
        {
            _batch = batch;
            _world = world;
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
            _player = _world.GetEntities().With<Player>().AsSet().GetEntities()[0];
            _viniette = Content.Load<Texture2D>("viniette");

        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref Room room = ref entity.Get<Room>();

            _batch.Begin(SpriteSortMode.Deferred, samplerState: SS_PointBorder, transformMatrix: _camera.Get<Camera>().Transform);

            ref var player = ref _player.Get<Player>();


            
            if (player.Demonized >= 1 && player.Transformed)
            {
                ref var viniette = ref _player.Get<GameObject>();


                for (int index = 0; index < room.Tiles.Length; index++)
                {
                    ref var tile = ref room.Tiles[index].Get<MapTile>();
                    ref var texture = ref room.Tiles[index].Get<TextureShared>();
                    _batch.Draw(texture.TextureSheet, tile.Position, new Rectangle(-16, 0, 16, 16), Color.Black);
                }
                _batch.Draw(_viniette, viniette.PlayerBody.Position - new Vector2(44,44),new Rectangle(0,0, _viniette.Width - 1, _viniette.Height - 1), Color.White);
               
            }

            if (!player.Transformed || player.Demonized == 0)
            {
                for (int index = 0; index < room.Tiles.Length; index++)
                {
                    ref var tile = ref room.Tiles[index].Get<MapTile>();
                    ref var texture = ref room.Tiles[index].Get<TextureShared>();
                    _batch.Draw(texture.TextureSheet, tile.Position, new Rectangle(-16, 0, 16, 16), Color.White);
                }
            }







            _batch.End();

        }

    }



}
