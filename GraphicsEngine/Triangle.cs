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
        List<(Vector4 point, Vector4 NormalVector)> parameters;
        public Color color = Colors.Black;
        public Vector4 NormalVector;
        public Vector4 Middle;
        public Triangle(List<(Vector4 point, Vector4 NormalVector)> _parameters)
        {
            if (_parameters.Count != count) throw new ArgumentException("Wrong number of points");
            parameters = _parameters;
            NormalVector = new Vector4(parameters.Average(v => v.NormalVector.X), parameters.Average(v => v.NormalVector.Y), parameters.Average(v => v.NormalVector.Z), 1);
            Middle =new Vector4(parameters.Average(p => p.point.X), parameters.Average(p => p.point.Y), parameters.Average(p => p.point.Z), 1);
        }
        public IEnumerable<(Vector4 point, Vector4 vector)> GetPointsAndNormalVectors()
        {
            var prev = parameters.Last();
            foreach(var par in parameters)
            {
                yield return par;
                prev = par;
            }
        }
    }
}
