using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace BilardGame
{
    partial class Viewer
    {
        public float FOV
        {
            set
            {
                e = 1 / (float)Math.Tan(value / 2);
                CalculateProjMatrix();
            }
        }
        private const float n = 1, f = 100;
        private float e = 1 / (float)Math.Tan(Math.PI / 6), a;
        private int Width, Height;
        Matrix4x4 Mview, Mproj;
        private Filling Filler;

        public Viewer(int _Width, int _Height)
        {
            Filler = new Filling(Height);
            Width = _Width; Height = _Height;
            a = (float)Height / Width;
            CalculateProjMatrix();
            CalculateViewMatrix();
        }
        public void Resize(int _Width, int _Height)
        {
            Width = _Width; Height = _Height;
            a = (float)Height / Width;
            CalculateProjMatrix();
        }
        private void CalculateViewMatrix()
        {
            Mview = new Matrix4x4(
                0, 1, 0, -0.5f,
                0, 0, 1, -0.5f,
                1, 0, 0, -3,
                0, 0, 0, 1);
        }
        private void CalculateProjMatrix()
        {
            Mproj = new Matrix4x4(
                e, 0, 0, 0,
                0, e / a, 0, 0,
                0, 0, -(f + n) / (f - n), -2 * f * n / (f - n),
                0, 0, -1, 0);
        }
        public static Point3D VertexShader(Matrix4x4 _Mmodel, Matrix4x4 _Mview, Matrix4x4 _Mproj, Point3D point)
        {
            return _Mproj * _Mview * _Mmodel * point;
        }
        private Point3D NormalizedToScreen(Point3D point)
        {
            return new Point3D(
                (int)(point.X * (Width / 2) + (Width / 2)),
                (int)(-point.Y * (Height / 2) + (Height / 2)),
                point.Z);
        }
        public void Draw(IEnumerable<Model> models, ref WriteableBitmap bmp)
        {
            float[,] zBuffer = new float[Width, Height];
            foreach (var model in models)
                foreach (var triangle in model)
                {
                    List<Point3D> points3d = new List<Point3D>(Triangle.pointsCount);
                    foreach (var edge in triangle.GetEdges())
                    {
                        Point3D p1 = VertexShader(model.matrix, Mview, Mproj, edge.startPoint);
                        Point3D p2 = VertexShader(model.matrix, Mview, Mproj, edge.endPoint);
                        p1.Normalize();
                        p2.Normalize();
                        if (Math.Abs(p1.X) <= 1 && Math.Abs(p1.Y) <= 1 && Math.Abs(p2.X) <= 1 && Math.Abs(p2.Y) <= 1)
                            points3d.Add(NormalizedToScreen(p1));
                    }
                    if (points3d.Count == Triangle.pointsCount)
                    {
                        PointFiller pf = new PointFiller(bmp, (x, y) => triangle.color, zBuffer, points3d.ToArray());
                        Filler.Draw(points3d, pf.Fill);
                    }
                }
        }
    }
}
