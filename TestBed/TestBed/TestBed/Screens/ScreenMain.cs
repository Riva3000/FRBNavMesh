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
using FlatRedBall.Graphics;

namespace TestBed.Screens
{
	public partial class ScreenMain
	{
        NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link> _MavMesh;

        Point _StartPos;
        Point _GoalPos;

        // Debug visuals
        Entities.MarkerCross _StartMarker;
        Entities.MarkerCross _GoalMarker;

        List<Line> _PathLines;


		void CustomInitialize()
		{
            var rects = new List<AxisAlignedRectangle>
            {
                mRect1Main,
                mRect2InnerTouching,
                mRect3InnerTouching,
                mRect4InnerTouching,
                mRect5InnerTouching,
                mRect6OuterTouching,
                mRect7OuterTouching,
                mRect8OuterNotTouching,
            };

            /*Circle circle;
            Text textObj;
            //Color color = Color.FromNonPremultiplied(100, )
            foreach (var rect in rects)
            {
                circle = ShapeManager.AddCircle();
                circle.Position = rect.Position;
                circle.Color = Color.Maroon;
                circle.Radius = rect.BoundingRadius;

                textObj = TextManager.AddText(rect.Name);
                textObj.Position = rect.Position;
                textObj.Y += 8f;
                textObj.HorizontalAlignment = HorizontalAlignment.Center;
            }*/

            _MavMesh = new NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link>( rects );

            _StartMarker = new Entities.MarkerCross();
            _GoalMarker = new Entities.MarkerCross { Color = Color.Green, Visible = false };

            _SetStart(-100f, -220f);

            _PathLines = new List<Line>(30);

            // -------------------- Debug & tests
            /*// works
            Color tranpColor = Color.FromNonPremultiplied(255, 255, 255, 128);
            mLine1.Color = tranpColor;
            mLine2.Color = tranpColor;
            mLine3.Color = tranpColor;*/

            //_Debug_SlowLine( new SimpleLine(-200f, 200f, 200f, -200f), Color.Red );

            /*var aaRect = new AxisAlignedRectangle { Width = 10f, Height = 10f, X = 100f, Y = 100f };
            Diag.Msg($"aaRect .Top: {aaRect.Top} .Left: {aaRect.Left}");*/
        }

		void CustomActivity(bool firstTimeCalled)
		{
            if (InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton))
            {
                if (Cmn.IsMouseInsideWindow())
                {
                    //var mouseWorldPos = new Xna.Vector2(InputManager.Mouse.WorldXAt(0), InputManager.Mouse.WorldYAt(0));

                    /*_DbgPathfindingGoal.X = mouseWorldPos.X;
                    _DbgPathfindingGoal.Y = mouseWorldPos.Y;
                    _DbgPathfindingGoal.Visible = true;
                    PatfindingTestCharacterInstance.SetMovementGoal(mouseWorldPos);*/

                    //Debug.WriteLine(" ---------------------------- TargetCharacter moved");

                    //_PlayerOneCharacter.X = InputManager.Mouse.WorldXAt(0);
                    //_PlayerOneCharacter.Y = InputManager.Mouse.WorldYAt(0);

                    
                    _SetGoal(InputManager.Mouse.WorldXAt(0), InputManager.Mouse.WorldYAt(0));
                    _FindAndShowPath();
                }
            }
		}

		void CustomDestroy()
		{ }

        static void CustomLoadStaticContent(string contentManagerName)
        { }




        // -------
        private void _SetGoal(float x, float y)
        {
            //_SetStart(ref _GoalPos);

            _GoalPos.X = x;
            _GoalPos.Y = y;
            _GoalMarker.X = x;
            _GoalMarker.Y = y;
            _GoalMarker.Visible = true;
        }
        private void _SetStart(float x, float y)
        {
            _StartPos.X = x;
            _StartPos.Y = y;
            _StartMarker.X = x;
            _StartMarker.Y = y;
        }
        private void _SetStart(ref Point newStartPos)
        {
            _StartPos.X = newStartPos.X;
            _StartPos.Y = newStartPos.Y;
            _StartMarker.X = (float)newStartPos.X;
            _StartMarker.Y = (float)newStartPos.Y;
        }

        private void _FindAndShowPath()
        {
            List<FRBNavMesh.PositionedNode> nodePath;
            var pointsPath = _MavMesh.FindPath(_StartPos, _GoalPos, out nodePath);

            // -- Debug visuals
            // NavPolys
            foreach (var node in _MavMesh.NavPolygons)
                node.Polygon.Color = Color.Salmon;
            if (nodePath != null)
            {
                foreach (var node in nodePath)
                    node.Polygon.Color = Color.SkyBlue;
            }
            
            // Path Lines
            int pathLinesCount;
            Line line;
            int i = 0;

            if (pointsPath == null) // no path found
            {
                pathLinesCount = 0;
            }
            else
            {
                pathLinesCount = pointsPath.Count - 1;
            
                // Add Lines
                int linesMissing = pathLinesCount - _PathLines.Count;
                while (linesMissing > 0)
                {
                    _PathLines.Add( ShapeManager.AddLine() );
                    linesMissing--;
                }

                // Update Lines
                for (; i < pathLinesCount; i++)
                {
                    line = _PathLines[i];
                    line.SetFromAbsoluteEndpoints(
                        new Point3D(pointsPath[i].X, pointsPath[i].Y),
                        new Point3D(pointsPath[i+1].X, pointsPath[i+1].Y)
                    );
                    line.Visible = true;
                }
            }
            
            // Hide remaining Lines
            for (; i < _PathLines.Count; i++)
            {
                _PathLines[i].Visible = false;
            }
        }

        private void _Debug_SlowLine(SimpleLine line, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(line.Start.X, line.Start.Y), new Point3D(line.End.X, line.End.Y));
            visLine.Color = color;
            visLine.Visible = true;
        }
	}
}
