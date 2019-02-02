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
        float ka = 0.3f;
        float ks = 0.3f;
        int nshiny = 5;
        public float getIntensivity(Vector3 N, Vector3 L, float kd, float Il, Vector3 V)
        {
            Vector3 R = Vector3.Normalize(2 * Vector3.Dot(L, N) * N - L);
            float intensivity = ka + kd * Il * Vector3.Dot(N, L) + ks * Il * (float)Math.Pow(Math.Abs(Vector3.Dot(R, V)), nshiny);
            return Math.Min(1, intensivity);
        }
    }
}
