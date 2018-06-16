using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;



namespace TestBed.Entities
{
	public partial class MarkerCross
	{
        public Color Color
        {
            get { return mLine1.Color; }
            set
            {
                if (value != mLine1.Color)
                {
                    mLine1.Color = value;
                    mLine2.Color = value;
                }
            }
        }

        public bool Visible
        {
            get { return mLine1.Visible; }
            set
            {
                if (value != mLine1.Visible)
                {
                    mLine1.Visible = value;
                    mLine2.Visible = value;
                }
            }
        }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{


		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
