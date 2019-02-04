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
        Model model;
        public Vector3 position { get => pos; }
        Vector3 pos;
        public float velocity { get; private set; } = 0;
        public float directionAngle { get; private set; } = -(float)Math.PI / 2;
        public ObjectParameters(Model _model, Vector3 _position)
        {
            model = _model;
            pos = _position;
        }
        public void Rotate(float angle)
        {
            directionAngle += angle;
            model.Translate(-pos.X, -pos.Y, 0);
            model.RotateZ(angle);
            model.Translate(pos.X, pos.Y, 0);
        }
        public void Move(float dx, float dy)
        {
            pos.X += dx;
            pos.Y += dy;
            model.Translate(dx, dy, 0);
        }
        public void MoveInDirection(float _directionAngle, float distance)
        {
            Move((float)Math.Cos(_directionAngle) * distance, (float)Math.Sin(_directionAngle) * distance);
        }
        public void ApplyVelocity(float _velocity, float _directionAngle)
        {
            velocity = _velocity;
            directionAngle = _directionAngle;
        }
        public void UpdatePosition(float acceleration)
        {
            if(velocity > 0)
            {
                acceleration = Math.Min(velocity, acceleration);
                Move((float)Math.Cos(directionAngle) * velocity, (float)Math.Sin(directionAngle) * velocity);
                velocity -= acceleration;
                if (velocity == 0) directionAngle = 0;
            }
        }
    }
}
