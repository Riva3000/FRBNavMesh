using FlatRedBall.Math.Geometry;
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
    }
}
