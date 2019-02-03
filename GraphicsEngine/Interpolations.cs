using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphicsEngine
{
    static class Interpolations
    {
        public static Vector4 getIntensivityGouraudVector(float[] intensivities, Vector3[] points)
        {
            Vector3 p1 = new Vector3(points[0].X, points[0].Y, intensivities[0]);
            Vector3 p2 = new Vector3(points[1].X, points[1].Y, intensivities[1]);
            Vector3 p3 = new Vector3(points[2].X, points[2].Y, intensivities[2]);
            var planeN = Vector3.Cross(p2 - p1, p3 - p1);
            return new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z));
        }
        public static float Gouraud(Vector4 intensivityVector, int x, int y)
        {
            return (-intensivityVector.W - intensivityVector.X * x - intensivityVector.Y * y) / intensivityVector.Z;
        }
        public static Vector4[] getNormalPhongEquations(Vector3[] normalVectors, Vector3[] points)
        {
            var result = new Vector4[3];

            Vector3 p1 = new Vector3(points[0].X, points[0].Y, normalVectors[0].X);
            Vector3 p2 = new Vector3(points[1].X, points[1].Y, normalVectors[1].X);
            Vector3 p3 = new Vector3(points[2].X, points[2].Y, normalVectors[2].X);
            var planeN = Vector3.Cross(p2 - p1, p3 - p1);
            result[0] = (new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z)));

            p1 = new Vector3(points[0].X, points[0].Y, normalVectors[0].Y);
            p2 = new Vector3(points[1].X, points[1].Y, normalVectors[1].Y);
            p3 = new Vector3(points[2].X, points[2].Y, normalVectors[2].Y);
            planeN = Vector3.Cross(p2 - p1, p3 - p1);
            result[1] = (new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z)));

            p1 = new Vector3(points[0].X, points[0].Y, normalVectors[0].Z);
            p2 = new Vector3(points[1].X, points[1].Y, normalVectors[1].Z);
            p3 = new Vector3(points[2].X, points[2].Y, normalVectors[2].Z);
            planeN = Vector3.Cross(p2 - p1, p3 - p1);
            result[2] = (new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z)));

            return result;
        }

        public static Vector3 Phong(Vector4[] vectors, int x, int y)
        {
            return new Vector3(
                (-vectors[0].W - vectors[0].X * x - vectors[0].Y * y) / vectors[0].Z,
                (-vectors[1].W - vectors[1].X * x - vectors[1].Y * y) / vectors[1].Z,
                (-vectors[2].W - vectors[2].X * x - vectors[2].Y * y) / vectors[2].Z);
        }
    }
}
