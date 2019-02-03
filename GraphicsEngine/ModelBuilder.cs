using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;

namespace GraphicsEngine
{
    public static class ModelBuilder
    {
        const float step = 0.05f;
        /// <summary>
        /// Create sphere model with given radius
        /// </summary>
        /// <remarks>center of sphere located in he begining of coordinate system</remarks>
        /// <param name="R">sphere radius</param>
        /// <param name="c">sphere color</param>
        /// <returns>sphere model</returns>
        public static Model CreateSphere(float R, Color c)
        {
            Model sphere = new Model();
            for (float t = 0; t < 1; t += step)
            {
                float theta1 = t * (float)Math.PI;
                float theta2 =(t + step) * (float)Math.PI;
                for (float f = 0; f < 1; f += step)
                {
                    float phi1 = f * 2 * (float)Math.PI;
                    float phi2 = (f + step) * 2 * (float)Math.PI;

                    //vertex1 = vertex on a sphere of radius r at spherical coords theta1, phi1
                    //vertex2 = vertex on a sphere of radius r at spherical coords theta1, phi2
                    //vertex3 = vertex on a sphere of radius r at spherical coords theta2, phi2
                    //vertex4 = vertex on a sphere of radius r at spherical coords theta2, phi1

                    if (t == 0) // top cap
                    {
                        var p1 = PointExtensions.fromSphericalCoordinates(R, theta1, phi1);
                        var p2 = PointExtensions.fromSphericalCoordinates(R, theta2, phi2);
                        var p3 = PointExtensions.fromSphericalCoordinates(R, theta2, phi1);
                        sphere.Add(new Triangle(new List<(Vector4, Vector4)>() {
                        (p1, p1.ToNormalVector()),
                        (p2, p2.ToNormalVector()),
                        (p3, p3.ToNormalVector()) })
                        {
                            color = c
                        });
                    }
                    else if (t + step >= 1) //end cap
                    {
                        var p1 = PointExtensions.fromSphericalCoordinates(R, theta2, phi2);
                        var p2 = PointExtensions.fromSphericalCoordinates(R, theta1, phi1);
                        var p3 = PointExtensions.fromSphericalCoordinates(R, theta1, phi2);
                        sphere.Add(new Triangle(new List<(Vector4, Vector4)>() {
                        (p1, p1.ToNormalVector()),
                        (p2, p2.ToNormalVector()),
                        (p3, p3.ToNormalVector()) })
                        {
                            color = c
                        });
                    }
                    else
                    {
                        var p1 = PointExtensions.fromSphericalCoordinates(R, theta1, phi1);
                        var p2 = PointExtensions.fromSphericalCoordinates(R, theta1, phi2);
                        var p3 = PointExtensions.fromSphericalCoordinates(R, theta2, phi1);
                        sphere.Add(new Triangle(new List<(Vector4, Vector4)>() {
                        (p1, p1.ToNormalVector()),
                        (p2, p2.ToNormalVector()),
                        (p3, p3.ToNormalVector()) })
                        {
                            color = c
                        });
                        p1 = PointExtensions.fromSphericalCoordinates(R, theta1, phi2);
                        p2 = PointExtensions.fromSphericalCoordinates(R, theta2, phi2);
                        p3 = PointExtensions.fromSphericalCoordinates(R, theta2, phi1);
                        sphere.Add(new Triangle(new List<(Vector4, Vector4)>() {
                        (p1, p1.ToNormalVector()),
                        (p2, p2.ToNormalVector()),
                        (p3, p3.ToNormalVector()) })
                        {
                            color = c
                        });
                    }
                }
            }
            return sphere;
        }
        public static Model CreateTube(float R, float H, Color c)
        {
            Model tube = new Model();
            for (float t = 0; t < 1; t += step)
            {
                var n = new Vector4(0, 0, -1, 0);
                tube.Add(new Triangle(new List<(Vector4, Vector4)>() {
                    (new Vector4(0, 0, 0, 1), n),
                    (PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0), n),
                    (PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0), n) })
                { color = c });

                var p1 = PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0);
                var p2 = PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0);
                var p3 = PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, H);
                var p4 = PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, H);

                var n1 = (new Vector4(p1.X, p1.Y, 0, 1)).ToNormalVector();
                var n2 = (new Vector4(p2.X, p2.Y, 0, 1)).ToNormalVector();
                var n3 = (new Vector4(p3.X, p3.Y, 0, 1)).ToNormalVector();
                var n4 = (new Vector4(p4.X, p4.Y, 0, 1)).ToNormalVector();

                tube.Add(new Triangle(new List<(Vector4, Vector4)>() {
                    (p1, n1),
                    (p2, n2),
                    (p3, n3)})
                { color = c });
                tube.Add(new Triangle(new List<(Vector4, Vector4)>() {
                    (p2, n2),
                    (p4, n4),
                    (p3, n3)})
                { color = c });

                n = new Vector4(0, 0, 1, 0);
                tube.Add(new Triangle(new List<(Vector4, Vector4)>() {
                    (new Vector4(0, 0, H, 1), n),
                    (PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, H), n),
                    (PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, H), n) })
                { color = c });
            }
            return tube;
        }
        //public static Model CreateCone(float R, float H, Color c)
        //{
        //    Model cone = new Model();
        //    for (float t = 0; t < 1; t += step)
        //    {
        //        cone.Add(new Triangle(new List<Vector4>() {
        //            new Vector4(0, 0, 0, 1),
        //            PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
        //            PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0) })
        //        { color = c });
        //        cone.Add(new Triangle(new List<Vector4>() {
        //            new Vector4(0, 0, H, 1),
        //            PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
        //            PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0) })
        //        { color = c });
        //    }
        //    return cone;
        //}
        public static Model CreateCuboid(float A, float B, float C, Color c)
        {
            var p1 = new Vector4(-A / 2, -B / 2, -C / 2, 1);
            var p2 = new Vector4(A / 2, -B / 2, -C / 2, 1);
            var p3 = new Vector4(-A / 2, B / 2, -C / 2, 1);
            var p4 = new Vector4(A / 2, B / 2, -C / 2, 1);

            var p5 = new Vector4(-A / 2, -B / 2, C / 2, 1);
            var p6 = new Vector4(A / 2, -B / 2, C / 2, 1);
            var p7 = new Vector4(-A / 2, B / 2, C / 2, 1);
            var p8 = new Vector4(A / 2, B / 2, C / 2, 1);

            var n1 = new Vector4(1, 0, 0, 0);
            var n2 = new Vector4(-1, 0, 0, 0);
            var n3 = new Vector4(0, 1, 0, 0);
            var n4 = new Vector4(0, -1, 0, 0);
            var n5 = new Vector4(0, 0, 1, 0);
            var n6 = new Vector4(0, 0, -1, 0);

            return new Model(new Triangle[]
            {
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n6), (p2, n6), (p3, n6)
                }) { color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n6), (p2, n6), (p3, n6)
                }){ color = c },

                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p5, n5), (p6, n5), (p7, n5)
                }){ color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p8, n5), (p6, n5), (p7, n5)
                }){ color = c },

                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n2), (p3, n2), (p5, n2)
                }){ color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p7, n2), (p3, n2), (p5, n2)
                }){ color = c },

                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p2, n1), (p4, n1), (p6, n1)
                }){ color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p8, n1), (p4, n1), (p6, n1)
                }){ color = c },

                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n4), (p2, n4), (p5, n4)
                }){ color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p6, n4), (p2, n4), (p5, n4)
                }){ color = c },

                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p3, n3), (p4, n3), (p7, n3)
                }){ color = c },
                new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p8, n3), (p4, n3), (p7, n3)
                }){ color = c },
            });
        }
    }
}
