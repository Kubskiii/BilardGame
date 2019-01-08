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
            Point3D[] points = new Point3D[Triangle.pointsCount];
            UInt32[,] colors;
            Func<int, int, Color> getColor;
            float[,] zBuffer;
            public PointFiller(UInt32[,] _colors, Func<int, int, Color> _getColor, float[,] _zBuffer, params Point3D[] _points)
            {
                if (_points.Length != Triangle.pointsCount) throw new ArgumentException("Wrong number of points");
                points = _points;
                colors = _colors;
                getColor = _getColor;
                zBuffer = _zBuffer;
            }
            private static float Interpolate(Point3D[] _points, int x, int y)
            {
                Matrix4x4 m1 = new Matrix4x4(
                    _points[0].X, _points[1].X, _points[2].X, 0,
                    _points[0].Y, _points[1].Y, _points[2].Y, 0,
                    1, 1, 1, 0,
                    0, 0, 0, 1);
                Matrix4x4 mA = new Matrix4x4(
                    x, _points[1].X, _points[2].X, 0,
                    y, _points[1].Y, _points[2].Y, 0,
                    1, 1, 1, 0,
                    0, 0, 0, 1);
                Matrix4x4 mB = new Matrix4x4(
                    _points[0].X, x, _points[2].X, 0,
                    _points[0].Y, y, _points[2].Y, 0,
                    1, 1, 1, 0,
                    0, 0, 0, 1);
                float M = m1.GetDeterminant();
                float a = mA.GetDeterminant() / M, b = mB.GetDeterminant() / M;
                float g = 1 - a - b;
                return a * _points[0].Z + b * _points[1].Z + g * _points[2].Z;
            }
            public void Fill(int x, int y)
            {
                float Z = Interpolate(points, x, y);
                if (Z - zBuffer[x, y] >= 0)
                {
                    zBuffer[x, y] = Z;
                    colors[x,y] = BitmapEx.ConvertColor(getColor(x,y));
                }
            }
        }
    }
}
