using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BilardGame
{
    static class Transformation
    {
        public static void Scale(this Model model, float scalar)
        {
            model.Transform(new Matrix4x4
            (
                scalar, 0, 0, 0,
                0, scalar, 0, 0,
                0, 0, scalar, 0,
                0, 0, 0, 1
            ));
        }
        public static void Move(this Model model, float Xdist, float Ydist, float Zdist)
        {
            model.Transform(new Matrix4x4
            (
                1, 0, 0, Xdist,
                0, 1, 0, Ydist,
                0, 0, 1, Zdist,
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
