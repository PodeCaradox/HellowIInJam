using DefaultEcs;
using DefaultEcs.System;
using HellowIInJam.Components.Objects;
using HellowIInJam.Components.Shared;
using HellowIInJam.Main.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellowIInJam.Systems.Draw
{
   
    [With(typeof(GameObject))]
    internal sealed class DrawGameObjectSystem : AEntitySystem<float>
    {
        private readonly SpriteBatch _batch;
        private readonly World _world;
        private readonly Entity _camera;
        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        internal DrawGameObjectSystem(SpriteBatch batch, World world)
             : base(world)
        {
            _batch = batch;
            _world = world;
            _camera = _world.GetEntities().With<Camera>().AsSet().GetEntities()[0];



        }

      

        protected override void PreUpdate(float state)
        {
            _batch.Begin(SpriteSortMode.Deferred, samplerState: SS_PointBorder, transformMatrix: _camera.Get<Camera>().Transform);
        }

        protected override void Update(float elaspedTime, in Entity entity)
        {
            ref GameObject gameObject = ref entity.Get<GameObject>();
            ref TextureShared texture = ref entity.Get<TextureShared>();


            _batch.Draw(texture.TextureSheet,gameObject.Position,gameObject.SourceRect, Color.White);

        }

        protected override void PostUpdate(float state)
        {
            _batch.End();
        }

    }
}
