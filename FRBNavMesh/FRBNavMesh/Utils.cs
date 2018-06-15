using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    /// <summary>R: FIN</summary>
    static class Utils
    {
        /*
        * Twice the area of the triangle formed by a, b and c
        * @private
        */
        /// <summary>Twice the area of the triangle formed by a, b and c</summary>
        /// <returns>Twice the area of the triangle formed by a, b and c</returns>
        public static double triarea2(Point a, Point b, Point c) {
            double ax = b.X - a.X;
            double ay = b.Y - a.Y;
            double bx = c.X - a.X;
            double by = c.Y - a.Y;
            return bx * ay - ax * by;
        }

        /*
        * @private
        */
        public static bool almostEqual(double value1, double value2, double errorMargin = 0.0001f)
        {
            if (Math.Abs(value1 - value2) <= errorMargin)
                return true;

            return false;
        }

        /*
        * https://gist.github.com/Aaronduino/4068b058f8dbc34b4d3a9eedc8b2cbe0
        * @private
        */
        /// <summary>https://gist.github.com/Aaronduino/4068b058f8dbc34b4d3a9eedc8b2cbe0</summary>
        public static double angleDifference(double x, double y)
        {
            double a = x - y;
            var i = a + Math.PI;
            var j = Math.PI * 2;
            a = i - Math.Floor(i / j) * j; // (a+180) % 360; this ensures the correct sign
            a -= Math.PI;
            return a;
        }

        /*
        * @private
        */
        public static bool areCollinear(SimpleLine line1, SimpleLine line2, float errorMargin = 0.0001f) {
            // Figure out if the two lines are equal by looking at the area of the triangle formed
            // by their points
            double area1 = triarea2(line1.Start, line1.End, line2.Start);
            double area2 = triarea2(line1.Start, line1.End, line2.End);

            if ( almostEqual(area1, 0, errorMargin) && almostEqual(area2, 0, errorMargin) )
                return true;

            return false;
        }
    }
}
