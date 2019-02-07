using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine
{
    public class PointLight : Light
    {
        Vector3 position;
        public PointLight(Vector3 _position)
        {
            position = _position;
        }

        public override Vector3 getValue(Vector3 point)
        {
            return Vector3.Normalize(position - point);
        }
    }
}
