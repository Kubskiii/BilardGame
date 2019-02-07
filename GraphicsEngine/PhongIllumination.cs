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
        float ka, kd, ks;
        int nshiny;
        public PhongIllumination(float _ka = 0.5f, float _kd = 0.5f, float _ks = 0.5f, int _nshiny = 5)
        {
            ka = _ka;
            kd = _kd;
            ks = _ks;
            nshiny = _nshiny;
        }
        public float getIntensivity(Vector3 N, List<Light> Lights, Vector3 V, Vector3 point)
        {
            float intensivity = ka;
            foreach (var light in Lights)
            {
                var L = Vector3.Normalize(light.getValue(point));
                var Il = light.GetIntensivity(point);
                Vector3 R = Vector3.Normalize(2 * Math.Max(Vector3.Dot(N, L), 0) * N - L);
                intensivity += kd * Il * Vector3.Dot(N, L) + ks * Il * (float)Math.Pow(Math.Abs(Math.Max(Vector3.Dot(V, R), 0)), nshiny);
            }
            return intensivity;
        }
    }
}
