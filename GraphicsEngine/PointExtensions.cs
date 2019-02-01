using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    static class PointExtensions
    {
        public static Vector3 MultiplyByPointAndNormalize(this Matrix4x4 matrix, Vector4 vector)
        {
            float X = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Y + matrix.M14 * vector.Z;
            float Y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Y + matrix.M24 * vector.Z;
            float Z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Y + matrix.M34 * vector.Z;
            float W = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Y + matrix.M44 * vector.Z;

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
