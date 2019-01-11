using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BilardGame
{
    partial class Viewer
    {
        class PointFiller
        { 
            static readonly float precision = 0.0001f;
            uint[,] colors;
            Func<int, int, Color> getColor;
            float[,] zBuffer;
            PhongLight pl;
            public Vector3 plane { get; }
            Triangle T;
            public PointFiller(PhongLight _pl, Triangle _T, uint[,] _colors, Func<int, int, Color> _getColor, float[,] _zBuffer, params Point3D[] _points)
            {
                if (_points.Length != Triangle.pointsCount) throw new ArgumentException("Wrong number of points");
                colors = _colors;
                getColor = _getColor;
                zBuffer = _zBuffer;
                pl = _pl;
                T = _T;

                plane = Geometry.PlaneVector(_points[0], _points[1], _points[2]);
            }
            private float Interpolate(int x, int y)
            {
                return (1 - plane.X * x - plane.Y * y) / plane.Z;
            }
            public void Fill(int x, int y)
            {
                float Z = Interpolate(x, y);
                if (Z - zBuffer[x, y] > precision)
                {
                    zBuffer[x, y] = Z;
                    y = colors.GetLength(1) - y;
                    colors[x,y] = BitmapEx.ConvertColor(getColor(x,y).ChangeBrigthness(pl.GetBrightness(T.NormalVector, T.Middle)));
                }
            }
        }
    }
}
