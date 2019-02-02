using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;

namespace GraphicsEngine
{
    public class Model : IEnumerable<Triangle>
    {
        List<Triangle> triangles;
        Matrix4x4 modelMatrix;
        public Vector4 Middle;
        public Matrix4x4 matrix { get => modelMatrix; }
        public Model(params Triangle[] _triangles)
        {
            triangles = new List<Triangle>(_triangles);
            foreach (var triangle in triangles)
            {
                var mid = triangle.Middle;
                Middle += new Vector4(mid.X, mid.Y, mid.Z, 1);
            }
            Middle /= triangles.Count;
            modelMatrix = Matrix4x4.Identity;
        }
        public void Add(Triangle t)
        {
            triangles.Add(t);
            var mid = t.Middle;
            int count = triangles.Count;
            Middle.X = (Middle.X * (count - 1) + mid.X) / count;
            Middle.Y = (Middle.Y * (count - 1) + mid.Y) / count;
            Middle.Z = (Middle.Z * (count - 1) + mid.Z) / count;
        }
        public void Transform(Matrix4x4 transformationMatrix)
        {
            modelMatrix *= transformationMatrix;
        }
        public IEnumerator<Triangle> GetEnumerator()
        {
            foreach (var t in triangles) yield return t;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var t in triangles) yield return t;
        }
    }
}
