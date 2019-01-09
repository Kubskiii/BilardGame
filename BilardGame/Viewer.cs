using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
        const float n = 1, f = 100;
        float e = 1 / (float)Math.Tan(Math.PI / 8);
        Resolution res;
        Matrix4x4 Mproj;
        Camera camera;
        Filling Filler;

        public Viewer(Resolution _res)
        {
            res = _res;
            Filler = new Filling(res.Width);
            CalculateProjMatrix();
            camera = new Camera(new Vector3(6, 7, 8), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        }
        public void Resize(Resolution _res)
        {
            res = _res;
            CalculateProjMatrix();
        }
        void CalculateProjMatrix()
        {
            Mproj = new Matrix4x4(
                e, 0, 0, 0,
                0, e / res.AspectRatio, 0, 0,
                0, 0, -(f + n) / (f - n), -2 * f * n / (f - n),
                0, 0, -1, 0);
        }
        public static Point3D VertexShader(Matrix4x4 _Mmodel, Matrix4x4 _Mview, Matrix4x4 _Mproj, Point3D point)
        {
            return _Mproj * _Mview * _Mmodel * point;
        }
        Point3D NormalizedToScreen(Point3D point)
        {
            return new Point3D(
                (int)(point.X * (res.Width / 2) + (res.Width / 2)),
                (int)(point.Y * (res.Height / 2) + (res.Height / 2)),
                point.Z);
        }
        public void Draw(IEnumerable<Model> models, UInt32[,] colors)
        {
            float[,] zBuffer = new float[res.Width, res.Height];
            foreach (var model in models)
                foreach (var triangle in model)
                {
                    List<Point3D> points3d = new List<Point3D>(Triangle.pointsCount);
                    foreach (var edge in triangle.GetEdges())
                    {
                        Point3D p1 = VertexShader(model.matrix, camera.Matrix, Mproj, edge.startPoint);
                        Point3D p2 = VertexShader(model.matrix, camera.Matrix, Mproj, edge.endPoint);
                        p1.Normalize();
                        p2.Normalize();
                        if (Math.Abs(p1.X) <= 1 && Math.Abs(p1.Y) <= 1 && Math.Abs(p2.X) <= 1 && Math.Abs(p2.Y) <= 1)
                            points3d.Add(NormalizedToScreen(p1));
                    }
                    if (points3d.Count == Triangle.pointsCount)
                    {
                        PointFiller pf = new PointFiller(colors, (x, y) => triangle.color, zBuffer, points3d.ToArray());
                        Filler.Draw(points3d, pf.Fill);
                    }
                }
        }
    }
}
