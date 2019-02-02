using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    static class CameraBuilder
    {
        public static Matrix4x4 CreateLookAt(Vector3 position, Vector3 target, Vector3 UpVector)
        {
            #region old version
            //Vector3 zAxis = Vector3.Normalize(new Vector3(
            //    position.X - target.X,
            //    position.Y - target.Y,
            //    position.Z - target.Z));

            //Vector3 xAxis = Vector3.Normalize(new Vector3(
            //    UpVector.Y * zAxis.Z - UpVector.Z * zAxis.Y,
            //    UpVector.X * zAxis.Z + UpVector.Z * zAxis.X,
            //    UpVector.X * zAxis.Y - UpVector.Y * zAxis.X));

            //Vector3 yAxis = new Vector3(
            //    zAxis.Y * xAxis.Z - zAxis.Z * xAxis.Y,
            //    zAxis.X * xAxis.Z + zAxis.Z * xAxis.X,
            //    zAxis.X * xAxis.Y - zAxis.Y * xAxis.X);

            //Matrix4x4 invViewMatrix = new Matrix4x4(
            //    xAxis.X, yAxis.X, zAxis.X, position.X,
            //    xAxis.Y, yAxis.Y, zAxis.Y, position.Y,
            //    xAxis.Z, yAxis.Z, zAxis.Z, position.Z,
            //    0, 0, 0, 1);

            //Matrix4x4.Invert(invViewMatrix, out Matrix4x4 matrix1);
            #endregion

            Vector3 f = Vector3.Normalize(-position + target);
            Vector3 s = Vector3.Normalize(Vector3.Cross(f, UpVector));
            Vector3 v = Vector3.Cross(s, f);
            f = Vector3.Negate(f);

            return new Matrix4x4(
                s.X, s.Y, s.Z, -Vector3.Dot(s, -position),
                v.X, v.Y, v.Z, -Vector3.Dot(v, -position),
                f.X, f.Y, f.Z, Vector3.Dot(f, -position),
                0, 0, 0, 1);
        }
    }
}
