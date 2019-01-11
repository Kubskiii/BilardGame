using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BilardGame
{
    class GouraudShader
    {
        Vector3 Ia, Ib, Ic;
        Point3D a, b, c;
        Vector3 AB, AC, BC;
        public GouraudShader(Vector3[] I, Point3D[] p)
        {
            if (I.Length != Triangle.pointsCount)
                throw new ArgumentException("Wrong number of normal vectors");
            if (p.Length != Triangle.pointsCount)
                throw new ArgumentException("Wrong number of points");

            Ia = I[0]; Ib = I[1]; Ic = I[2];
            a = p[0]; b = p[1]; c = p[2];
            AB = new Vector3(a.Y - b.Y, a.X - b.X, (b.Y - a.Y) * b.X + (a.X - b.X) * b.Y);
            AC = new Vector3(a.Y - c.Y, a.X - c.X, (c.Y - a.Y) * c.X + (a.X - c.X) * c.Y);
            BC = new Vector3(b.Y - c.Y, b.X - c.X, (c.Y - b.Y) * c.X + (b.X - c.X) * c.Y);
        }
        static bool IsInLine(Vector3 v, float x, float y)
        {
            return v.X * x - v.Y * y + v.Z == 0;
        }
        static Point3D GetCrossPoint(Vector3 v, float x, float y)
        {
            return new Point3D((v.Y * y - v.Z) / v.X, y, 1);
        }
        Vector3 GetVector(float x, float y)
        {
            if (IsInLine(AB, x, y))
                return Ia * (b.Y - y) / (b.Y - a.Y) + Ib * (y - a.Y) / (b.Y - a.Y);
            else if(IsInLine(AC, x, y))
                return Ia * (c.Y - y) / (c.Y - a.Y) + Ic * (y - a.Y) / (c.Y - a.Y);
            else if(IsInLine(BC, x, y))
                return Ia * (c.Y - y) / (c.Y - b.Y) + Ic * (y - b.Y) / (c.Y - b.Y);
            else if( y >= Math.Max(a.Y, b.Y) || y <=Math.Min(a.Y, b.Y))
            {
                var e = GetCrossPoint(AC, x, y);
                var f = GetCrossPoint(BC, x, y);
                var Ie = GetVector(e.X, e.Y);
                var If = GetVector(e.X, e.Y);
                return Ie * (f.X - x) / (f.X - e.X) + If * (x - e.X) / (f.X - e.X);
            }
            else if (y >= Math.Max(a.Y, c.Y) || y <= Math.Min(a.Y, c.Y))
            {
                var e = GetCrossPoint(AB, x, y);
                var f = GetCrossPoint(BC, x, y);
                var Ie = GetVector(e.X, e.Y);
                var If = GetVector(e.X, e.Y);
                return Ie * (f.X - x) / (f.X - e.X) + If * (x - e.X) / (f.X - e.X);
            }
            else
            {
                var e = GetCrossPoint(AC, x, y);
                var f = GetCrossPoint(AC, x, y);
                var Ie = GetVector(e.X, e.Y);
                var If = GetVector(e.X, e.Y);
                return Ie * (f.X - x) / (f.X - e.X) + If * (x - e.X) / (f.X - e.X);
            }
        }
    }
}
