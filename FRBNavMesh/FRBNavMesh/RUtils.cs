using FlatRedBall.Math.Geometry;
using Xna = Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    internal class RUtils
    {
        public static double Angle(ref Point point1, ref Point point2)
        {
            return System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }

        /*
        * Force a value within the boundaries by clamping it to the range `min`, `max`.
        *
        * @method Phaser.Math#clamp
        * @param {float} v - The value to be clamped.
        * @param {float} min - The minimum bounds.
        * @param {float} max - The maximum bounds.
        * @return {number} The clamped value.
        */
        /// <summary>Force a value within the boundaries by clamping it to the range `min`, `max`.</summary>
        /// <param name="v">The value to be clamped.</param>
        /// <param name="min">The minimum bounds.</param>
        /// <param name="max">The maximum bounds.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float v, float min, float max) {

            if (v < min)
            {
                return min;
            }
            else if (max < v)
            {
                return max;
            }
            else
            {
                return v;
            }
        }

        /// <summary>Force a value within the boundaries by clamping it to the range `min`, `max`.</summary>
        /// <param name="v">The value to be clamped.</param>
        /// <param name="min">The minimum bounds.</param>
        /// <param name="max">The maximum bounds.</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(double v, double min, double max) {

            if (v < min)
            {
                return min;
            }
            else if (max < v)
            {
                return max;
            }
            else
            {
                return v;
            }
        }

        /// <summary>Calculates 2D distance of Vector3 (3D point) and Point (2D point). Using X and Y components.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static double Distance2D(ref Xna.Vector3 pointA, ref Point pointB)
		{
            double X = pointA.X - pointB.X;
            double Y = pointA.Y - pointB.Y;
			return Math.Abs( Math.Sqrt( X * X + Y * Y ) );
		}

        /// <summary>Calculates 2D distance of two Vector3 (3D points). Using their X and Y components.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static double Distance2D(ref Xna.Vector3 pointA, ref Xna.Vector3 pointB)
		{
            float X = pointA.X - pointB.X;
            float Y = pointA.Y - pointB.Y;
			return Math.Abs( Math.Sqrt( X * X + Y * Y ) );
		}
    }
}
