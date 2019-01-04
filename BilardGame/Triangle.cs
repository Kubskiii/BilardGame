using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BilardGame
{
    class Triangle
    {
        public static readonly int pointsCount = 3;
        List<Point3D> points;
        public Triangle(params Point3D[] _points)
        {
            if (_points.Count() != pointsCount) throw new ArgumentException("Wrong number of points");
            points = new List<Point3D>(_points);
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
        public Color color = Colors.Black;
    }
}
