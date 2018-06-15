using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    public class Debug
    {
        // Debug color palette
        public static readonly Color[] palette = { new Color(0x00, 0xa0, 0xb0), new Color(0x6a, 0x4a, 0x3c), new Color(0xc, 0xc33, 0x3f), new Color(0xeb, 0x68, 0x41), new Color(0xed, 0xc9, 0x51) };

        public static void ShowLine(SimpleLine line, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(line.Start.X, line.Start.Y), new Point3D(line.End.X, line.End.Y));
            visLine.Color = color;
            visLine.Visible = true;
        }
        public static void ShowLine(Vector3 point1, Vector3 point2, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(point1, point2);
            visLine.Color = color;
            visLine.Visible = true;
        }
    }
}
