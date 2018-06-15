using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

using RCommonFRB;

using System.Diagnostics;
using D = System.Diagnostics.Debug;
//using FDebug = FRBNavMesh.Debug;

namespace FRBNavMesh
{
    //using FDebug = FRBNavMesh.Debug;

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

    public class RLineAndPoint
    {
        public SimpleLine line;
        public Point point;
    }

    /// <summary>
    /// R: Almost FIN
    /// </summary>
    public class NavMesh<TNode, TLink>
        where TNode : PositionedNodeBase<TLink, TNode>, new()
        where TLink : LinkBase<TLink, TNode>, new()
    {
        //private int _meshShrinkAmount;
        private List<TNode> _NavPolygons;
        //private NavGraph _Graph; // just for javascript-astar

        /// <summary></summary>
        /// <param name="polygons"></param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        public NavMesh(List<AxisAlignedRectangle> polygons /*, int meshShrinkAmount = 0*/)
        {
            //_meshShrinkAmount = meshShrinkAmount;

            // Construct NavPoly instances for each polygon
            _NavPolygons = new List<TNode>();
            for (int i = 0; i < polygons.Count; i++)
            {
                this._NavPolygons.Add( PositionedNodeBase<TLink, TNode>.Create(i, polygons[i]) );
            }

            this._CalculateNeighbors();

            // Astar graph of connections between polygons
            //this._Graph = new NavGraph(this._NavPolygons);
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
        //public List<Point> findPath(Point startPoint, Point endPoint, bool drawPolyPath = false, bool drawFinalPath = false)
        //{
        //    NavPoly startPoly = null;
        //    NavPoly endPoly = null;
        //    float startDistance = float.MaxValue;
        //    float endDistance = float.MaxValue;
        //    float d, r;

        //    // Find the closest poly for the starting and ending point
        //    foreach (var navPoly in this._NavPolygons) {
        //        r = navPoly.Polygon.BoundingRadius;
        //        // Start
        //        //d = navPoly.centroid.Distance(startPoint);
        //        d = (float)RCommonFRB.Geometry.Distance2D(ref navPoly.Polygon.Position, ref startPoint);
        //        if (d <= startDistance && d <= r && navPoly.contains(startPoint)) {
        //            startPoly = navPoly;
        //            startDistance = d;
        //        }
        //        // End
        //        //d = navPoly.centroid.Distance(endPoint);
        //        d = (float)RCommonFRB.Geometry.Distance2D(ref navPoly.Polygon.Position, ref endPoint);
        //        if (d <= endDistance && d <= r && navPoly.contains(endPoint))
        //        {
        //            endPoly = navPoly;
        //            endDistance = d;
        //        }
        //    }

        //    #region    If the start point wasn't inside a polygon, run a more liberal check that allows a point
        //    /*
        //        // If the start point wasn't inside a polygon, run a more liberal check that allows a point
        //        // to be within meshShrinkAmount radius of a polygon
        //        if (!startPoly && this._meshShrinkAmount > 0)
        //        {
        //            for (const navPoly of this._navPolygons) 
        //            {
        //                // Check if point is within bounding circle to avoid extra projection calculations
        //                r = navPoly.boundingRadius + this._meshShrinkAmount;
        //                d = navPoly.centroid.distance(startPoint);
        //                if (d <= r) {
        //                    // Check if projected point is within range of a polgyon and is closer than the
        //                    // previous point
        //                    const { distance } = this._projectPointToPolygon(startPoint, navPoly);
        //                    if (distance <= this._meshShrinkAmount && distance < startDistance) {
        //                        startPoly = navPoly;
        //                        startDistance = distance;
        //                    }
        //                }
        //            }
        //        }
        //        */

        //    /*
        //    // Same check as above, but for the end point
        //    if (!endPoly && this._meshShrinkAmount > 0) {
        //        for (const navPoly of this._navPolygons) {
        //            r = navPoly.boundingRadius + this._meshShrinkAmount;
        //            d = navPoly.centroid.distance(endPoint);
        //            if (d <= r) {
        //                const { distance } = this._projectPointToPolygon(endPoint, navPoly);
        //                if (distance <= this._meshShrinkAmount && distance < endDistance) {
        //                    endPoly = navPoly;
        //                    endDistance = distance;
        //                }
        //            }
        //        }
        //    }
        //    */ 
        //    #endregion If the start point wasn't inside a polygon, run a more liberal check that allows a point END

        //    // No matching polygons locations for the start or end, so no path found
        //    // R: = start or end point not on nav mesh
        //    if (startPoly == null || endPoly == null) return null;

        //    // If the start and end polygons are the same, return a direct path
        //    if (startPoly == endPoly)
        //    {
        //        List<Point> _phaserPath = new List<Point> { startPoint, endPoint };
        //        //if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
        //        return _phaserPath;
        //    }

        //    // Search!
        //    /*NavPoly[] astarPath = jsastar.astar.search(
        //                            this._graph, startPoly, endPoly, 
        //                            { heuristic: this._graph.navHeuristic }
        //                          );*/
        //    NavPoly[] astarPath = JavasciptAstar.Astar.search(
        //                                this._Graph, startPoly, endPoly, 
        //                                new JavasciptAstar.Astar.Options<NavPoly> { heuristic = this._Graph.navHeuristic }
        //                          );

        //    // While the start and end polygons may be valid, no path between them
        //    if (astarPath.Length == 0) return null;

        //    // jsastar drops the first point from the path, but the funnel algorithm needs it
        //    astarPath.unshift(startPoly);

        //    // We have a path, so now time for the funnel algorithm
        //    Channel channel = new Channel();
        //    channel.Add(startPoint);
        //    for (int i = 0; i < astarPath.Length - 1; i++)
        //    {
        //        NavPoly navPolygon = astarPath[i];
        //        NavPoly nextNavPolygon = astarPath[i + 1];

        //        // Find the portal
        //        SimpleLine portal = null;
        //        for (int j = 0; j < navPolygon.Neighbors.Count; j++)
        //        {
        //            if (navPolygon.Neighbors[j].Id == nextNavPolygon.Id)
        //            {
        //                portal = navPolygon.Portals[j];
        //            }
        //        }

        //        // Push the portal vertices into the channel
        //        channel.Add(portal.Start, portal.End);
        //    }
        //    channel.Add(endPoint);

        //    // Pull a string along the channel to run the funnel
        //    channel.stringPull();

        //    // Clone path, excluding duplicates
        //    Point? lastPoint = null;
        //    List<Point> phaserPath = new List<Point>();
        //    foreach (var p in channel.path)
        //    {
        //        //var newPoint = p.clone();
        //        var newPoint = p;
        //        //if (!lastPoint || !newPoint.equals(lastPoint)) 
        //        if (lastPoint.HasValue == false || newPoint != lastPoint)
        //            phaserPath.Add(newPoint);
        //        lastPoint = newPoint;
        //    }

        //    /*
        //    // Call debug drawing
        //    if (drawPolyPath) {
        //        const polyPath = astarPath.map(elem => elem.centroid);
        //        this.debugDrawPath(polyPath, 0x00ff00, 5);
        //    }
        //    if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
        //    */

        //    return phaserPath;
        //}

        private void _CalculateNeighbors()
        {
            D.WriteLine(" * NavMesh._CalculateNeighbors()");

            // Fill out the neighbor information for each navpoly
            TNode navPoly;
            TNode otherNavPoly;
            AxisAlignedRectangle navPolyPolygon;
            AxisAlignedRectangle otherNavPolyPolygon;
            for (int i = 0; i < _NavPolygons.Count; i++)
            {
                navPoly = _NavPolygons[i];
                navPolyPolygon = navPoly.Polygon;

                D.WriteLine("   navPoly: " + navPolyPolygon.Name);

                for (int j = i + 1; j < _NavPolygons.Count; j++)
                {
                    otherNavPoly = _NavPolygons[j];
                    otherNavPolyPolygon = otherNavPoly.Polygon;

                    D.WriteLine("     otherNavPoly: " + otherNavPolyPolygon.Name);

                    // Check if the other navpoly is within range to touch
                    // Distance between centers
                    var distanceBetweenCenters = RCommonFRB.Geometry.Distance2D(ref navPolyPolygon.Position, ref otherNavPolyPolygon.Position);
                    // If Distance between centers is bigger than combined radii, they are not in range
                    // If Distance between centers is smaller or equal, they are in range
                    // * Like that they are in touch even when touching just by corners !
                    if (distanceBetweenCenters >= navPolyPolygon.BoundingRadius + otherNavPolyPolygon.BoundingRadius)
                    {
                        // Not in range => proceed to another navpoly
                        D.WriteLine($"       Not in range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navPolyPolygon.BoundingRadius + otherNavPolyPolygon.BoundingRadius})");
                        continue;
                    }

                    D.WriteLine($"       In range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navPolyPolygon.BoundingRadius + otherNavPolyPolygon.BoundingRadius})");

                    // The are in range, so check each edge pairing
                    // to find shared edge and shared part of edges = portal

                    #region    - Using areCollinear and SegmentOverlap
                    /*foreach (var edge in navPoly.Edges)
                    {
                        foreach (var otherEdge in otherNavPoly.Edges)
                        {
                            // If edges aren't collinear, not an option for connecting navpolys
                            if ( !Utils.areCollinear(edge, otherEdge) )
                                continue;

                            // If they are collinear, check if they overlap
                            var overlap = this._GetSegmentOverlap(edge, otherEdge);
                            if ( overlap == null )
                                continue;

                            // Connections are symmetric!
                            navPoly.Neighbors.Add(otherNavPoly);
                            otherNavPoly.Neighbors.Add(navPoly);

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
                                navPoly.Portals.Add(new SimpleLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y));
                            }
                            else
                            {
                                navPoly.Portals.Add(new SimpleLine((float)p2.X, (float)p2.Y, (float)p1.X, (float)p1.Y));
                            }
                        }
                    }*/
                    #endregion - Using areCollinear and SegmentOverlap END

                    #region    - Using common sense
                    // Here I know they do overlap
                    //SimpleLine navPolyEdge;
                    //SimpleLine otherNavPolyEdge;
                    SimpleLine portal;
                    // other is above
                    if (navPolyPolygon.Top == otherNavPolyPolygon.Bottom)
                    {
                        D.WriteLine($"       Other above -- Touching this Top ({navPolyPolygon.Top}) - other Bottom ({otherNavPolyPolygon.Bottom})");

                        //navPolyEdge = navPoly.EdgeTop;
                        //otherNavPolyEdge = otherNavPoly.EdgeBottom;
                        portal = _GetSegmentOverlapHorisontal(navPoly.EdgeTop, otherNavPoly.EdgeBottom);
                    }
                    // other is below
                    else if (navPolyPolygon.Bottom == otherNavPolyPolygon.Top)
                    {
                        D.WriteLine($"       Other below -- Touching this Bottom ({navPolyPolygon.Bottom}) - other Top ({otherNavPolyPolygon.Top})");

                        //navPolyEdge = navPoly.EdgeBottom;
                        //otherNavPolyEdge = otherNavPoly.EdgeTop;
                        portal = _GetSegmentOverlapHorisontal(navPoly.EdgeBottom, otherNavPoly.EdgeTop);
                    }
                    // other is to left
                    else if (navPolyPolygon.Left == otherNavPolyPolygon.Right)
                    {
                        D.WriteLine($"       Other to left -- Touching this Left ({navPolyPolygon.Left}) - other Right ({otherNavPolyPolygon.Right})");

                        //navPolyEdge = navPoly.EdgeLeft;
                        //otherNavPolyEdge = otherNavPoly.EdgeRight;
                        portal = _GetSegmentOverlapVertical(navPoly.EdgeLeft, otherNavPoly.EdgeRight);
                    }
                    // other is to right
                    else if (navPolyPolygon.Right == otherNavPolyPolygon.Left)
                    {
                        D.WriteLine($"       Other to right -- Touching this Right ({navPolyPolygon.Right}) - other Left ({otherNavPolyPolygon.Left})");

                        //navPolyEdge = navPoly.EdgeRight;
                        //otherNavPolyEdge = otherNavPoly.EdgeLeft;
                        portal = _GetSegmentOverlapVertical(navPoly.EdgeRight, otherNavPoly.EdgeLeft);
                    }
                    // not touching
                    else
                    {
                        // Not actually touching => proceed to another navpoly
                        D.WriteLine($"       Not Touching");
                        continue;
                    }

                    //var portal = _GetSegmentOverlap(navPoly.EdgeTop, otherNavPoly.EdgeBottom);
                    navPoly.LinkTo( otherNavPoly, portal );
                    Debug.ShowLine(portal, Color.Yellow);
                    Debug.ShowLine(navPolyPolygon.Position, otherNavPolyPolygon.Position, Color.Gray);
                    #endregion - Using common sense END
                }
            }

        }

        /// <summary>Check two collinear line segments to see if they overlap by sorting the points.</summary>
        /// <param name="line1">Polygon's edge</param>
        /// <param name="line2">Other polygon's edge</param>
        /// <returns>Line Segment of edges where they overlap or null id they don't overlap.</returns>
        /// <remarks>
        /// Algorithm source: http://stackoverflow.com/a/17152247
        /// </remarks>
        private SimpleLine _GetSegmentOverlap(SimpleLine line1, SimpleLine line2) 
        {
            var points = new RLineAndPoint[]
            {
                new RLineAndPoint { line = line1, point = line1.Start },
                new RLineAndPoint { line = line1, point = line1.End },
                new RLineAndPoint { line = line2, point = line2.Start },
                new RLineAndPoint { line = line2, point = line2.End }
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
                    else
                    {
                        if (a.point.Y < b.point.Y) return -1;
                        else if (a.point.Y > b.point.Y) return 1;
                        else return 0;
                    }
                }
            );
            // If the first two points in the array come from the same line, no overlap
            bool noOverlap = points[0].line == points[1].line;
            // If the two middle points in the array are the same coordinates, then there is a
            // single point of overlap.
            bool singlePointOverlap = points[1].point == points[2].point;
            if (noOverlap || singlePointOverlap)
                return null;
            else
                return new SimpleLine(points[1].point, points[2].point);
        }

        private SimpleLine _GetSegmentOverlapVertical(SimpleLine edge, SimpleLine othersEdge) 
        {
            // * Svisle edged are always top (Start) to bottom (End)
            // I only need Ys
            return new SimpleLine(
                edge.Start.X, 
                Math.Min(edge.Start.Y, othersEdge.Start.Y), 
                edge.Start.X, 
                Math.Max(edge.End.Y, othersEdge.End.Y)
            );
        }
        private SimpleLine _GetSegmentOverlapHorisontal(SimpleLine edge, SimpleLine othersEdge) 
        {
            // * Vodorovne edged are always left (Start) to right (End)
            // I only need Xes
            return new SimpleLine(
                Math.Max(edge.Start.X, othersEdge.Start.X), 
                edge.Start.Y, 
                Math.Min(edge.End.X, othersEdge.End.X),
                edge.Start.Y
            );
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
            foreach (var edge in navPoly.Edges) {
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
        private Point _projectPointToEdge(Point point, SimpleLine line) {
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


        #region    --- Debug permanent

        

        #endregion --- Debug permanent END
    }
}
