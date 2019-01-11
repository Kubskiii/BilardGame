using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BilardGame
{
    class Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; } = 1;
        public Point3D(float _X, float _Y, float _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
        public static Point3D operator *(Matrix4x4 matrix, Point3D point)
        {
            Point3D result = new Point3D(0, 0, 0);
            result.X = point.X * matrix.M11 + point.Y * matrix.M12 + point.Z * matrix.M13 + point.W * matrix.M14;
            result.Y = point.X * matrix.M21 + point.Y * matrix.M22 + point.Z * matrix.M23 + point.W * matrix.M24;
            result.Z = point.X * matrix.M31 + point.Y * matrix.M32 + point.Z * matrix.M33 + point.W * matrix.M34;
            result.W = point.X * matrix.M41 + point.Y * matrix.M42 + point.Z * matrix.M43 + point.W * matrix.M44;
            return result;
        }
        public static Point3D fromSphericalCoordinates(float R, float t, float f)
        {
            return new Point3D(
                R * (float)(Math.Sin(t) * Math.Cos(f)),
                R * (float)(Math.Sin(t) * Math.Sin(f)),
                R * (float)Math.Cos(t));
        }
        public static Point3D fromPolarCoordinates(float R, float t, float z)
        {
            return new Point3D(
                R * (float)Math.Cos(t),
                R * (float)Math.Sin(t), z);
        }

        public void Normalize()
        {
            X /= W;
            Y /= W;
            Z /= W;
            W = 1;
        }
        public static explicit operator Vector3(Point3D point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }
}
