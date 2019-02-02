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
                        sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi1),
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi2),
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi1) })
                        {
                            color = c
                        });
                    else if (t + step >= 1) //end cap
                        sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi2),
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi1),
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi2) })
                        {
                            color = c
                        });
                    else
                    {
                        sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi1),
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi2),
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi1) })
                        {
                            color = c
                        });
                        sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, theta1, phi2),
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi2),
                        PointExtensions.fromSphericalCoordinates(R, theta2, phi1) })
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
            for (float t = 0; t <= 1; t += step)
            {
                tube.Add(new Triangle(new List<Vector4>() {
                    new Vector4(0, 0, 0, 1),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0) })
                { color = c });
                tube.Add(new Triangle(new List<Vector4>() {
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, H) })
                { color = c });
                tube.Add(new Triangle(new List<Vector4>() {
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, H),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, H) })
                { color = c });
                tube.Add(new Triangle(new List<Vector4>() {
                    new Vector4(0, 0, H, 1),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, H),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, H) })
                { color = c });
            }
            return tube;
        }
        public static Model CreateCone(float R, float H, Color c)
        {
            Model cone = new Model();
            for (float t = 0; t < 1; t += step)
            {
                cone.Add(new Triangle(new List<Vector4>() {
                    new Vector4(0, 0, 0, 1),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0) })
                { color = c });
                cone.Add(new Triangle(new List<Vector4>() {
                    new Vector4(0, 0, H, 1),
                    PointExtensions.fromPolarCoordinates(R, t * 2 * (float)Math.PI, 0),
                    PointExtensions.fromPolarCoordinates(R, (t + step) * 2 * (float)Math.PI, 0) })
                { color = c });
            }
            return cone;
        }
        public static Model CreateCuboid(float A, float B, float C, Color c)
        {
            return new Model(
                new Triangle(new List<Vector4>() { new Vector4(-A / 2f, -B / 2f, -C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1), new Vector4(-A / 2f, B / 2f, -C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() { new Vector4(-A / 2f, B / 2f, C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1), new Vector4(-A / 2f, B / 2f, -C / 2f, 1) }) { color = c },

                new Triangle(new List<Vector4>() {new Vector4(-A / 2f, -B / 2f, -C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1), new Vector4(A / 2f, -B / 2f, -C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() {new Vector4(A / 2f, B / 2f, -C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1), new Vector4(A / 2f, -B / 2f, -C / 2f, 1) }) { color = c },

                new Triangle(new List<Vector4>() {new Vector4(-A / 2f, -B / 2f, -C / 2f, 1), new Vector4(A / 2f, -B / 2f, -C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() {new Vector4(A / 2f, -B / 2f, C / 2f, 1), new Vector4(A / 2f, -B / 2f, -C / 2f, 1), new Vector4(-A / 2f, -B / 2f, C / 2f, 1) }) { color = c },

                new Triangle(new List<Vector4>() {new Vector4(A / 2f, B / 2f, C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1), new Vector4(A / 2f, -B / 2f, C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() {new Vector4(A / 2f, -B / 2f, -C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1), new Vector4(A / 2f, -B / 2f, C / 2f, 1) }) { color = c },

                new Triangle(new List<Vector4>() {new Vector4(A / 2f, B / 2f, C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1), new Vector4(-A / 2f, B / 2f, C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() {new Vector4(-A / 2f, -B / 2f, C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1), new Vector4(-A / 2f, B / 2f, C / 2f, 1) }) { color = c },

                new Triangle(new List<Vector4>() {new Vector4(A / 2f, B / 2f, C / 2f, 1), new Vector4(-A / 2f, B / 2f, C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1) }) { color = c },
                new Triangle(new List<Vector4>() {new Vector4(-A / 2f, B / 2f, -C / 2f, 1), new Vector4(-A / 2f, B / 2f, C / 2f, 1), new Vector4(A / 2f, B / 2f, -C / 2f, 1) }) { color = c }
                );
        }
    }
}
