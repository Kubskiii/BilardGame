using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    class CPUEngine
    {
        float[,] Zbuffer;
        Filler filler;
        Resolution resolution
        {
            get => resolution;
            set
            {
                resolution = value;
                filler = new Filler(value.Height);
            }
        }
        Matrix4x4 projectionMatrix { get; }
        Matrix4x4 viewMatrix { get; }
        public CPUEngine(Resolution _resolution)
        {
            resolution = _resolution;
        }
        Vector3 VertexShader(Matrix4x4 modelMatrix, Vector4 point)
        {
            return (projectionMatrix * viewMatrix * modelMatrix).MultiplyByPointAndNormalize(point);
        }
        Vector3 getPlaneVector(Vector3 p1, Vector3 p2, Vector3 p3)
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
        public uint[,] Render(IEnumerable<Model> models)
        {
            uint[,] colors = new uint[resolution.Width, resolution.Height];
            Zbuffer = new float[resolution.Width, resolution.Height];
            foreach(var model in models)
                foreach(var triangle in model)
                {
                    Vector3[] processedPoints = new Vector3[Triangle.count];
                    int processedCount = 0;
                    foreach(var edge in triangle.GetEdges())
                    {
                        var start = VertexShader(model.matrix, edge.startPoint);
                        var end = VertexShader(model.matrix, edge.endPoint);
                        if (Math.Abs(start.X) <= 1 && Math.Abs(start.Y) <= 1 && Math.Abs(end.X) <= 1 && Math.Abs(end.Y) <= 1)
                            processedPoints[processedCount++] = resolution.ToScreen(start);
                    }
                    if(processedCount == Triangle.count)
                    {
                        Vector3 plane = getPlaneVector(processedPoints[0], processedPoints[1], processedPoints[2]);
                        filler.Draw(processedPoints, (x, y) =>
                        {
                            float Z = (1 - plane.X * x - plane.Y * y) / plane.Z;
                            if(Z - Zbuffer[x, y] >= 0.0001f)
                            {
                                Zbuffer[x, y] = Z;
                                colors[x, y] = BitmapExtensions.ConvertColor(triangle.color);
                            }
                        });
                    }
                }
            return colors;
        }
    }
}
