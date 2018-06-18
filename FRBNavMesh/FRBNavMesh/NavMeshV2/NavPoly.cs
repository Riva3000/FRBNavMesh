﻿using FlatRedBall.Math.Geometry;
using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RCommonFRB;

namespace FRBNavMesh
{
    /*
    * A class that represents a navigable polygon in a navmesh. It is build from a Phaser.Polygon. It
    * has a drawing function to help visualize it's features:
    *  - polygon
    *  - neighbors - any navpolys that can be reached from this navpoly
    *  - portals - overlapping edges between neighbors
    *  - centroid - not a true centroid, just an approximation.
    *  - boundingRadius - the radius of a circle at the centroid that fits all the points of the poly 
    * 
    * It implements the properties and fields that javascript-astar needs - weight, toString, isWall
    * and getCost. See GPS test from astar repo for structure: 
    * https://github.com/bgrins/javascript-astar/blob/master/test/tests.js
    *
    * @class NavPoly
    * @private
    */
    /// <summary>
    /// A class that represents a navigable polygon in a navmesh. It is build from a Phaser.Polygon. It
    /// has a drawing function to help visualize it's features:
    /// - polygon
    /// - neighbors - any navpolys that can be reached from this navpoly
    /// - portals - overlapping edges between neighbors
    /// - centroid - not a true centroid, just an approximation.
    /// - boundingRadius - the radius of a circle at the centroid that fits all the points of the poly 
    /// 
    /// It implements the properties and fields that javascript-astar needs - weight, toString, isWall
    /// and getCost.
    /// </summary>
    class NavPoly
    {
        /*
        * Some internal terminology usage:
        * 
        * - neighbor: a polygon that shares part of an edge with another polygon
        * - portal: when two neighbor's have edges that overlap, the portal is the overlapping line segment
        * - channel: the path of polygons from starting point to end point
        * - pull the string: run the funnel algorithm on the channel so that the path hugs the edges of the
        *   channel. Equivalent to having a string snaking through a hallway and then pulling it taut.
        */

        public int Id;
        AxisAlignedRectangle Polygon;
        public SimpleLine[] Edges;
        public List<NavPoly> Neighbors;
        public List<SimpleLine> Portals;
        public Point centroid;
        public float boundingRadius;
        int weight; // jsastar property

        Color _color;




        /*
        * Creates an instance of NavPoly.
        * @param {Phaser.Game} game 
        * @param {number} id 
        * @param {Phaser.Polygon} polygon 
        * 
        * @memberof NavPoly
        */
        public NavPoly(int id, AxisAlignedRectangle polygon) {
            //this.game = game;
            this.Id = id;
            this.Polygon = polygon;
            this.Edges = this._calculateEdges();
            this.Neighbors = new List<NavPoly>();
            this.Portals = new List<SimpleLine>();

            //this.centroid = this._calculateCentroid();
            this.centroid = new Point(polygon.X, polygon.Y);

            this.boundingRadius = this._calculateRadius();

            this.weight = 1; // jsastar property

            int i = this.Id % Debug.palette.Length;
            this._color = Debug.palette[i];
        }





        // -- Public methods
        public bool contains(Point point) {
            // Phaser's polygon check doesn't handle when a point is on one of the edges of the line. Note:
            // check numerical stability here. It would also be good to optimize this for different shapes.
            return Polygon.IsPointOnOrInside(ref point); // || this._isPointOnEdge(point);
        }


        // -- jsastar (public) methods 
        public override string ToString()
        {
            return $"NavPoly(id: {this.Id} at: {this.centroid})";
        }
        public bool isWall() {
            return this.weight == 0;
        }
        public double centroidDistance(NavPoly navPolygon) {
            //return this.centroid.distance(navPolygon.centroid);
            return RCommonFRB.Geometry.Distance(ref centroid, ref navPolygon.centroid);
        }
        public float getCost(NavPoly navPolygon) {
            return (float)this.centroidDistance(navPolygon);
        }


        // -- Private methods
        /// <summary>R: FIN. Maybe not needed.</summary>
        /// <returns>This NavPoly's AARect's all edges as array of Line-s.</returns>
        private SimpleLine[] _calculateEdges()
        {
            /*// -- R: FRB version v1 - not efficient
            var points = this.polygon.points;
            var edges = new List<Line>();
            for (int i = 1; i < points.length; i++)
            {
                var p1 = points[i - 1];
                var p2 = points[i];
                edges.Add(new Line(p1.x, p1.y, p2.x, p2.y));
            }
            var first = points[0];
            var last = points[points.length - 1];
            edges.Add(new Line(first.x, first.y, last.x, last.y));
            return edges.ToArray();*/

            /*// -- R: FRB version v2 - not sure if right - vertices order may be important
            var edges = new PhaserLine[4];
            PhaserLine edge;
            // Left
            edge = new PhaserLine();
            edge.Position.X = polygon.Left;
            edge.Position.Y = polygon.Position.Y;
            edges[0] = edge;
            // Top
            edge = new PhaserLine();
            edge.Position.X = polygon.Position.X;
            edge.Position.Y = polygon.Top;
            edges[1] = edge;
            // Right
            edge = new PhaserLine();
            edge.Position.X = polygon.Right;
            edge.Position.Y = polygon.Position.Y;
            edges[2] = edge;
            // Bottom
            edge = new PhaserLine();
            edge.Position.X = polygon.Position.X;
            edge.Position.Y = polygon.Bottom;
            edges[3] = edge;
            */

            // -- R: FRB version v3 - precise vertices/edges order
            // R: In what order polygon was constructed:
            //                                    0 top-left    1 bottom-left    2 bottom-right    3 top-right
            //    const poly = new Phaser.Polygon(left, top,    left, bottom,    right, bottom,    right, top);

            /*
            const points = this.polygon.points;
            const edges = [];
            for (let i = 1; i < 4; i++) {
                const p1 = points[i - 1];
                const p2 = points[i];
                edges.push(new Phaser.Line(p1.x, p1.y, p2.x, p2.y));
            }
            const first = points[0];
            const last = points[3];
            edges.push(new Phaser.Line(first.x, first.y, last.x, last.y));
            return edges;
            */
            var edges = new SimpleLine[4];
            /*
            0 > > 3
            v     ^
            v     ^
            1 > > 2
            */
            // Left - top-left to bottom-left
            edges[0] = new SimpleLine(Polygon.Left, Polygon.Top,    Polygon.Left, Polygon.Bottom);
            // Bottom - bottom-left to bottom-right
            edges[1] = new SimpleLine(Polygon.Left, Polygon.Bottom,    Polygon.Right, Polygon.Bottom);
            // Right - bottom-right to top-right
            edges[2] = new SimpleLine(Polygon.Right, Polygon.Bottom,    Polygon.Right, Polygon.Top);
            // Top - top-left to top-right
            edges[3] = new SimpleLine(Polygon.Left, Polygon.Top,    Polygon.Right, Polygon.Top);

            return edges;
        }

        /// <summary>R: Not needed</summary>
        private Point _calculateCentroid()
        {
            // R: I am supporting only AARects. 
            //    Centroid of AARect is always it's center. 
            //    Center of FRB AARect is known by default.
            throw new NotNeededException();
        }

        /// <summary>R: FIN</summary>
        /// <returns>This NavMesh's polygon's bounding circle's radius</returns>
        private float _calculateRadius() {
            /*var boundingRadius = 0;
            foreach (Point point in this.polygon.points) {
                var d = this.centroid.distance(point);
                if (d > boundingRadius) boundingRadius = d;
            }
            return boundingRadius;*/

            // wrong. AARect.Top etc. return absolute (world) coordinates of edges of the AARect: return Math.Max(Polygon.Top, Polygon.Left);
            return Math.Max(Polygon.Width, Polygon.Left) / 2;
        }

        /// <summary>R: Not needed. Not referenced anywhere.</summary>
        private bool _isPointOnEdge(Point point)
        {
            // R:
            throw new NotNeededException();

            /*foreach (var edge in this.edges) {
                if (edge.pointOnSegment(point.x, point.y)) return true;
            }
            return false;*/

            if ( ( (point.X >= Polygon.Left && point.X <= Polygon.Right) && (point.Y == Polygon.Top || point.Y == Polygon.Bottom) )
                 ||
                 ( (point.Y <= Polygon.Top && point.Y >= Polygon.Bottom) && (point.X == Polygon.Left || point.X == Polygon.Right) ) )
                return true;

            return false;
        }
    }
}