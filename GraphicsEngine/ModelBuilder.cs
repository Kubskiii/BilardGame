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
        const float step = 0.025f;
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
            for (float t = 0; t <= 1; t += step)
                for (float f = 0; f <= 1; f += step)
                {
                    sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, t * (float)Math.PI, f * 2 * (float)Math.PI),
                        PointExtensions.fromSphericalCoordinates(R, (t + step) * (float)Math.PI, f * 2 * (float)Math.PI),
                        PointExtensions.fromSphericalCoordinates(R, t * (float)Math.PI, (f + step) * 2 * (float)Math.PI) })
                    {
                        color = c
                    });
                    sphere.Add(new Triangle(new List<Vector4>() {
                        PointExtensions.fromSphericalCoordinates(R, t * (float)Math.PI, f * 2 * (float)Math.PI),
                        PointExtensions.fromSphericalCoordinates(R, (t - step) * (float)Math.PI, f * 2 * (float)Math.PI),
                        PointExtensions.fromSphericalCoordinates(R, t * (float)Math.PI, (f - step) * 2 * (float)Math.PI) })
                    {
                        color = c
                    });
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
