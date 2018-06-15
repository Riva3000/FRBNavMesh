using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    public class SimpleLine
    {
        /// <summary></summary>
        /// <param name="x1">The x coordinate of the start of the line.</param>
        /// <param name="y1">The y coordinate of the start of the line.</param>
        /// <param name="x2">The x coordinate of the end of the line.</param>
        /// <param name="y2">The y coordinate of the end of the line.</param>
        public SimpleLine(float x1 = 0, float y1 = 0, float x2 = 0, float y2 = 0)
        {
            Start = new Point(x1, y1);
            End = new Point(x2, y2);
        }

        /// <summary></summary>
        /// <param name="x1">The x coordinate of the start of the line.</param>
        /// <param name="y1">The y coordinate of the start of the line.</param>
        /// <param name="x2">The x coordinate of the end of the line.</param>
        /// <param name="y2">The y coordinate of the end of the line.</param>
        public SimpleLine(double x1 = 0, double y1 = 0, double x2 = 0, double y2 = 0)
        {
            Start = new Point(x1, y1);
            End = new Point(x2, y2);
        }

        /// <summary></summary>
        /// <param name="start">Starting Point of line.</param>
        /// <param name="end">End Point of line.</param>
        public SimpleLine(Point start, Point end)
        {
            Start = start;
            End = end;
        }


        /// <summary>Gets the left-most point of this line.</summary>
        //float Left;
        /// <summary>Gets the right-most point of this line.</summary>
        //float Right;
        /// <summary>Gets the top-most point of this line.</summary>
        //float Top;
        /// <summary>Gets the bottom-most point of this line.</summary>
        //float Bottom;

        /// <summary>The start point of the line.</summary>
        public Point Start;
        /// <summary>The end point of the line.</summary>
        public Point End;

        /// <summary>Gets the height of this bounds of this line.</summary>
        //float Height;
        /// <summary>Gets the width of this bounds of this line.</summary>
        //float Width;

        /// <summary>Gets the length of the line segment.</summary>
        //float Length;

        /// <summary>Gets the angle of the line in radians.</summary>
        //float Angle;
        /// <summary>Gets the angle in radians of the normal of this line (line.angle - 90 degrees.)</summary>
        //float NormalAngle;
        /// <summary>Gets the x component of the left-hand normal of this line.</summary>
        //float NormalX;
        /// <summary>Gets the perpendicular slope of the line (x/y).</summary>
        //float PerpSlope;
        /// <summary>Gets the slope of the line (y/x).</summary>
        //float Slope;

        /// <summary>Gets the x coordinate of the top left of the bounds around this line.</summary>
        //float X;
        /// <summary>Gets the y coordinate of the top left of the bounds around this line.</summary>
        //float Y;

    }
}
