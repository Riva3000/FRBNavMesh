using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    internal class Channel
    {
        private struct Portal
        {
            public readonly Point Left;
            public readonly Point Right;

            public Portal(Point left, Point right)
            {
                Left = left;
                Right = right;
            }
        }

        private List<Portal> _Portals;
        public List<Point> Path;





        public Channel()
        {
            _Portals = new List<Portal>();
        }




        public void Add(Point leftPoint, Point? rightPoint = null)
        {
            if (!rightPoint.HasValue) rightPoint = leftPoint;

            _Portals.Add(
                new Portal
                (
                    leftPoint,
                    rightPoint.Value
                )
            );
        }

        public List<Point> StringPull()
        {
            var pts = new List<Point>();
            // Init scan state
            Point portalApex, portalLeft, portalRight;
            var apexIndex = 0;
            var leftIndex = 0;
            var rightIndex = 0;

            portalApex = _Portals[0].Left;
            portalLeft = _Portals[0].Left;
            portalRight = _Portals[0].Right;

            // Add start point.
            pts.Add(portalApex);

            for (int i = 1; i < _Portals.Count; i++)
            {
                // Find the next portal vertices
                var left = _Portals[i].Left;
                var right = _Portals[i].Right;

                // Update right vertex.
                if (Utils.triarea2(portalApex, portalRight, right) <= 0.0)
                {
                    if (portalApex == portalRight || Utils.triarea2(portalApex, portalLeft, right) > 0.0)
                    {
                        // Tighten the funnel.
                        portalRight = right;
                        rightIndex = i;
                    }
                    else
                    {
                        // Right vertex just crossed over the left vertex, so the left vertex should
                        // now be part of the path.
                        pts.Add(portalLeft);

                        // Restart scan from portal left point.

                        // Make current left the new apex.
                        portalApex = portalLeft;
                        apexIndex = leftIndex;
                        // Reset portal
                        portalLeft = portalApex;
                        portalRight = portalApex;
                        leftIndex = apexIndex;
                        rightIndex = apexIndex;
                        // Restart scan
                        i = apexIndex;
                        continue;
                    }// if else
                }// if

                // Update left vertex.
                if (Utils.triarea2(portalApex, portalLeft, left) >= 0.0)
                {
                    if (portalApex == portalLeft || Utils.triarea2(portalApex, portalRight, left) < 0.0)
                    {
                        // Tighten the funnel.
                        portalLeft = left;
                        leftIndex = i;
                    }
                    else
                    {
                        // Left vertex just crossed over the right vertex, so the right vertex should
                        // now be part of the path
                        pts.Add(portalRight);

                        // Restart scan from portal right point.

                        // Make current right the new apex.
                        portalApex = portalRight;
                        apexIndex = rightIndex;
                        // Reset portal
                        portalLeft = portalApex;
                        portalRight = portalApex;
                        leftIndex = apexIndex;
                        rightIndex = apexIndex;
                        // Restart scan
                        i = apexIndex;
                        continue;
                    }
                }
            }// for

            if (pts.Count == 0 || pts[pts.Count - 1] != _Portals[_Portals.Count - 1].Left)
            {
                // Append last point to path.
                pts.Add(_Portals[_Portals.Count - 1].Left);
            }

            this.Path = pts;

            return pts;
        }
    }
}
