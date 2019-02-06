using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine
{
    public abstract class Light
    {
        public virtual float GetIntensivity(Vector3 point) => 1;
        public abstract Vector3 getValue(Vector3 point);
    }
}
