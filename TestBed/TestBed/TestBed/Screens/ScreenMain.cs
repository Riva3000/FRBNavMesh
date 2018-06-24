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
using RCommonFRB.VisualPrimitives;
using FRBNavMesh;
using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using FlatRedBall.Graphics;
using System.Diagnostics;

namespace TestBed.Screens
{
	public partial class ScreenMain
	{
        NavMesh<FRBNavMesh.PortalNode, FRBNavMesh.Link> _NavMesh;

        Point _StartPos;
        Point _GoalPos;

        // Debug visuals
        Entities.MarkerCross _StartMarker;
        Entities.MarkerCross _GoalMarker;

        List<Line> _FinalPathLines;
        List<Line> _NodesPathLines;
        List<RCommonFRB.VisualPrimitives.VisualRectangle> _VisualRects;


		void CustomInitialize()
		{
            Camera.Main.BackgroundColor = Color.Gray;

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

            _NavMesh = new NavMesh<FRBNavMesh.PortalNode, FRBNavMesh.Link>( RectsList );

            _StartMarker = new Entities.MarkerCross();
            _GoalMarker = new Entities.MarkerCross { Color = Color.LightGreen, Visible = false };

            _SetStart(-100f, -220f);
            _SetGoal(-100f, -220f);


            // -------------------- Debug & tests

            _FinalPathLines = new List<Line>(30);
            _NodesPathLines = new List<Line>(10);
            _VisualRects = new List<VisualRectangle>(RectsList.Count);
            AxisAlignedRectangle rect;
            VisualRectangle visRect;
            Color transpBlack = new Color(0, 0, 0, 128);
            for (int i = 0; i < RectsList.Count; i++)
            {
                rect = RectsList[i];
                rect.Visible = false;

                visRect = new VisualRectangle(rect.Width, rect.Height, transpBlack, Color.Salmon);
                visRect.Position.X = rect.Position.X;
                visRect.Position.Y = rect.Position.Y;
                visRect.Position.Z = 0f;

                SpriteManager.AddDrawableBatch(visRect);
            }

            foreach (var portal in _NavMesh.PortalNodes)
            {
                _Debug_ShowLine( portal.GetPortalSideFor(portal.ParentNavArea1), Color.Yellow );
            }

            Text textObj;
            foreach (var navArea in _NavMesh.NavAreas)
            {
                textObj = TextManager.AddText($"{navArea.ID} [{navArea.Portals.Count}]");
                textObj.HorizontalAlignment = HorizontalAlignment.Center;
                textObj.X = navArea.Polygon.X;
                textObj.Y = navArea.Polygon.Y;
            }
        }

		void CustomActivity(bool firstTimeCalled)
		{
            if (InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton))
            {
                if (Cmn.IsMouseInsideWindow())
                {
                    if ( InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) 
                         || 
                         InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift) )
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
            List<FRBNavMesh.PortalNode> nodePath;
            var pointsPath = _NavMesh.FindPath(_StartPos, _GoalPos, out nodePath);


            // -- Debug visuals
            // NavPolys
            foreach (var navArea in _NavMesh.NavAreas)
            {
                navArea.Polygon.Color = Color.Salmon;
            }
            if (nodePath != null)
            {
                foreach (var node in nodePath)
                {
                    node.ParentNavArea1.Polygon.Color = Color.SkyBlue;
                    node.ParentNavArea2.Polygon.Color = Color.SkyBlue;
                }
            }
            
            // - Node Path Lines
            int nodePathLinesCount;
            Line nodePathLine;
            int i = 0;

            if (nodePath == null) // no path found
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
                    nodePathLine.Color = Dbg.NiceBlue; //Debug.Gray96;
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

        private void _Debug_ShowLine(SimpleLine line, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(line.Start.X, line.Start.Y), new Point3D(line.End.X, line.End.Y));
            visLine.Color = color;
            visLine.Visible = true;
            visLine.Z = 10f;
        }
	}
}
