using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    public static class Transformations
    {
        public static void Scale(this Model model, float sx, float sy, float sz)
        {
            model.Transform(new Matrix4x4
            (
                sx, 0, 0, 0,
                0, sy, 0, 0,
                0, 0, sz, 0,
                0, 0, 0, 1
            ));
        }
        public static void Translate(this Model model, float Tx, float Ty, float Tz)
        {
            model.Transform(new Matrix4x4
            (
                1, 0, 0, Tx,
                0, 1, 0, Ty,
                0, 0, 1, Tz,
                0, 0, 0, 1
            ));
        }
        public static void RotateX(this Model model, float angle)
        {
            model.Transform(new Matrix4x4
            (
                1, 0, 0, 0,
                0, (float)Math.Cos(angle), -(float)Math.Sin(angle), 0,
                0, (float)Math.Sin(angle), (float)Math.Cos(angle), 0,
                0, 0, 0, 1
            ));
        }
        public static void RotateY(this Model model, float angle)
        {
            model.Transform(new Matrix4x4
            (
                (float)Math.Cos(angle), 0, (float)Math.Sin(angle), 0,
                0, 1, 0, 0,
                -(float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0,
                0, 0, 0, 1
            ));
        }
        public static void RotateZ(this Model model, float angle)
        {
            model.Transform(new Matrix4x4
            (
                (float)Math.Cos(angle), -(float)Math.Sin(angle), 0, 0,
                (float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            ));
        }
    }
}
