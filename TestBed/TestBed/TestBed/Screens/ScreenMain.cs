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
        NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link> _NavMesh;

        Point _StartPos;
        Point _GoalPos;

        // Debug visuals
        Entities.MarkerCross _StartMarker;
        Entities.MarkerCross _GoalMarker;

        List<Line> _FinalPathLines;
        List<Line> _NodesPathLines;


		void CustomInitialize()
		{
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

            _NavMesh = new NavMesh<FRBNavMesh.PositionedNode, FRBNavMesh.Link>( RectsList );

            _StartMarker = new Entities.MarkerCross();
            _GoalMarker = new Entities.MarkerCross { Color = Color.Green, Visible = false };

            _SetStart(-100f, -220f);
            _SetGoal(-100f, -220f);

            _FinalPathLines = new List<Line>(30);
            _NodesPathLines = new List<Line>(10);

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

                    if (InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                    {
                        _SetGoal(InputManager.Mouse.WorldXAt(0), InputManager.Mouse.WorldYAt(0));
                    }
                    else
                    {
                        _SetStart(ref _GoalPos);
                        _SetGoal(InputManager.Mouse.WorldXAt(0), InputManager.Mouse.WorldYAt(0));
                    }

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
            // -- Find path
            List<FRBNavMesh.PositionedNode> nodePath;
            var pointsPath = _NavMesh.FindPath(_StartPos, _GoalPos, out nodePath);


            // -- Debug visuals
            // NavPolys
            foreach (var node in _NavMesh.PortalNodes)
            {
                node.Polygon.Color = Color.Salmon;
            }
            if (nodePath != null)
            {
                foreach (var node in nodePath)
                    node.Polygon.Color = Color.SkyBlue;
            }
            
            // - Node Path Lines
            int nodePathLinesCount;
            Line nodePathLine;
            int i = 0;

            if (pointsPath == null) // no path found
            {
                nodePathLinesCount = 0;
            }
            else
            {
                nodePathLinesCount = nodePath.Count - 1;
            
                // Add Lines
                int linesMissing = nodePathLinesCount - _NodesPathLines.Count;
                while (linesMissing > 0)
                {
                    nodePathLine = ShapeManager.AddLine();
                    nodePathLine.Color = Debug.NiceBlue; //Debug.Gray96;
                    _NodesPathLines.Add( nodePathLine );
                    linesMissing--;
                }

                // Update Lines
                for (; i < nodePathLinesCount; i++)
                {
                    nodePathLine = _NodesPathLines[i];
                    nodePathLine.SetFromAbsoluteEndpoints(
                        new Point3D(nodePath[i].X, nodePath[i].Y),
                        new Point3D(nodePath[i+1].X, nodePath[i+1].Y)
                    );
                    nodePathLine.Visible = true;
                }
            }
            
            // Hide remaining Lines
            for (; i < _NodesPathLines.Count; i++)
            {
                _NodesPathLines[i].Visible = false;
            }

            // - Final Path Lines
            int pathLinesCount;
            Line line;
            int j = 0;

            if (pointsPath == null) // no path found
            {
                pathLinesCount = 0;
            }
            else
            {
                pathLinesCount = pointsPath.Count - 1;
            
                // Add Lines
                int linesMissing = pathLinesCount - _FinalPathLines.Count;
                while (linesMissing > 0)
                {
                    _FinalPathLines.Add( ShapeManager.AddLine() );
                    linesMissing--;
                }

                // Update Lines
                for (; j < pathLinesCount; j++)
                {
                    line = _FinalPathLines[j];
                    line.SetFromAbsoluteEndpoints(
                        new Point3D(pointsPath[j].X, pointsPath[j].Y),
                        new Point3D(pointsPath[j+1].X, pointsPath[j+1].Y)
                    );
                    line.Visible = true;
                }
            }
            
            // Hide remaining Lines
            for (; j < _FinalPathLines.Count; j++)
            {
                _FinalPathLines[j].Visible = false;
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
