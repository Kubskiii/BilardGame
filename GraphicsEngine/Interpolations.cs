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
        static bool IsBetween(int x, int y, Vector3 p1, Vector3 p2)
        {
            return x >= Math.Min(p1.X, p2.X) && x <= Math.Max(p1.X, p2.X) &&
                y >= Math.Min(p1.Y, p2.Y) && y <= Math.Max(p1.Y, p2.Y);
        }
        public static float Gouraud(float[] intensivities, Vector3[] points, int x, int y)
        {
            if(!IsBetween(x, y, points[0], points[1]))
            {
                float xb;
                float xa;
                float Ia = intensivities[2] - (intensivities[2] - intensivities[0]) * (points[2].Y - y) / (points[2].Y - points[0].Y);
                float Ib = intensivities[2] - (intensivities[2] - intensivities[1]) * (points[2].Y - y) / (points[2].Y - points[1].Y);

                return Ib - (Ib - Ia) * 1/2;
            }
            else if(!IsBetween(x, y, points[0], points[2]))
            {
                float xb;
                float xa;
                float Ia = intensivities[1] - (intensivities[1] - intensivities[0]) * (points[1].Y - y) / (points[1].Y - points[0].Y);
                float Ib = intensivities[1] - (intensivities[1] - intensivities[2]) * (points[1].Y - y) / (points[1].Y - points[2].Y);

                return Ib - (Ib - Ia) * 1 / 2;
            }
            else
            {
                float xb;
                float xa;
                float Ia = intensivities[0] - (intensivities[0] - intensivities[1]) * (points[0].Y - y) / (points[0].Y - points[1].Y);
                float Ib = intensivities[0] - (intensivities[0] - intensivities[2]) * (points[0].Y - y) / (points[0].Y - points[2].Y);

                return Ib - (Ib - Ia) * 1 / 2;
            }
            return 1;
        }
    }
}
