using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GraphicsEngine;

namespace TheGame
{
    class ObjectParameters
    {
        public Model model;
        public Vector3 position;
        public ObjectParameters(Model _model, Vector3 _position)
        {
            model = _model;
            position = _position;
        }
        public float velocity = 0;
        public float directionAngle = 0;
    }
}
