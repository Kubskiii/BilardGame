using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine
{
    public class ReflectorLight : Light
    {
        Vector3 position;
        Vector3 D;
        int p = 10;
        public ReflectorLight(Vector3 _position, Vector3 target)
        {
            position = _position;
            D = Vector3.Normalize(target - _position);
        }
        public void Move(Vector3 newposition)
        {
            position = newposition;
        }
        public override Vector3 getValue(Vector3 point)
        {
            return Vector3.Normalize(position - point);
        }
        public override float GetIntensivity(Vector3 point)
        {
            return (float)Math.Pow(Vector3.Dot(Vector3.Normalize(D), Vector3.Normalize(position - point)), p);
        }
    }
}
