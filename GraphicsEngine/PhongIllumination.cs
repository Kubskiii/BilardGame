using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    class PhongIllumination
    {
        float ka = 1;
        float ks = 1;
        int nshiny = 1;
        public float getIntensivity(Vector3 N, Vector3 L, float kd, float Il, Vector3 V)
        {
            Vector3 R = Vector3.Normalize(2 * Vector3.Dot(L, N) * N - L);
            float intensivity = ka + kd * Il * Vector3.Dot(N, L) + ks * Il * (float)Math.Pow(Vector3.Dot(R, V), nshiny);
            return intensivity / 3;
        }
    }
}
