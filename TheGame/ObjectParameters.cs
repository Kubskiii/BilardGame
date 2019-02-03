using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TheGame
{
    class ObjectParameters
    {
        public int id;
        public Vector3 position;
        public ObjectParameters(int _id, Vector3 _position)
        {
            id = _id;
            position = _position;
        }
    }
}
