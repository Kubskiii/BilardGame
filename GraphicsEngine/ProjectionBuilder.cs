using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    public static class ProjectionBuilder
    {
        public static Matrix4x4 CreatePerspectiveOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance = 1, float farPlaneDistance = 100)
        {
            float e = 1 / (float)Math.Tan(fieldOfView / 2);
            return new Matrix4x4(
                e, 0, 0, 0,
                0, e / aspectRatio, 0, 0,
                0, 0, -(farPlaneDistance + nearPlaneDistance) / (farPlaneDistance - nearPlaneDistance), -2 * farPlaneDistance * nearPlaneDistance / (farPlaneDistance - nearPlaneDistance),
                0, 0, -1, 0);
        }
    }
}
