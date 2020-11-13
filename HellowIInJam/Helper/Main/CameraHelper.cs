using DefaultEcs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using HellowIInJam.Components.Main;
using HellowIInJam.Main.Components;

namespace CastleSim.Systems.HelperClasses
{
    internal static class CameraHelper
    {
        private static Entity _gameConfig;
        private static Entity _camera;
        internal static void Init(World world)
        {
            _gameConfig = world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0];
            _camera = world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
        }

        internal static void SetCameraPos(Vector2 position)
        {
            ref Camera cameraData = ref _camera.Get<Camera>();
            cameraData.CameraPosition = position;
            
            cameraData.Changed = true;
        }

        internal static void SetCameraZoom(float zoom)
        {
            ref Camera cameraData = ref _camera.Get<Camera>();
            cameraData.Zoom = zoom;
            cameraData.ZoomChanged = true;
        }

        internal static void UpdateMatrix(ref Camera component)
        {
            var positionRounded = component.CameraPosition;
            positionRounded.Round();
            component.Transform = Matrix.CreateTranslation(new Vector3(-positionRounded.X, -positionRounded.Y, 0)) *
                    Matrix.CreateScale(component.Zoom, component.Zoom, 1) *
                    Matrix.CreateTranslation(new Vector3(component.Bounds.Width * 0.5f, component.Bounds.Height * 0.5f, 0));
            UpdateVisibleArea(ref component);
        }

        private static void UpdateVisibleArea(ref Camera component)
        {
            var inverseViewMatrix = Matrix.Invert(component.Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(component.Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, component.Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(component.Bounds.Width, component.Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            component.VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)Math.Round(max.X - min.X, 0), (int)Math.Round(max.Y - min.Y, 0));
            component.Offset = PosTransformer.ScreenToWorldPos(new Point(component.VisibleArea.X + component.VisibleArea.Width, component.VisibleArea.Y));
       
            if (component.ZoomChanged)
            {
                var TopLef = PosTransformer.ScreenToWorldPos(new Point(component.VisibleArea.X, component.VisibleArea.Y));
                var TopRig = component.Offset;
                var BottomRig = PosTransformer.ScreenToWorldPos(new Point(component.VisibleArea.X + component.VisibleArea.Width, component.VisibleArea.Y + component.VisibleArea.Height));
                ref var gameConfig = ref _gameConfig.Get<GameConfig>();
                component.ColumnsToDraw = TopRig.X - TopLef.X + 2;
               
                component.RowsToDraw =  BottomRig.Y - TopRig.Y + 2;

                component.ZoomChanged = false;
            }

        }

        internal static void AdjustZoom(float zoomAmount, ref Camera component)
        {

            component.Zoom += zoomAmount;
            if (component.Zoom < component.MaxZoomOut)
            {
                component.Zoom = component.MaxZoomOut;
            }
            if (component.Zoom > 5f)
            {
                component.Zoom = 5f;
            }

            component.ZoomChanged = true;


        }
    }
}
