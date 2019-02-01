using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;

namespace BilardGame
{
    class PhongLight
    {
        IEnumerable<Point3D> lights;
        readonly float ks = 0.5f;
        readonly float ka = 0.7f;
        readonly float kd = 0.6f;
        readonly float alpha = 5;
        Point3D cameraPos;
        public PhongLight(IEnumerable<Point3D> _lights, Point3D _cameraPos)
        {
            lights = _lights;
            cameraPos = _cameraPos;
        }
        public float GetBrightness(Vector3 N, Point3D p)
        {
            float Ip = ka;

            Vector3 V = Vector3.Normalize((Vector3)cameraPos - (Vector3)p);

            foreach(var l in lights)
            {
                Vector3 Lm = Vector3.Normalize((Vector3)p - (Vector3)l);
                float dot = Vector3.Dot(Lm, N);
                Vector3 Rm = Vector3.Normalize(2 * dot * N - Lm);
                Ip += kd * dot + ks * (float)Math.Pow(Vector3.Dot(Rm, V), alpha);
            }

            return Math.Max(Math.Min(Ip, 1), 0);
        }
    }
}
