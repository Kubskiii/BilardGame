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
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 90;
            engine.ChangeCameraPosition(new Vector3(10, -1, 10), new Vector3(0, -1, 0));
            engine.SwitchToGouraudShading();
            engine.AddLight(new PointLight(new Vector3(0, 0, 20)));


            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            AddBall(Colors.Red, 0, 5);
            //AddBallTriangle(0, 5);
        }
        void AddBall(Color color, float x, float y)
        {
            var ball = ModelBuilder.CreateSphere(GameParameters.ballRadius, color);
            var position = new Vector3(x, y, GameParameters.ballRadius);
            ball.Translate(position.X, position.Y, position.Z);
            models.Add(ball);
            balls.Add(new ObjectParameters(ball, position));
        }
        void AddBallTriangle(float x, float y)
        {
            float yStep = (float)Math.Sqrt(3) * GameParameters.ballRadius;
            AddBall(GameParameters.ballColors[0], x, y);

            y += yStep;
            AddBall(GameParameters.ballColors[1], x - GameParameters.ballRadius, y);
            AddBall(GameParameters.ballColors[2], x + GameParameters.ballRadius, y);

            y += yStep;
            AddBall(GameParameters.ballColors[3], x, y);
            AddBall(GameParameters.ballColors[4], x - 2 * GameParameters.ballRadius, y);
            AddBall(GameParameters.ballColors[5], x + 2 * GameParameters.ballRadius, y);
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
            var model = ModelBuilder.CreateTube(GameParameters.stickRadius, GameParameters.stickLength, c);
            model.RotateX((float)Math.PI / 2);
            model.Translate(0, -GameParameters.ballRadius, GameParameters.ballRadius);
            models.Add(model);
            stick = new ObjectParameters(model, new Vector3(0, 0, GameParameters.ballRadius));
        }
        void AddWhiteBall()
        {
            var ball = ModelBuilder.CreateSphere(GameParameters.ballRadius, Colors.White);
            ball.Translate(0, 0, GameParameters.ballRadius);
            models.Add(ball);
            whiteBall = new ObjectParameters(ball, new Vector3(0, 0, GameParameters.ballRadius));
        }
        IEnumerable<ObjectParameters> AllOtherBalls(ObjectParameters ball)
        {
            var list = new List<ObjectParameters>();
            foreach (var b in balls)
                if (b != ball) list.Add(b);
            if (ball != whiteBall) list.Add(whiteBall);
            foreach (var b in list) yield return b;
        }
        void UpdateBallPosition(ObjectParameters ball)
        {
            if (ball.velocity == 0) return;
            var dist = ball.UpdatePosition(GameParameters.ballAcceleration);

            #region borders detection
            if (ball.position.X + GameParameters.ballRadius > GameParameters.tableWidth / 2)
            {
                var alpha = 1 - (GameParameters.tableWidth / 2 - ball.position.X - GameParameters.ballRadius + dist.x) / dist.x;
                ball.Move(-dist.x * alpha, -dist.y * alpha);
                var newAngle = (float)Math.PI - ball.directionAngle;
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, newAngle);
            }
            else if (ball.position.X - GameParameters.ballRadius < -GameParameters.tableWidth / 2)
            {
                var alpha = 1 - (-GameParameters.tableWidth / 2 - ball.position.X + GameParameters.ballRadius + dist.x) / dist.x;
                ball.Move(-dist.x * alpha, -dist.y * alpha);
                var newAngle = (float)Math.PI - ball.directionAngle;
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, newAngle);
            }
            if (ball.position.Y + GameParameters.ballRadius > GameParameters.tableDepth / 2)
            {
                var alpha = 1 - (GameParameters.tableDepth / 2 - ball.position.Y - GameParameters.ballRadius + dist.y) / dist.y;
                ball.Move(-dist.x * alpha, -dist.y * alpha);
                var newAngle = -ball.directionAngle;
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, newAngle);
            }
            else if (ball.position.Y - GameParameters.ballRadius < -GameParameters.tableDepth / 2)
            {
                var alpha = 1 - (-GameParameters.tableDepth / 2 - ball.position.Y * GameParameters.ballRadius + dist.y) / dist.y;
                ball.Move(-dist.x * alpha, -dist.y * alpha);
                var newAngle = -ball.directionAngle;
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, newAngle);
            }
            #endregion

            foreach (var b in AllOtherBalls(ball))
                if((b.position - ball.position).Length() < GameParameters.ballRadius * 2)
                {
                    ball.Move(-dist.x, -dist.y);
                    var D = b.position - ball.position;
                    var L = D.Length();
                    ball.Move(D.X - D.X / L * 2 * GameParameters.ballRadius, D.Y - D.Y / L * 2 * GameParameters.ballRadius);
                    ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, (float)Math.PI / 2 - ball.directionAngle);
                } 
        }
        public void RotateStickLeft() => rotateLeft = true;
        public void RotateStickRigth() => rotateRigth = true;
        public void HoldStick()
        {
            if (!Hold) power = 0;
            Hold = true;
        }
        public void ReleaseStick()
        {
            if (Hold && !duringMove)
            {
                Hold = false;
                duringMove = true;
                stick.ApplyVelocity(power, stick.directionAngle);
            }
        }
        public void Update()
        {
            if (!duringMove)
            {
                #region setting hit parameters
                if (Hold)
                {
                    if (power < GameParameters.maxPower)
                    {
                        power += GameParameters.powerStep;
                        if (power <= GameParameters.maxPower)
                            stick.MoveInDirection(stick.directionAngle, GameParameters.powerStep);
                    }
                }
                else
                {
                    if (rotateLeft)
                    {
                        stick.Rotate(-GameParameters.angleStep);
                        rotateLeft = false;
                    }
                    if (rotateRigth)
                    {
                        stick.Rotate(GameParameters.angleStep);
                        rotateRigth = false;
                    }
                }
                #endregion
            }
            else
            {
                if (stick.velocity > 0)
                {
                    var release = Math.Min(GameParameters.releaseSpeed, stick.velocity);
                    stick.MoveInDirection(stick.directionAngle, -release);
                    stick.ApplyVelocity(stick.velocity - release, stick.directionAngle);
                    if (stick.velocity <= 0) whiteBall.ApplyVelocity(GameParameters.ballVelocity * power, stick.directionAngle + (float)Math.PI);
                }
                else
                {
                    if (balls.Sum(b => b.velocity) + whiteBall.velocity > 0)
                    {
                        UpdateBallPosition(whiteBall);
                        foreach (var b in balls)
                            UpdateBallPosition(b);
                    }
                    else
                    {
                        duringMove = false;
                        stick.Move(whiteBall.position.X - stick.position.X, whiteBall.position.Y - stick.position.Y);
                    }
                }
            }
        }
        public uint[,] Display()
        {
            return engine.Render(models);
        }

    }
}
