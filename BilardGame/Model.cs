using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Collections;

namespace BilardGame
{
    class Model : IEnumerable<Triangle>
    {
        List<Triangle> triangles;
        Matrix4x4 modelMatrix;
        public Matrix4x4 matrix { get => modelMatrix; }
        public Model(params Triangle[] _triangles)
        {
            triangles = new List<Triangle>(_triangles);
            modelMatrix = new Matrix4x4(
                1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }
        public void Add(Triangle t)
        {
            triangles.Add(t);
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
