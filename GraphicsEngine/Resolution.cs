using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    public class Resolution
    {
        public int Width { get; }
        public int Height { get; }
        public float AspectRatio
        {
            get => (float)Height / Width;
        }
        public Resolution(int width = 800, int height = 600)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Width and Height must be positive numbers");
            Width = width;
            Height = height;
        }
        public Vector3 ToScreen(Vector3 point)
        {
            //if (Math.Abs(point.X) > 1 || Math.Abs(point.Y) > 1)
            //    throw new ArgumentException("given point not in normal coordinates");
            point.X = (int)(point.X * (Width / 2) + (Width / 2));
            point.Y = (int)(point.Y * (-Height / 2) + (Height / 2));
            return point;
        }
    }
}
