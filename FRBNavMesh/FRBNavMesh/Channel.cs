using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    class Portal
    {
        public Point left;
        public Point right;
    }

    class Channel
    {
        public List<Portal> portals;
        public List<Point> path; // R: pts ?


        public Channel()
        {
            portals = new List<Portal>();
        }


        /*
        * @param {Phaser.Point} p1 
        * @param {Phaser.Point} p2 
        * 
        * @memberof Channel
        */
        public void Add(Point p1, Point? p2 = null)
        {
            if (!p2.HasValue) p2 = p1;
            this.portals.Add(
                new Portal
                {
                    left = p1,
                    right = p2.Value
                }
            );
        }

        public List<Point> stringPull()
        {
            var portals_ = this.portals;
            var pts = new List<Point>();
            // Init scan state
            Point portalApex, portalLeft, portalRight;
            var apexIndex = 0;
            var leftIndex = 0;
            var rightIndex = 0;

            portalApex = portals_[0].left;
            portalLeft = portals_[0].left;
            portalRight = portals_[0].right;

            // Add start point.
            pts.Add(portalApex);

            for (int i = 1; i < portals.Count; i++)
            {
                // Find the next portal vertices
                var left = portals[i].left;
                var right = portals[i].right;

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

            if (pts.Count == 0 || pts[pts.Count - 1] != portals[portals.Count - 1].left)
            {
                // Append last point to path.
                pts.Add(portals[portals.Count - 1].left);
            }

            this.path = pts;

            return pts; // List<Point> ?
        }
    }
}
