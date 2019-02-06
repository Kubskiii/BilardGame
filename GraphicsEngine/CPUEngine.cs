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
        enum Shading
        {
            Constant,
            Phong,
            Gouraud
        }
        float[,] Zbuffer;
        Filler filler;
        PhongIllumination phong = new PhongIllumination();
        Resolution res;
        Shading shading = Shading.Constant;
        Resolution resolution
        {
            get => res;
            set
            {
                res = value;
                //filler = new Filler(value);
                projectionMatrix = ProjectionBuilder.CreatePerspectiveOfView(fov, res.AspectRatio);
            }
        }
        float fov;
        public float FOV
        {
            set
            {
                fov = value * (float)Math.PI / 180;
                projectionMatrix = ProjectionBuilder.CreatePerspectiveOfView(fov, resolution.AspectRatio);
            }
        }
        Matrix4x4 projectionMatrix;
        Matrix4x4 viewMatrix;
        Vector3 cameraPos = new Vector3(50, 50, 50);
        List<Light> Lights = new List<Light>();
        public CPUEngine(Resolution _resolution)
        {
            resolution = _resolution;
            viewMatrix = CameraBuilder.CreateLookAt(cameraPos, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            projectionMatrix = ProjectionBuilder.CreatePerspectiveOfView((float)Math.PI / 3, resolution.AspectRatio);
        }
        public void ChangeCameraPosition(Vector3 position, Vector3 target)
        {
            viewMatrix = CameraBuilder.CreateLookAt(position, target, new Vector3(0, 0, 1));
            cameraPos = position;
        }

        public void ChangeCameraPosition2(Vector3 position, Vector3 target)
        {
            cameraPos = position;
            var CUp = new Vector3(0, 0, 1);

            Vector3 cZ = (position - target) / (position - target).Length();
            Vector3 cX = (Vector3.Cross(CUp, cZ)) / (float)(Vector3.Cross(CUp, cZ)).Length();
            Vector3 cY = (Vector3.Cross(cZ, cX)) / (float)(Vector3.Cross(cZ, cX)).Length();

            viewMatrix = new Matrix4x4
            (

                cX.X, cY.X, cZ.X, position.X,
                cX.Y, cY.Y, cZ.Y, position.Y,
                cX.Z, cY.Z, cZ.Z, position.Z,

                0, 0, 0, 1
            );
            Matrix4x4.Invert(viewMatrix, out var invertedViewMatrix);
            viewMatrix = invertedViewMatrix;
        }

        public void AddLight(Light L)
        {
            Lights.Add(L);
        }
        public void SwitchToConstantShading() => shading = Shading.Constant;
        public void SwitchToGouraudShading() => shading = Shading.Gouraud;
        public void SwitchToPhongShading() => shading = Shading.Phong;
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
            uint[,] colors = new uint[resolution.Width + 1, resolution.Height + 1];
            Zbuffer = new float[resolution.Width + 1, resolution.Height + 1];
            foreach (var model in models)
            {
                Parallel.ForEach(model, triangle =>
                //foreach (var triangle in model)
                {
                    var middle = model.matrix.Multiply(triangle.Middle).To3Dim();
                    var normalToMiddle = Vector3.Normalize(model.matrix.Multiply(triangle.NormalVector).To3Dim());
                    var view = Vector3.Normalize(middle - cameraPos);
                    if (Vector3.Dot(normalToMiddle, view) > 0) return;
                    var filler1 = new Filler(resolution);
                    var barycentricPoints = new List<Vector3>();
                    var normalVectors = new List<Vector3>();
                    var points = new List<Vector3>();
                    int count = 0;
                    foreach (var vertex in triangle.GetPointsAndNormalVectors())
                    {
                        var parameters = VertexShader(model.matrix, vertex.point, vertex.vector);
                        if (isNormal(parameters.barycentricPoint)) count++;
                        //if (isNormal(parameters.barycentricPoint))
                        //{
                        barycentricPoints.Add(resolution.ToScreen(parameters.barycentricPoint));
                        normalVectors.Add(parameters.normalVector);
                        points.Add(parameters.point);
                        //}
                        //else break;
                    }
                    //if (barycentricPoints.Count == Triangle.count)
                    if(count > 0)
                    {
                        var plane = getPlaneVector(barycentricPoints[0], barycentricPoints[1], barycentricPoints[2]);
                        switch (shading)
                        {
                            case Shading.Constant:
                                {
                                    var intensivity = phong.getIntensivity(normalToMiddle, Lights, view, middle);
                                    Color c = triangle.color;
                                    c.R = (byte)(Math.Min(Math.Max(c.R * intensivity, 0), 255));
                                    c.G = (byte)(Math.Min(Math.Max(c.G * intensivity, 0), 255));
                                    c.B = (byte)(Math.Min(Math.Max(c.B * intensivity, 0), 255));
                                    filler1.Draw(barycentricPoints, (x, y) =>
                                    {
                                        float Z = 1 - (float)Math.Log10((-plane.W - plane.X * x - plane.Y * y) / plane.Z);
                                        if (Z > Zbuffer[x, y])
                                        {
                                            Zbuffer[x, y] = Z;
                                            colors[x, y] = BitmapExtensions.ConvertColor(c);
                                        }
                                    });
                                    break;
                                }
                            case Shading.Phong:
                                {
                                    var pointsArray = barycentricPoints.ToArray();
                                    var pointsviews = new Vector3[Triangle.count];
                                    for (int i = 0; i < Triangle.count; i++)
                                    {
                                        pointsviews[i] = Vector3.Normalize(new Vector3(cameraPos.X - points[i].X, cameraPos.Y - points[i].Y, cameraPos.Z - points[i].Z));
                                    }

                                    var normals = Interpolations.getNormalPhongEquations(normalVectors.ToArray(), pointsArray);
                                    var views = Interpolations.getNormalPhongEquations(pointsviews, pointsArray);
                                    var positions = Interpolations.getNormalPhongEquations(points.ToArray(), pointsArray);
                                    filler1.Draw(barycentricPoints, (x, y) =>
                                    {
                                        float Z = 1 - (float)Math.Log10((-plane.W - plane.X * x - plane.Y * y) / plane.Z);
                                        if (Z > Zbuffer[x, y])
                                        {
                                            Zbuffer[x, y] = Z;

                                            var N = Interpolations.Phong(normals, x, y);
                                            var pview = Interpolations.Phong(views, x, y);
                                            var pposition = Interpolations.Phong(positions, x, y);
                                            var intensivity = phong.getIntensivity(N, Lights, pview, pposition);
                                            Color c = triangle.color;
                                            c.R = (byte)(Math.Min(Math.Max(c.R * intensivity, 0), 255));
                                            c.G = (byte)(Math.Min(Math.Max(c.G * intensivity, 0), 255));
                                            c.B = (byte)(Math.Min(Math.Max(c.B * intensivity, 0), 255));

                                            colors[x, y] = BitmapExtensions.ConvertColor(c);
                                        }
                                    });
                                    break;
                                }
                            case Shading.Gouraud:
                                {
                                    float[] intensivities = new float[Triangle.count];
                                    for (int i = 0; i < Triangle.count; i++)
                                    {
                                        intensivities[i] = phong.getIntensivity(normalVectors[i], Lights,
                                            Vector3.Normalize(new Vector3(cameraPos.X - points[i].X, cameraPos.Y - points[i].Y, cameraPos.Z - points[i].Z)),
                                            points[i]);
                                    }
                                    var intensivityVector = Interpolations.getIntensivityGouraudVector(intensivities, barycentricPoints.ToArray());
                                    filler1.Draw(barycentricPoints, (x, y) =>
                                    {
                                        float Z = 1 - (float)Math.Log10((-plane.W - plane.X * x - plane.Y * y) / plane.Z);
                                        if (Z > Zbuffer[x, y])
                                        {
                                            Zbuffer[x, y] = Z;
                                            float intensivity = Interpolations.Gouraud(intensivityVector, x, y);
                                            Color c = triangle.color;
                                            c.R = (byte)(Math.Min(Math.Max(c.R * intensivity, 0), 255));
                                            c.G = (byte)(Math.Min(Math.Max(c.G * intensivity, 0), 255));
                                            c.B = (byte)(Math.Min(Math.Max(c.B * intensivity, 0), 255));
                                            colors[x, y] = BitmapExtensions.ConvertColor(c);
                                        }
                                    });
                                    break;
                                }
                        }
                    }

                    #region drawing normal vectors
                    //var lolo = VertexShader(model.matrix, triangle.Middle, triangle.NormalVector);
                    //var lolo2 = VertexShader(model.matrix, triangle.Middle + triangle.NormalVector, triangle.NormalVector);
                    //var l = triangle.NormalVector.Length();
                    //if (l < 0.9) count++;
                    //if (isNormal(lolo.barycentricPoint) && isNormal(lolo2.barycentricPoint))
                    //{
                    //    var v1 = resolution.ToScreen(lolo.barycentricPoint);
                    //    var v2 = resolution.ToScreen(lolo2.barycentricPoint);
                    //    DrawLine(v1.X, v1.Y, v2.X, v2.Y, colors);
                    //}
                    #endregion
                });
            }
            return colors;
        }

        static void DrawLine(float x1, float y1, float x2, float y2, uint[,] colors)
        {
            const int size = 1;
            int dx = (int)Math.Abs(x1 - x2);
            int dy = (int)Math.Abs(y1 - y2);
            int xi = x2 > x1 ? size : -size;
            int yi = y2 > y1 ? size : -size;
            colors[(int)x1, (int)y1] = BitmapExtensions.ConvertColor(Colors.Green);
            if (dx > dy)
            {
                int d = 2 * dy - dx; //initial value of d
                int incrE = 2 * dy; //increment used for move to E
                int incrNE = 2 * (dy - dx); //increment used for move to NE
                while (x1 != x2)
                {
                    if (d < 0) //choose E
                    {
                        d += incrE;
                        x1 += xi;
                    }
                    else //choose NE
                    {
                        d += incrNE;
                        x1 += xi;
                        y1 += yi;
                    }
                    colors[(int)x1, (int)y1] = BitmapExtensions.ConvertColor(Colors.Green);
                }
            }
            else
            {
                int d = 2 * dx - dy;
                int incrE = 2 * dx;
                int incrNE = 2 * (dx - dy);
                while (y1 != y2)
                {
                    if (d < 0)
                    {
                        d += incrE;
                        y1 += yi;
                    }
                    else
                    {
                        d += incrNE;
                        x1 += xi;
                        y1 += yi;
                    }
                    colors[(int)x1, (int)y1] = BitmapExtensions.ConvertColor(Colors.Green);
                }
            }
        }
    }
}
