using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;

using RCommonFRB;
using FRBNavMesh;
using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace TestBed.Screens
{
	public partial class ScreenMain
	{

		void CustomInitialize()
		{
            var navMesh = 
                new NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link>( 
                    new List<AxisAlignedRectangle>
                    {
                        mRect1Main,
                        mRect2InnerTouching,
                        mRect3InnerTouching,
                        mRect4InnerTouching,
                        mRect5InnerTouching,
                        mRect6OuterTouching,
                        mRect7OuterTouching,
                        mRect8OuterNotTouching,
                    } 
                );


            // -------------------- Debug & tests

            //_Debug_SlowLine( new SimpleLine(-200f, 200f, 200f, -200f), Color.Red );

            /*var aaRect = new AxisAlignedRectangle { Width = 10f, Height = 10f, X = 100f, Y = 100f };
            Diag.Msg($"aaRect .Top: {aaRect.Top} .Left: {aaRect.Left}");*/
        }

		void CustomActivity(bool firstTimeCalled)
		{


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }



        // -------
        private void _Debug_SlowLine(SimpleLine line, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(line.Start.X, line.Start.Y), new Point3D(line.End.X, line.End.Y));
            visLine.Color = color;
            visLine.Visible = true;
        }
	}
}
