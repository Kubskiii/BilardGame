using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GraphicsEngine;
using System.Numerics;

namespace TheGame
{
    public class Game
    {
        List<ObjectParameters> balls = new List<ObjectParameters>();
        ObjectParameters whiteBall;
        ObjectParameters stick;
        ObjectParameters table;
        List<Model> models = new List<Model>();
        CPUEngine engine;
        bool rotateRigth = false;
        bool rotateLeft = false;
        bool Hold = false;
        bool duringMove = false;
        float power = 0;
        float ballV;
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 90;
            engine.ChangeCameraPosition(new Vector3(-10, 0, 10), new Vector3(0, 0, 0));
            engine.SwitchToGouraudShading();
            engine.AddLight(new PointLight(new Vector3(0, 0, 20)));


            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            BallTriangle(0, 5);
        }
        void AddBall(Color color)
        {
            var ball = ModelBuilder.CreateSphere(GameParameters.ballR, color);
            var position = new Vector3(0, 0, GameParameters.ballR);
            ball.Translate(position.X, position.Y, position.Z);
            models.Add(ball);
            balls.Add(new ObjectParameters(ball, position));
        }
        void BallTriangle(float x, float y)
        {
            float yStep = (float)Math.Sqrt(3) * GameParameters.ballR;
            AddBall(GameParameters.ballColors[0]);
            MoveBall(balls[balls.Count - 1], x, y);

            y += yStep;
            AddBall(GameParameters.ballColors[1]);
            MoveBall(balls[balls.Count - 1], x - GameParameters.ballR, y);
            AddBall(GameParameters.ballColors[2]);
            MoveBall(balls[balls.Count - 1], x + GameParameters.ballR, y);

            y += yStep;
            AddBall(GameParameters.ballColors[3]);
            MoveBall(balls[balls.Count - 1], x, y);
            AddBall(GameParameters.ballColors[4]);
            MoveBall(balls[balls.Count - 1], x - 2 * GameParameters.ballR, y);
            AddBall(GameParameters.ballColors[5]);
            MoveBall(balls[balls.Count - 1], x + 2 * GameParameters.ballR, y);
        }
        void AddTable(Color c)
        {
            var N = 50;
            var M = 100;
            var model = new Model();
            var n = new Vector4(0, 0, 1, 0);
            for (int i = 0; i < N; i++)
                for(int j = 0; j < M; j++)
                {
                    float x1 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)i / N);
                    float x2 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)(i + 1) / N);
                    float y1 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)j / M);
                    float y2 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)(j + 1) / M);
                    var p1 = new Vector4(x1, y1, 0, 1);
                    var p2 = new Vector4(x2, y1, 0, 1);
                    var p3 = new Vector4(x1, y2, 0, 1);
                    var p4 = new Vector4(x2, y2, 0, 1);
                    model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                    {
                        (p1, n) , (p2, n), (p3,n)
                    })
                    { color = c });
                    model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                    {
                        (p2, n), (p4, n), (p3, n)
                    })
                    { color = c });
                }
            models.Add(model);
            table = new ObjectParameters(model, new Vector3(0, 0, 0));
        }
        void AddStick(Color c)
        {
            var model = ModelBuilder.CreateTube(GameParameters.stickR, GameParameters.stickL, c);
            model.RotateX((float)Math.PI / 2);
            model.Translate(0, -GameParameters.ballR, GameParameters.ballR);
            models.Add(model);
            stick = new ObjectParameters(model, new Vector3(0, 0, GameParameters.ballR));
        }
        void AddWhiteBall()
        {
            var ball = ModelBuilder.CreateSphere(GameParameters.ballR, Colors.White);
            ball.Translate(0, 0, GameParameters.ballR);
            models.Add(ball);
            whiteBall = new ObjectParameters(ball, new Vector3(0, 0, GameParameters.ballR));
        }
        void MoveBall(ObjectParameters ball, float x, float y)
        {
            if(ball.position.X + x > GameParameters.tableWidth / 2 - GameParameters.ballR)
            {
                var dir = Vector2.Normalize(new Vector2(x, y));
                var N = new Vector2(0, 1);
                var cos = Vector2.Dot(dir, N);
                x *= -1;
                ball.directionAngle += 2 * (float)Math.Acos(cos);
            }

            if(ball.position.X + x < -GameParameters.tableWidth / 2 + GameParameters.ballR)
            {
                var dir = Vector2.Normalize(new Vector2(x, y));
                var N = new Vector2(0, -1);
                var cos = Vector2.Dot(dir, N);
                x *= -1;
                ball.directionAngle += 2 * (float)Math.Acos(cos);
            }

            if(ball.position.Y + y > GameParameters.tableDepth / 2 - GameParameters.ballR)
            {
                var dir = Vector2.Normalize(new Vector2(x, y));
                var N = new Vector2(-1, 0);
                var cos = Vector2.Dot(dir, N);
                y *= -1;
                ball.directionAngle += 2 * (float)Math.Acos(cos);
            }

            if (ball.position.Y + y < -GameParameters.tableDepth / 2 + GameParameters.ballR)
            {
                var dir = Vector2.Normalize(new Vector2(x, y));
                var N = new Vector2(1, 0);
                var cos = Vector2.Dot(dir, N);
                y *= -1;
                ball.directionAngle += 2 * (float)Math.Acos(cos);
            }
            ball.position.X += x;
            ball.position.Y += y;
            ball.model.Translate(x, y, 0);
        }
        void RotateStick(float angle)
        {
            var x = stick.position.X;
            var y = stick.position.Y;
            stick.model.Translate(-x, -y, 0);
            stick.model.RotateZ(angle);
            stick.model.Translate(x, y, 0);
            stick.directionAngle += angle;
        }
        public void RotateStickLeft() => rotateLeft = true;
        public void RotateStickRigth() => rotateRigth = true;
        public void HoldOnStick() => Hold = true;
        public void HoldOffStick() => Hold = false;
        public void Update()
        {
            if (!duringMove)
            {
                if (Hold)
                {
                    power += GameParameters.powerStep;
                    power = Math.Min(GameParameters.maxPower, power);
                    if (power != GameParameters.maxPower)
                    {
                        var x = (float)Math.Sin(stick.directionAngle) * GameParameters.powerStep;
                        var y = -(float)Math.Cos(stick.directionAngle) * GameParameters.powerStep;
                        stick.model.Translate(x, y, 0);
                    }
                    whiteBall.velocity = power * GameParameters.v;
                }
                else if(power > 0)
                {
                    var release = Math.Min(GameParameters.releaseSpeed, power);
                    var x = -(float)Math.Sin(stick.directionAngle) * release;
                    var y = (float)Math.Cos(stick.directionAngle) * release;
                    if (power > 0)
                        stick.model.Translate(x, y, 0);
                    power -= release;
                    if (power <= 0)
                    {
                        duringMove = true;
                        whiteBall.directionAngle = stick.directionAngle;
                    }
                }

                if (rotateLeft)
                {
                    RotateStick(-GameParameters.angle);
                    rotateLeft = false;
                }

                if (rotateRigth)
                {
                    RotateStick(GameParameters.angle);
                    rotateRigth = false;
                }
            }
            else if(whiteBall.velocity > 0)
            {
                var x = -(float)Math.Sin(whiteBall.directionAngle) * whiteBall.velocity;
                var y = (float)Math.Cos(whiteBall.directionAngle) * whiteBall.velocity;

                MoveBall(whiteBall, x, y);

                whiteBall.velocity += GameParameters.a;
            }
            else
            {
                ballV = 0;
                duringMove = false;
                var x = whiteBall.position.X - stick.position.X;
                var y = whiteBall.position.Y - stick.position.Y;

                stick.model.Translate(x, y, 0);
                stick.position.X += x;
                stick.position.Y += y;
            }
        }
        public uint[,] Display()
        {
            return engine.Render(models);
        }

    }
}
