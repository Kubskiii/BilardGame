using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;

namespace TheGame
{
    static class GameParameters
    {
        static readonly public float ballR = 1;
        static readonly public float stickR = 0.5f;
        static readonly public float stickL = 40;
        static readonly public float tableWidth = 40;
        static readonly public float tableDepth = 80;
        static readonly public float angle = (float)Math.PI / 100;
        static readonly public float powerStep = 0.5f;
        static readonly public float maxPower = 10;
        static readonly public float releaseSpeed = 1;
        static readonly public float v = 0.2f;
        static readonly public float a = -0.01f;
        static public readonly Color[] ballColors = new Color[6]
            { Colors.Red, Colors.Yellow, Colors.Blue, Colors.Magenta, Colors.Orange, Colors.DarkGray };
    }
}
