using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine
{
    public class ConstLight : Light
    {
        Vector3 value;
        public ConstLight(Vector3 _value)
        {
            value = Vector3.Normalize(_value);
        }
        public override Vector3 getValue(Vector3 point)
        {
            return value;
        }
    }
}
