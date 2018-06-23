using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point = FlatRedBall.Math.Geometry.Point;

namespace FRBNavMesh
{
    public class Debug
    {
        // Debug color palette
        public static readonly Color[] palette = { new Color(0x00, 0xa0, 0xb0), new Color(0x6a, 0x4a, 0x3c), new Color(0xc, 0xc33, 0x3f), new Color(0xeb, 0x68, 0x41), new Color(0xed, 0xc9, 0x51) };

        public static readonly Color Gray32 = Color.FromNonPremultiplied(32, 32, 32, 255);
        public static readonly Color Gray64 = Color.FromNonPremultiplied(64, 64, 46, 255);
        public static readonly Color Gray96 = Color.FromNonPremultiplied(96, 96, 96, 255);
        public static readonly Color Gray128 = Color.FromNonPremultiplied(128, 128, 128, 255);
        public static readonly Color NiceRed = Color.FromNonPremultiplied(202, 46, 0, 255);
        public static readonly Color NiceBlue = Color.FromNonPremultiplied(21, 101, 181, 255);


        public static void ShowLine(SimpleLine line, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(line.Start.X, line.Start.Y), new Point3D(line.End.X, line.End.Y));
            visLine.Color = color;
            //visLine.Visible = true;
        }
        public static void ShowLine(Vector3 point1, Vector3 point2, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(point1, point2);
            visLine.Color = color;
            //visLine.Visible = true;
        }
        public static void ShowLine(double x1, double y1, double x2, double y2, Color color)
        {
            var visLine = ShapeManager.AddLine();
            visLine.SetFromAbsoluteEndpoints(new Point3D(x1, y1), new Point3D(x2, y2));
            visLine.Color = color;
            //visLine.Visible = true;
        }

        private static readonly Vector3 _TEXT_POS_SHIFT = new Vector3(1f, 1f, 20f);
        public static void ShowText(ref Vector3 position, string text)
        {
            var textObj = FlatRedBall.Graphics.TextManager.AddText(text);
            textObj.HorizontalAlignment = FlatRedBall.Graphics.HorizontalAlignment.Left;
            textObj.VerticalAlignment = FlatRedBall.Graphics.VerticalAlignment.Top;
            textObj.Position = position + _TEXT_POS_SHIFT;
        }

        // ------- Temp debug --------
        private void Tests()
        {
            //object.ReferenceEquals
        }
    }
}
