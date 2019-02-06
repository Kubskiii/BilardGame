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
        float viewAngle;
        public ReflectorLight(Vector3 _position, float _viewAngle)
        {
            position = _position;
            viewAngle = _viewAngle * (float)Math.PI / 180;
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
            return Vector3.Dot(Vector3.Normalize(position), Vector3.Normalize(position - point));
        }
    }
}
