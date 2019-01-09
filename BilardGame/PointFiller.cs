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
            const float precision = 0.0001f;
            uint[,] colors;
            Func<int, int, Color> getColor;
            float[,] zBuffer;
            float A, B, C;
            public PointFiller(uint[,] _colors, Func<int, int, Color> _getColor, float[,] _zBuffer, params Point3D[] _points)
            {
                if (_points.Length != Triangle.pointsCount) throw new ArgumentException("Wrong number of points");
                colors = _colors;
                getColor = _getColor;
                zBuffer = _zBuffer;

                Matrix4x4 M = new Matrix4x4(
                    _points[0].X, _points[0].Y, _points[0].Z, 0,
                    _points[1].X, _points[1].Y, _points[1].Z, 0,
                    _points[2].X, _points[2].Y, _points[2].Z, 0,
                    0, 0, 0, 1);
                Matrix4x4 M1 = new Matrix4x4(
                    1, _points[0].Y, _points[0].Z, 0,
                    1, _points[1].Y, _points[1].Z, 0,
                    1, _points[2].Y, _points[2].Z, 0,
                    0, 0, 0, 1);
                Matrix4x4 M2 = new Matrix4x4(
                    _points[0].X, 1, _points[0].Z, 0,
                    _points[1].X, 1, _points[1].Z, 0,
                    _points[2].X, 1, _points[2].Z, 0,
                    0, 0, 0, 1);
                Matrix4x4 M3 = new Matrix4x4(
                    _points[0].X, _points[0].Y, 1, 0,
                    _points[1].X, _points[1].Y, 1, 0,
                    _points[2].X, _points[2].Y, 1, 0,
                    0, 0, 0, 1);
                float det = M.GetDeterminant();
                A = M1.GetDeterminant() / det;
                B = M2.GetDeterminant() / det;
                C = M3.GetDeterminant() / det;
            }
            private float Interpolate(int x, int y)
            {
                return (1 - A * x - B * y) / C;
            }
            public void Fill(int x, int y)
            {
                float Z = Interpolate(x, y);
                if (Z - zBuffer[x, y] > precision)
                {
                    zBuffer[x, y] = Z;
                    y = colors.GetLength(1) - y;
                    colors[x,y] = BitmapEx.ConvertColor(getColor(x,y));
                }
            }
        }
    }
}
