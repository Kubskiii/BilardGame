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
            //Vector3 f = Vector3.Normalize(-position + target);
            //Vector3 s = Vector3.Normalize(Vector3.Cross(f, UpVector));
            //Vector3 v = Vector3.Cross(s, f);
            //f = Vector3.Negate(f);

            //return new Matrix4x4(
            //    s.X, s.Y, s.Z, -Vector3.Dot(s, -position),
            //    v.X, v.Y, v.Z, -Vector3.Dot(v, -position),
            //    f.X, f.Y, f.Z, Vector3.Dot(f, -position),
            //    0, 0, 0, 1);

            Vector3 cZ = (position - target) / (position - target).Length();
            Vector3 cX = (Vector3.Cross(UpVector, cZ)) / (float)(Vector3.Cross(UpVector, cZ)).Length();
            Vector3 cY = (Vector3.Cross(cZ, cX)) / (float)(Vector3.Cross(cZ, cX)).Length();

            var viewMatrix = new Matrix4x4
            (

                cX.X, cY.X, cZ.X, position.X,
                cX.Y, cY.Y, cZ.Y, position.Y,
                cX.Z, cY.Z, cZ.Z, position.Z,

                0, 0, 0, 1
            );
            Matrix4x4.Invert(viewMatrix, out var invertedViewMatrix);
            return invertedViewMatrix;
        }
    }
}
