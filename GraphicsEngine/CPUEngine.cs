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
        List<Vector3> Lights = new List<Vector3>() { Vector3.Normalize(new Vector3(0, 0, 1)) };
        Vector3 V = Vector3.Normalize(new Vector3(3, 3, 3));
        public CPUEngine(Resolution _resolution)
        {
            resolution = _resolution;
            viewMatrix = CameraBuilder.CreateLookAt(new Vector3(5, 5, 5), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            projectionMatrix = ProjectionBuilder.CreatePerspectiveOfView((float)Math.PI / 3, resolution.AspectRatio);
        }
        public void ChangeCameraPosition(Vector3 position, Vector3 target)
        {
            viewMatrix = CameraBuilder.CreateLookAt(position, target, new Vector3(0, 0, 1));
            cameraPos = position;
            V = position - target;
        }
        (Vector3 barycentricPoint, Vector3 normalVector, Vector3 point) VertexShader(Matrix4x4 modelMatrix, Vector4 point, Vector4 normalVector)
        {
            var _normalVector = modelMatrix.Multiply(normalVector).To3Dim();
            var _point = modelMatrix.Multiply(point);
            return ((projectionMatrix * viewMatrix).Multiply(_point).NormalizeByW(), _normalVector, _point.To3Dim());
        }
        Vector4 getPlaneVector(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var planeN = Vector3.Cross(p2 - p1, p3 - p1);
            return new Vector4(planeN.X, planeN.Y, planeN.Z, -(planeN.X * p1.X + planeN.Y * p1.Y + planeN.Z * p1.Z));
        }
        static bool isNormal(Vector3 point)
        {
            return Math.Abs(point.X) <= 1 &&
                Math.Abs(point.Y) <= 1 &&
                Math.Abs(point.Z) <= 1;
        }
        public uint[,] Render(IEnumerable<Model> models)
        {
            uint[,] colors = new uint[resolution.Width, resolution.Height];
            Zbuffer = new float[resolution.Width, resolution.Height];
            foreach (var model in models)
            {
                foreach (var triangle in model)
                {
                    var normalToMiddle = Vector3.Normalize(model.matrix.Multiply(triangle.NormalVector).To3Dim());
                    //var V = Vector3.Normalize(model.matrix.Multiply(triangle.Middle).To3Dim() - cameraPos);
                    //if (Vector3.Dot(normalToMiddle, V) < 0) continue;
                    var barycentricPoints = new List<Vector3>();
                    var normalVectors = new List<Vector3>();
                    var points = new List<Vector3>();
                    foreach(var vertex in triangle.GetPointsAndNormalVectors())
                    {
                        var parameters = VertexShader(model.matrix, vertex.point, vertex.vector);
                        if (isNormal(parameters.barycentricPoint))
                        {
                            barycentricPoints.Add(resolution.ToScreen(parameters.barycentricPoint));
                            normalVectors.Add(parameters.normalVector);
                            points.Add(parameters.point);
                        }
                        else break;
                    }
                    if(barycentricPoints.Count == Triangle.count)
                    {
                        var plane = getPlaneVector(barycentricPoints[0], barycentricPoints[1], barycentricPoints[2]);
                        float[] intensivities = new float[Triangle.count];
                        for (int i = 0; i < Triangle.count; i++)
                        {
                            //Lights.Add(Vector3.Normalize(new Vector3(5-points[i].X, 5-points[i].Y, 5-points[i].Z)));
                            intensivities[i] = phong.getIntensivity(normalVectors[i], Lights, 1,
                                Vector3.Normalize(new Vector3(cameraPos.X - points[i].X, cameraPos.Y - points[i].Y, cameraPos.Z - points[i].Z)));
                            //Lights.RemoveAt(0);
                        }
                        var intensivityVector = Interpolations.getIntensivityGouraudVector(intensivities, barycentricPoints.ToArray());
                        filler.Draw(barycentricPoints, (x, y) =>
                        {
                            float Z = 1 - (float)Math.Log10((-plane.W - plane.X * x - plane.Y * y) / plane.Z);
                            if (Z > Zbuffer[x, y])
                            {
                                Zbuffer[x, y] = Z;
                                float intensivity = Math.Min(1, Interpolations.Gouraud(intensivityVector, x, y));
                                Color c = triangle.color;
                                c.R = (byte)(c.R * intensivity);
                                c.G = (byte)(c.G * intensivity);
                                c.B = (byte)(c.B * intensivity);
                                colors[x, y] = BitmapExtensions.ConvertColor(c);
                            }
                        });
                    }
                }
            }
            return colors;
        }
    }
}
