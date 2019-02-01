using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    class CPUEngine
    {
        float[,] Zbuffer;
        Resolution resolution { get; set; }
        Matrix4x4 projectionMatrix { get; }
        Matrix4x4 viewMatrix { get; }
        public CPUEngine(Resolution _resolution)
        {
            resolution = _resolution;
        }
        public UInt32[,] Render(IEnumerable<Model> models)
        {
            Zbuffer = new float[resolution.Width, resolution.Height];
            foreach(var model in models)
                foreach(var triangle in model)
                {
                    Vector3[] processedPoints = new Vector3[Triangle.count];
                    int processedCount = 0;
                    foreach(var edge in triangle.GetEdges())
                    {

                    }
                }
        }
    }
}
