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
        /// <param name="startX">The x coordinate of the start of the line.</param>
        /// <param name="startY">The y coordinate of the start of the line.</param>
        /// <param name="endX">The x coordinate of the end of the line.</param>
        /// <param name="endY">The y coordinate of the end of the line.</param>
        public SimpleLine(double startX, double startY, double endX, double endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        /// <summary></summary>
        /// <param name="start">Starting Point of line.</param>
        /// <param name="end">End Point of line.</param>
        public SimpleLine(ref Point start, ref Point end)
        {
            Start.X = start.X;
            Start.Y = start.Y;
            End.X = end.X;
            End.Y = end.Y;
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
        public readonly Point Start;
        /// <summary>The end point of the line.</summary>
        public readonly Point End;

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
