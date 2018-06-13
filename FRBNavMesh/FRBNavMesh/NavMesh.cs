using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RCommonFRB;

namespace FRBNavMesh
{
    /*
    * The workhorse that represents a navigation mesh built from a series of polygons. Once built, the
    * mesh can be asked for a path from one point to another point. It has debug methods for 
    * visualizing paths and visualizing the individual polygons. Some internal terminology usage:
    * 
    * - neighbor: a polygon that shares part of an edge with another polygon
    * - portal: when two neighbor's have edges that overlap, the portal is the overlapping line segment
    * - channel: the path of polygons from starting point to end point
    * - pull the string: run the funnel algorithm on the channel so that the path hugs the edges of the
    *   channel. Equivalent to having a string snaking through a hallway and then pulling it taut.
    */

    /// <summary>
    /// R: Almost FIN
    /// </summary>
    class NavMesh
    {
        class LineAndPoint
        {
            public PhaserLine line;
            public Point point;
        }

        private int _meshShrinkAmount;
        private List<NavPoly> _navPolygons;
        private NavGraph _graph;

        /// <summary></summary>
        /// <param name="polygons"></param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        public NavMesh(None game, List<AxisAlignedRectangle> polygons, int meshShrinkAmount = 0)
        {
            _meshShrinkAmount = meshShrinkAmount;

            // Construct NavPoly instances for each polygon
            _navPolygons = new List<NavPoly>();
            for (int i = 0; i < polygons.Count; i++)
            {
                this._navPolygons.Add(new NavPoly(null, i, polygons[i]));
            }

            this._calculateNeighbors();

            // Astar graph of connections between polygons
            this._graph = new NavGraph(this._navPolygons);
        }

        /*
        * Find a path from the start point to the end point using this nav mesh.
        *
        * @param {Phaser.Point} startPoint
        * @param {Phaser.Point} endPoint
        * @param {object} [drawOptions={}] Options for controlling debug drawing
        * @param {boolean} [drawOptions.drawPolyPath=false] Whether or not to visualize the path
        * through the polygons - e.g. the path that astar found.
        * @param {boolean} [drawOptions.drawFinalPath=false] Whether or not to visualize the path
        * through the path that was returned.
        * @returns {Phaser.Point[]|null} An array of points if a path is found, or null if no path
        *
        * @memberof NavMesh
        */
        /// <summary>Find a path from the start point to the end point using this nav mesh.</summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="drawPolyPath">Whether or not to visualize the path through the polygons - e.g. the path that astar found.</param>
        /// <param name="drawFinalPath">Whether or not to visualize the path through the path that was returned.</param>
        /// <returns>An array of points if a path is found, or null if no path</returns>
        public List<Point> findPath(Point startPoint, Point endPoint, bool drawPolyPath = false, bool drawFinalPath = false)
        {
            NavPoly startPoly = null;
            NavPoly endPoly = null;
            float startDistance = float.MaxValue;
            float endDistance = float.MaxValue;
            float d, r;

            // Find the closest poly for the starting and ending point
            foreach (var navPoly in this._navPolygons) {
                r = navPoly.boundingRadius;
                // Start
                //d = navPoly.centroid.Distance(startPoint);
                d = (float)RCommonFRB.Geometry.Distance(ref navPoly.centroid, ref startPoint);
                if (d <= startDistance && d <= r && navPoly.contains(startPoint)) {
                    startPoly = navPoly;
                    startDistance = d;
                }
                // End
                //d = navPoly.centroid.Distance(endPoint);
                d = (float)RCommonFRB.Geometry.Distance(ref navPoly.centroid, ref endPoint);
                if (d <= endDistance && d <= r && navPoly.contains(endPoint))
                {
                    endPoly = navPoly;
                    endDistance = d;
                }
            }

            /*
            // If the start point wasn't inside a polygon, run a more liberal check that allows a point
            // to be within meshShrinkAmount radius of a polygon
            if (!startPoly && this._meshShrinkAmount > 0)
            {
                for (const navPoly of this._navPolygons) 
                {
                    // Check if point is within bounding circle to avoid extra projection calculations
                    r = navPoly.boundingRadius + this._meshShrinkAmount;
                    d = navPoly.centroid.distance(startPoint);
                    if (d <= r) {
                        // Check if projected point is within range of a polgyon and is closer than the
                        // previous point
                        const { distance } = this._projectPointToPolygon(startPoint, navPoly);
                        if (distance <= this._meshShrinkAmount && distance < startDistance) {
                            startPoly = navPoly;
                            startDistance = distance;
                        }
                    }
                }
            }
            */

            /*
            // Same check as above, but for the end point
            if (!endPoly && this._meshShrinkAmount > 0) {
                for (const navPoly of this._navPolygons) {
                    r = navPoly.boundingRadius + this._meshShrinkAmount;
                    d = navPoly.centroid.distance(endPoint);
                    if (d <= r) {
                        const { distance } = this._projectPointToPolygon(endPoint, navPoly);
                        if (distance <= this._meshShrinkAmount && distance < endDistance) {
                            endPoly = navPoly;
                            endDistance = distance;
                        }
                    }
                }
            }
            */

            // No matching polygons locations for the start or end, so no path found
            // R: = start or end point not on nav mesh
            if (startPoly == null || endPoly == null) return null;

            // If the start and end polygons are the same, return a direct path
            if (startPoly == endPoly)
            {
                List<Point> _phaserPath = new List<Point> { startPoint, endPoint };
                //if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
                return _phaserPath;
            }

            // Search!
            NavPoly[] astarPath = jsastar.astar.search(
                                    this._graph, startPoly, endPoly, 
                                    { heuristic: this._graph.navHeuristic }
                                  );

            // While the start and end polygons may be valid, no path between them
            if (astarPath.Length == 0) return null;

            // jsastar drops the first point from the path, but the funnel algorithm needs it
            astarPath.unshift(startPoly);

            // We have a path, so now time for the funnel algorithm
            Channel channel = new Channel();
            channel.Add(startPoint);
            for (int i = 0; i < astarPath.Length - 1; i++)
            {
                NavPoly navPolygon = astarPath[i];
                NavPoly nextNavPolygon = astarPath[i + 1];

                // Find the portal
                PhaserLine portal = null;
                for (int j = 0; j < navPolygon.neighbors.Count; j++)
                {
                    if (navPolygon.neighbors[j].id == nextNavPolygon.id)
                    {
                        portal = navPolygon.portals[j];
                    }
                }

                // Push the portal vertices into the channel
                channel.Add(portal.Start, portal.End);
            }
            channel.Add(endPoint);

            // Pull a string along the channel to run the funnel
            channel.stringPull();

            // Clone path, excluding duplicates
            Point? lastPoint = null;
            List<Point> phaserPath = new List<Point>();
            foreach (var p in channel.path)
            {
                //var newPoint = p.clone();
                var newPoint = p;
                //if (!lastPoint || !newPoint.equals(lastPoint)) 
                if (lastPoint.HasValue == false || newPoint != lastPoint)
                    phaserPath.Add(newPoint);
                lastPoint = newPoint;
            }

            /*
            // Call debug drawing
            if (drawPolyPath) {
                const polyPath = astarPath.map(elem => elem.centroid);
                this.debugDrawPath(polyPath, 0x00ff00, 5);
            }
            if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
            */

            return phaserPath;
        }

        private void _calculateNeighbors()
        {
            // Fill out the neighbor information for each navpoly
            for (int i = 0; i < this._navPolygons.Count; i++)
            {
                var navPoly = this._navPolygons[i];

                for (int j = i + 1; j < this._navPolygons.Count; j++)
                {
                    var otherNavPoly = this._navPolygons[j];

                    // Check if the other navpoly is within range to touch
                    //var d = navPoly.centroid.distance(otherNavPoly.centroid);
                    var d = RCommonFRB.Geometry.Distance(ref navPoly.centroid, ref otherNavPoly.centroid);
                    if (d > navPoly.boundingRadius + otherNavPoly.boundingRadius)
                        continue;

                    // The are in range, so check each edge pairing
                    foreach (var edge in navPoly.edges)
                    {
                        foreach (var otherEdge in otherNavPoly.edges)
                        {
                            // If edges aren't collinear, not an option for connecting navpolys
                            if ( !Utils.areCollinear(edge, otherEdge) )
                                continue;

                            // If they are collinear, check if they overlap
                            var overlap = this._getSegmentOverlap(edge, otherEdge);
                            if ( overlap == null )
                                continue;

                            // Connections are symmetric!
                            navPoly.neighbors.Add(otherNavPoly);
                            otherNavPoly.neighbors.Add(navPoly);

                            // Calculate the portal between the two polygons - this needs to be in
                            // counter-clockwise order, relative to each polygon
                            //const [p1, p2] = overlap;
                            var p1 = overlap[0];
                            var p2 = overlap[1];
                            //var edgeStartAngle = navPoly.centroid. angle(edge.Start);
                            var edgeStartAngle = RUtils.Angle(ref navPoly.centroid, ref edge.Start);
                            //var a1 = navPoly.centroid.angle(overlap[0]);
                            var a1 = RUtils.Angle(ref navPoly.centroid, ref overlap[0]);
                            //var a2 = navPoly.centroid.angle(overlap[1]);
                            var a2 = RUtils.Angle(ref navPoly.centroid, ref overlap[1]);
                            var d1 = Utils.angleDifference(edgeStartAngle, a1);
                            var d2 = Utils.angleDifference(edgeStartAngle, a2);
                            if (d1 < d2)
                            {
                                navPoly.portals.Add(new PhaserLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y));
                            }
                            else
                            {
                                navPoly.portals.Add(new PhaserLine((float)p2.X, (float)p2.Y, (float)p1.X, (float)p1.Y));
                            }
                        }
                    }
                }
            }

        }

        // Check two collinear line segments to see if they overlap by sorting the points.
        // Algorithm source: http://stackoverflow.com/a/17152247
        private Point[] _getSegmentOverlap(PhaserLine line1, PhaserLine line2) // returns bool ?
        {
            var points = new LineAndPoint[]
            {
                new LineAndPoint { line = line1, point = line1.Start },
                new LineAndPoint { line = line1, point = line1.End },
                new LineAndPoint { line = line2, point = line2.Start },
                new LineAndPoint { line = line2, point = line2.End }
            };
            /*points.sort(function(a, b) {
                if (a.point.x < b.point.x) return -1;
                else if (a.point.x > b.point.x) return 1;
                else {
                    if (a.point.y < b.point.y) return -1;
                    else if (a.point.y > b.point.y) return 1;
                    else return 0;
                }
            });*/
            Array.Sort(
                points, 
                (a, b) => 
                {
                if (a.point.X < b.point.X) return -1;
                else if (a.point.X > b.point.X) return 1;
                else {
                    if (a.point.Y < b.point.Y) return -1;
                    else if (a.point.Y > b.point.Y) return 1;
                    else return 0;
                }
            });
            // If the first two points in the array come from the same line, no overlap
            bool noOverlap = points[0].line == points[1].line;
            // If the two middle points in the array are the same coordinates, then there is a
            // single point of overlap.
            bool singlePointOverlap = points[1].point == points[2].point;
            if (noOverlap || singlePointOverlap)
                return null;
            else
                return new Point[] { points[1].point, points[2].point };
        }

        /*
        * Project a point onto a polygon in the shortest distance possible.
        * 
        * @param {Phaser.Point} point The point to project
        * @param {NavPoly} navPoly The navigation polygon to test against
        * @returns {{point: Phaser.Point, distance: number}}
        * 
        * @private
        * @memberof NavMesh
        */
        /// <summary>Project a point onto a polygon in the shortest distance possible.</summary>
        /// <param name="point">The point to project</param>
        /// <param name="navPoly">The navigation polygon to test against</param>
        /// <returns></returns>
        private Tuple<Point?, float> _projectPointToPolygon(Point point, NavPoly navPoly)
        {
            Point? closestProjection = null;
            var closestDistance = float.MaxValue;
            foreach (var edge in navPoly.edges) {
                var projectedPoint = this._projectPointToEdge(point, edge);
                //var d = point.distance(projectedPoint);
                var d = (float)RCommonFRB.Geometry.Distance(ref point, ref projectedPoint);
                if (closestProjection == null || d < closestDistance) {
                    closestDistance = d;
                    closestProjection = projectedPoint;
                }
            }
            return Tuple.Create(closestProjection, closestDistance);
        }

        private double _distanceSquared(Point a, Point b) {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Project a point onto a line segment
        /// JS Source: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment</summary>
        /// </summary>
        private Point _projectPointToEdge(Point point, PhaserLine line) {
            Point a = line.Start;
            Point b = line.End;
            // Consider the parametric equation for the edge's line, p = a + t (b - a). We want to find
            // where our point lies on the line by solving for t:
            //  t = [(p-a) . (b-a)] / |b-a|^2
            double l2 = this._distanceSquared(a, b);
            var t = ((point.X - a.X) * (b.X - a.X) + (point.Y - a.Y) * (b.Y - a.Y)) / l2;
            // We clamp t from [0,1] to handle points outside the segment vw.
            //t = Phaser.Math.clamp(t, 0, 1);
            t = RUtils.Clamp(t, 0, 1);
            // Project onto the segment
            var p = new Point(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
            return p;
        }



    }
}
