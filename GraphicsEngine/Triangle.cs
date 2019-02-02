using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;

namespace GraphicsEngine
{
    public class Triangle
    {
        public static readonly int count = 3;
        List<Vector4> points;
        public Color color = Colors.Black;
        public Vector4 NormalVector;
        public Vector4 Middle
        {
            get => new Vector4(points.Average(p => p.X), points.Average(p => p.Y), points.Average(p => p.Z), 1);
        }
        public Triangle(List<Vector4> _points)
        {
            if (_points.Count() != count) throw new ArgumentException("Wrong number of points");
            points = _points;
            Vector3 N = Vector3.Normalize(Vector3.Cross(
                new Vector3(points[1].X - points[0].X, points[1].Y - points[0].Y, points[1].Z - points[0].Z),
                new Vector3(points[2].X - points[0].X, points[2].Y - points[0].Y, points[2].Z - points[0].Z)));
            NormalVector = new Vector4(N.X, N.Y, N.Z, 1);
        }
        public IEnumerable<(Vector4 startPoint, Vector4 endPoint)> GetEdges()
        {
            Vector4 prev = points.Last();
            foreach (Vector4 p in points)
            {
                yield return (prev, p);
                prev = p;
            }
        }
        public IEnumerable<Vector4> GetPoints()
        {
            foreach (var point in points)
                yield return point;
        }
    }
}
