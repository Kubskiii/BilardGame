using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;

namespace TheGame
{
    class GameParameters
    {
        readonly public float ballR = 1;
        readonly public float stickR = 0.5f;
        readonly public float stickL = 40;
        readonly public float tableWidth = 40;
        readonly public float tableDepth = 80;
        public readonly Color[] ballColors = new Color[6]
            { Colors.Red, Colors.Yellow, Colors.Blue, Colors.Magenta, Colors.Orange, Colors.DarkGray };
        public List<ObjectParameters> balls = new List<ObjectParameters>();
        public ObjectParameters whiteBall;
        public ObjectParameters stick;
        public ObjectParameters table;
    }
}
