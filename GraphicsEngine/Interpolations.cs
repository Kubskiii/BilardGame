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
        static float getArea(Vector3 a, Vector3 b, Vector3 c)
        {
            return Math.Abs((a.X - c.X) * (b.Y - a.Y) - (a.X - b.X) * (c.Y - a.Y)) / 2;
        }
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
            //float area = getArea(points[0], points[1], points[2]);
            //Vector3 p = new Vector3(x, y, 0);

            //return intensivities[0] * getArea(points[1], points[2], p) / area + intensivities[1] * getArea(points[0], points[2], p) / area + intensivities[2] * getArea(points[0], points[1], p) / area;
        }
    }
}
