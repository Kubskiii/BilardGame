using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    public static class PointExtensions
    {
        public static Vector4 Multiply(this Matrix4x4 matrix, Vector4 vector)
        {
            return new Vector4(
                matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W,
                matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W,
                matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W,
                matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W);
        }
        public static Vector3 NormalizeByW(this Vector4 vector)
        {
            return new Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
        }
        public static Vector4 ToNormalVector(this Vector4 vector)
        {
            var L = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return new Vector4(vector.X / L, vector.Y / L, vector.Z / L, 0);
        }
        public static Vector3 To3Dim(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
        public static Vector4 fromSphericalCoordinates(float R, float t, float f)
        {
            float X = R * (float)(Math.Sin(t) * Math.Cos(f));
            float Y = R * (float)(Math.Sin(t) * Math.Sin(f));
            float Z = R * (float)Math.Cos(t);
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
