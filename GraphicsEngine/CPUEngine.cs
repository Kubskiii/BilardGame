using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;

namespace GraphicsEngine
{
    public class CPUEngine
    {
        float[,] Zbuffer;
        Filler filler;
        PhongIllumination phong = new PhongIllumination();
        Resolution res;    
        Resolution resolution
        {
            get => res;
            set
            {
                res = value;
                filler = new Filler(value.Height);
            }
        }
        Matrix4x4 projectionMatrix;
        Matrix4x4 viewMatrix;
        Vector3 cameraPos = Vector3.Normalize(new Vector3(3, 3, 3));
        Vector3 L = new Vector3(100, 100, 100);
        Vector3 V = Vector3.Normalize(new Vector3(-3, -3, -3));
        public CPUEngine(Resolution _resolution)
        {
            resolution = _resolution;
            viewMatrix = CameraBuilder.CreateLookAt(new Vector3(3, 3, 3), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            projectionMatrix = ProjectionBuilder.CreatePerspectiveOfView((float)Math.PI / 2, resolution.AspectRatio);
        }
        public void ChangeCameraPosition(Vector3 position, Vector3 target)
        {
            viewMatrix = CameraBuilder.CreateLookAt(position, target, new Vector3(0, 0, 1));
            cameraPos = position;
            V = target - position;
        }
        Vector3 VertexShader(Matrix4x4 modelMatrix, Vector4 point)
        {
            Matrix4x4 M = projectionMatrix * viewMatrix * modelMatrix;
            return M.MultiplyByPointAndNormalize(point);
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
            //for (int i = 0; i < resolution.Width; i++)
            //    for (int j = 0; j < resolution.Height; j++)
            //        Zbuffer[i,j] = float.MaxValue;
            foreach (var model in models)
            {
                var ModelMiddle = model.matrix.MultiplyByPointAndNormalize(model.Middle);
                foreach (var triangle in model)
                {
                    var TriangleNormalVector = Vector3.Normalize(model.matrix.MultiplyByPointAndNormalize(triangle.NormalVector));
                    if (Vector3.Dot(TriangleNormalVector, V) > 0) continue;
                    Vector3[] processedPoints = new Vector3[Triangle.count];
                    int processedCount = 0;
                    foreach (var edge in triangle.GetEdges())
                    {
                        var start = VertexShader(model.matrix, edge.startPoint);
                        var end = VertexShader(model.matrix, edge.endPoint);
                        if (Math.Abs(start.X) <= 1 && Math.Abs(start.Y) <= 1 && Math.Abs(end.X) <= 1 && Math.Abs(end.Y) <= 1)
                            processedPoints[processedCount++] = resolution.ToScreen(start);
                    }
                    if (processedCount == Triangle.count)
                    {
                        Vector3 plane = getPlaneVector(processedPoints[0], processedPoints[1], processedPoints[2]);
                        filler.Draw(processedPoints, (x, y) =>
                        {
                            float Z = 1 - (1 - plane.X * x - plane.Y * y) / plane.Z;
                            //var mid4 = triangle.Middle;
                            //var mid3 = Vector3.Normalize(new Vector3(mid4.X, mid4.Y, mid4.Z));
                            //var V = Vector3.Normalize(cameraPos - mid3);
                            //if (Z - Zbuffer[x, y] >= 0.0001f)
                            //{
                            //    Zbuffer[x, y] = Z;
                                Color c = triangle.color;
                                //float intensivity = phong.getIntensivity(TriangleNormalVector, Vector3.Normalize(L), 1, 1);
                                //c.R = (byte)(c.R * intensivity);
                                //c.G = (byte)(c.G * intensivity);
                                //c.B = (byte)(c.B * intensivity);
                                colors[x, y] = BitmapExtensions.ConvertColor(c);
                            //}
                        });
                    }
                }
            }
            return colors;
        }
    }
}
