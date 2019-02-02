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
        Vector3 L = Vector3.Normalize(new Vector3(100, 100, 100));
        Vector3 V = Vector3.Normalize(new Vector3(3, 3, 3));
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
            V = position - target;
        }
        Vector3 VertexShader(Matrix4x4 modelMatrix, Vector4 point)
        {
            Matrix4x4 M = projectionMatrix * viewMatrix * modelMatrix;
            return M.MultiplyByPointAndNormalize(point);
        }
        Vector4 getPlaneVector(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var planeN = Vector3.Cross(p2 - p1, p3 - p1);
            return new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z));
        }
        public uint[,] Render(IEnumerable<Model> models)
        {
            uint[,] colors = new uint[resolution.Width, resolution.Height];
            Zbuffer = new float[resolution.Width, resolution.Height];
            //Parallel.ForEach(models, model =>
            foreach (var model in models)
            {
                foreach (var triangle in model)
                {
                    var TriangleNormalVector = Vector3.Normalize(model.matrix.MultiplyByVector(triangle.NormalVector));
                    var VV = V;// Vector3.Normalize(-cameraPos + model.matrix.MultiplyByPointAndNormalize(triangle.Middle));
                    if (Vector3.Dot(TriangleNormalVector, VV) < 0) continue;
                    Vector3[] processedPoints = new Vector3[Triangle.count];
                    Vector3[] unprocessedPoints = new Vector3[Triangle.count];
                    Vector4[] normals = new Vector4[Triangle.count];
                    int processedCount = 0;
                    Vector3? start = null, end = null;
                    foreach (var edge in triangle.GetEdges())
                    {
                        if (!start.HasValue) start = VertexShader(model.matrix, edge.startPoint);
                        end = VertexShader(model.matrix, edge.endPoint);
                        if (Math.Abs(start.Value.X) <= 1 && Math.Abs(start.Value.Y) <= 1 && Math.Abs(end.Value.X) <= 1 && Math.Abs(end.Value.Y) <= 1)
                        {
                            processedPoints[processedCount] = resolution.ToScreen(end.Value);
                            unprocessedPoints[processedCount] = model.matrix.MultiplyByPointAndNormalize(edge.endPoint);
                            normals[processedCount++] = edge.endNormal;
                        }
                        start = end;
                    }
                    if (processedCount == Triangle.count)
                    {
                        var plane = getPlaneVector(processedPoints[0], processedPoints[1], processedPoints[2]);
                        //var mid4 = triangle.Middle;
                        //var v = Vector3.Normalize(new Vector3(cameraPos.X - mid4.X, cameraPos.Y - mid4.Y, cameraPos.Z - mid4.Z));
                        //Color c = triangle.color;
                        float[] intensivities = new float[Triangle.count];
                        for(int i = 0; i < Triangle.count; i++)
                        {
                            var p = unprocessedPoints[i];
                            intensivities[i] = phong.getIntensivity(model.matrix.MultiplyByVector(normals[i]), L, 0.5f, 1, Vector3.Normalize(new Vector3(cameraPos.X - p.X, cameraPos.Y - p.Y, cameraPos.Z - p.Z)));
                        }
                        //float intensivity = phong.getIntensivity(TriangleNormalVector, L, 0.5f, 1, v);
                        //c.R = (byte)(c.R * intensivity);
                        //c.G = (byte)(c.G * intensivity);
                        //c.B = (byte)(c.B * intensivity);
                        filler.Draw(processedPoints, (x, y) =>
                        {
                            float Z = 1 - (-plane.W - plane.X * x - plane.Y * y) / plane.Z;
                            if (Z - Zbuffer[x, y] >= 0.0001f)
                            {
                                Zbuffer[x, y] = Z;
                                float intensivity = Interpolations.Gouraud(intensivities, processedPoints, x, y);
                                Color c = triangle.color;
                                c.R = (byte)(c.R * intensivity);
                                c.G = (byte)(c.G * intensivity);
                                c.B = (byte)(c.B * intensivity);
                                colors[x, y] = BitmapExtensions.ConvertColor(c);
                            }
                        });
                    }
                }
            }//);
            return colors;
        }
    }
}
