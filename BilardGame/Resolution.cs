using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilardGame
{
    class Resolution
    {
        public int Width { get; }
        public int Height { get; }
        public float AspectRatio
        {
            get => (float)Height / Width;
        }
        public Resolution(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Width and Height must be positive numbers");
            Width = width;
            Height = height;
        }
    }
}
