using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;

namespace BilardGame
{
    class Triangle
    {
        public static readonly int pointsCount = 3;
        List<Point3D> points;
        public Color color = Colors.Black;
        public Vector3 NormalVector { get => Geometry.NormalVector(points[0], points[1], points[2]); }
        public Point3D Middle { get => new Point3D(points.Average(p => p.X), points.Average(p => p.Y), points.Average(p => p.Z)); }
        public Triangle(params Point3D[] _points)
        {
            if (_points.Count() != pointsCount) throw new ArgumentException("Wrong number of points");
            points = new List<Point3D>(_points);
            //NormalVector = Geometry.NormalVector(points[0], points[1], points[2]);
        }
        public IEnumerable<(Point3D startPoint, Point3D endPoint)> GetEdges()
        {
            Point3D prev = points.Last();
            foreach (Point3D p in points)
            {
                yield return (prev, p);
                prev = p;
            }
        }
        public IEnumerable<Point3D> GetPoints()
        {
            foreach (var point in points)
                yield return point;
        }
    }
}
