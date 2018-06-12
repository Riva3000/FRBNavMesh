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
                new Portal {
                    left = p1,
                    right = p2.Value
                } 
            );
        }

        public Nevim stringPull()
        {
            // R ...

            return new Nevim();
        }
    }
}
