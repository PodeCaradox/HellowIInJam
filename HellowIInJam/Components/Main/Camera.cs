using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleSim.Components
{
    public struct Camera
    {
        public float CurrentMouseWheelValue, PreviousMouseWheelValue, Zoom;
        public Vector2 CameraPosition;
        public Rectangle Bounds;
       
        public Rectangle VisibleArea;
        public Matrix Transform;

        public float CameraSpeed;
        public bool Changed;
        public bool ZoomChanged;
        public float MaxZoomOut;
        public float ZoomSteps;

        //each update:
        public Point Offset;
        public int ColumnsToDraw;
        public int RowsToDraw;


    }
}
