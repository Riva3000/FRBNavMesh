using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRBNavMesh
{
    public class NavArea
    {
        public readonly AxisAlignedRectangle Polygon;


        public readonly List<PositionedNode> Portals;

        public readonly int Id;

        //protected SimpleLine[] mEdges;
        //protected SimpleLine mLeftEdge

        // @ These are used only for NavMesh and NavMesh data construction. Should be removed afterwards.
        private SimpleLine _EdgeLeft;
        public SimpleLine EdgeLeft { get { return _EdgeLeft; } }
        private SimpleLine _EdgeRight;
        public SimpleLine EdgeRight { get { return _EdgeRight; } }
        private SimpleLine _EdgeTop;
        public SimpleLine EdgeTop { get { return _EdgeTop; } }
        private SimpleLine _EdgeBottom;
        public SimpleLine EdgeBottom { get { return _EdgeBottom; } }




        public NavArea(AxisAlignedRectangle polygon, int id)
        {
            Portals = new List<PositionedNode>();

            Id = id;
            Polygon = polygon;

            _CalculateEdges();
        }

        /*protected void _CalculateEdges()
        {
            // Assign edges
            mEdges = new SimpleLine[4];

            //  0 > > 3
            //  v     ^
            //  v     ^
            //  1 > > 2

            // Left - top-left to bottom-left
            mEdges[0] = new SimpleLine(mPolygon.Left, mPolygon.Top,    mPolygon.Left, mPolygon.Bottom);
            // Bottom - bottom-left to bottom-right
            mEdges[1] = new SimpleLine(mPolygon.Left, mPolygon.Bottom,    mPolygon.Right, mPolygon.Bottom);
            // Right - bottom-right to top-right
            mEdges[2] = new SimpleLine(mPolygon.Right, mPolygon.Bottom,    mPolygon.Right, mPolygon.Top);
            // Top - top-left to top-right
            mEdges[3] = new SimpleLine(mPolygon.Left, mPolygon.Top,    mPolygon.Right, mPolygon.Top);
        }*/
        protected void _CalculateEdges()
        {
            // Assign edges

            //  v1 old:
            //  0 > > 3
            //  v     v
            //  v     v
            //  1 > > 2

            //  v2 current - counter-clocwise order of points:
            //  0 < < 3
            //  v     ^
            //  v     ^
            //  1 > > 2

            // Left - top-left to bottom-left
            _EdgeLeft = new SimpleLine(Polygon.Left, Polygon.Top,    Polygon.Left, Polygon.Bottom);
            // Bottom - bottom-left to bottom-right
            _EdgeBottom = new SimpleLine(Polygon.Left, Polygon.Bottom,    Polygon.Right, Polygon.Bottom);
            // Right - bottom-right to top-right
            _EdgeRight = new SimpleLine(Polygon.Right, Polygon.Bottom,    Polygon.Right, Polygon.Top);
            // Top - top-right to top-left
            _EdgeTop = new SimpleLine(Polygon.Right, Polygon.Top,    Polygon.Left, Polygon.Top);

            /*Debug.ShowLine(_EdgeRight, Color.Green);
            Debug.ShowLine(_EdgeLeft, Color.DarkGreen);
            Debug.ShowLine(_EdgeTop, Color.Cyan);
            Debug.ShowLine(_EdgeBottom, Color.DarkCyan);*/
        }
    }
}
