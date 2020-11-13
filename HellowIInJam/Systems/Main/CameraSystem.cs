using DefaultEcs;
using DefaultEcs.System;
using CastleSim.Systems.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellowIInJam.Main.Components;
using HellowIInJam.Components.Objects.Player;

namespace CastleSim.Systems.Update
{
    public sealed class CameraSystem : AComponentSystem<float, Camera>
    {

        #region Variabels

       

        private readonly World _world;
        private readonly Entity _player;

        #endregion

        #region Constructor

        public CameraSystem(World world)
          : base(world)
        {
            _world = world;
            _player = _world.GetEntities().With<Player>().AsSet().GetEntities()[0];
            //minimapView

        }

        #endregion


        #region Methods

        protected override void Update(float elaspedTime, ref Camera component)
        {
            UpdateInput(ref component, elaspedTime);

        }

        private void UpdateInput(ref Camera component, float elaspedTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            component.Changed = true;
            component.CameraPosition.Y -= _player.Get<Player>().Speed;
            //if (keyboard.IsKeyDown(Keys.W))
            //{
            //    component.Changed = true;
            //    component.CameraPosition.Y -= component.CameraSpeed * elaspedTime;
            //}

            //if (keyboard.IsKeyDown(Keys.S))
            //{
            //    component.Changed = true;
            //    component.CameraPosition.Y += component.CameraSpeed * elaspedTime;

            //}

            //if (keyboard.IsKeyDown(Keys.D))
            //{
            //    component.Changed = true;
            //    component.CameraPosition.X += component.CameraSpeed * elaspedTime;
            //}

            //if (keyboard.IsKeyDown(Keys.A))
            //{
            //    component.Changed = true;
            //    component.CameraPosition.X -= component.CameraSpeed * elaspedTime;
            //}

            component.PreviousMouseWheelValue = component.CurrentMouseWheelValue;
            component.CurrentMouseWheelValue = Mouse.GetState().ScrollWheelValue;

            if (component.CurrentMouseWheelValue > component.PreviousMouseWheelValue)
            {
                //todo
           
                    component.Changed = true;
                    CameraHelper.AdjustZoom(component.ZoomSteps, ref component);
                
            }

            if (component.CurrentMouseWheelValue < component.PreviousMouseWheelValue)
            {
               
                    component.Changed = true;
                    CameraHelper.AdjustZoom(-component.ZoomSteps, ref component);
                

            }

            if (component.Changed)
            {
                CameraHelper.UpdateMatrix(ref component);
                component.Changed = false;
            }

        }



        #endregion

    }
}
