﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    public static class PointExtensions
    {
        public static Vector3 MultiplyByPointAndNormalize(this Matrix4x4 matrix, Vector4 point)
        {
            float X = point.X * matrix.M11 + point.Y * matrix.M12 + point.Z * matrix.M13 + point.W * matrix.M14;
            float Y = point.X * matrix.M21 + point.Y * matrix.M22 + point.Z * matrix.M23 + point.W * matrix.M24;
            float Z = point.X * matrix.M31 + point.Y * matrix.M32 + point.Z * matrix.M33 + point.W * matrix.M34;
            float W = point.X * matrix.M41 + point.Y * matrix.M42 + point.Z * matrix.M43 + point.W * matrix.M44;

            return new Vector3(X / W, Y / W, Z / W);
        }
        public static Vector4 fromSphericalCoordinates(float R, float t, float f)
        {
            return new Vector4(
                R * (float)(Math.Sin(t) * Math.Cos(f)),
                R * (float)(Math.Sin(t) * Math.Sin(f)),
                R * (float)Math.Cos(t), 1);
        }
        public static Vector4 fromPolarCoordinates(float R, float t, float z)
        {
            return new Vector4(
                R * (float)Math.Cos(t),
                R * (float)Math.Sin(t), z, 1);
        }
    }
}
