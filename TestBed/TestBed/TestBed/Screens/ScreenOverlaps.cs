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

using FRBNavMesh;



namespace TestBed.Screens
{
	public partial class ScreenOverlaps
	{
        NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link> _NavMesh;



		void CustomInitialize()
		{
            _NavMesh = new NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link>( RectsList );

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

	}
}
