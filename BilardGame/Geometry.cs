using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BilardGame
{
    static class Geometry
    {
        public static Vector3 PlaneVector(Point3D p1, Point3D p2, Point3D p3)
        {
            Matrix4x4 M = new Matrix4x4(
                p1.X, p1.Y, p1.Z, 0,
                p2.X, p2.Y, p2.Z, 0,
                p3.X, p3.Y, p3.Z, 0,
                0, 0, 0, 1);
            Matrix4x4 M1 = new Matrix4x4(
                1, p1.Y, p1.Z, 0,
                1, p2.Y, p2.Z, 0,
                1, p3.Y, p3.Z, 0,
                0, 0, 0, 1);
            Matrix4x4 M2 = new Matrix4x4(
                p1.X, 1, p1.Z, 0,
                p2.X, 1, p2.Z, 0,
                p3.X, 1, p3.Z, 0,
                0, 0, 0, 1);
            Matrix4x4 M3 = new Matrix4x4(
                p1.X, p1.Y, 1, 0,
                p2.X, p2.Y, 1, 0,
                p3.X, p3.Y, 1, 0,
                0, 0, 0, 1);
            float det = M.GetDeterminant();
            return new Vector3(M1.GetDeterminant() / det, M2.GetDeterminant() / det, M3.GetDeterminant() / det);
        }
        public static Vector3 NormalVector(Point3D p1, Point3D p2, Point3D p3)
        {
            return Vector3.Normalize(Vector3.Cross(
                new Vector3(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z), 
                new Vector3(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z)));
        }
    }
}
