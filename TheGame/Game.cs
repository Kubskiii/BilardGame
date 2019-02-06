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
        bool staticCamera = false;
        float power = 0;
        Vector3 cameraDirection = new Vector3(0, -GameParameters.cameraDistance, GameParameters.cameraDistance);
        bool staticLightOn = false;
        bool trackingLightOn = false;
        PointLight staticLight = new PointLight(new Vector3(0, 0, GameParameters.lightHeight));
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 90;

            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            AddBallTriangle(0, 5);
            engine.SwitchToPhongShading();
            StaticCamera();
            //SwitchPointLight();
            SwitchTrackingLight();
            UpdateLights();
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
            stick.ApplyVelocity(0, (float)Math.PI / 2);
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
            foreach (var b in balls)
                if (b != ball) yield return b;
            if (ball != whiteBall) yield return whiteBall;
        }
        void UpdateBallPosition(ObjectParameters ball)
        {
            if (ball.velocity == 0) return;
            var move = ball.UpdatePosition(GameParameters.ballAcceleration);
            float distance = move.Length();

            #region borders detection
            if (ball.position.X + GameParameters.ballRadius > GameParameters.tableWidth / 2)
            {
                float alpha = 1 - (GameParameters.tableWidth / 2 - ball.position.X - GameParameters.ballRadius + move.X) / move.X;
                ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, (float)Math.PI - ball.directionAngle);
            }
            else if (ball.position.X - GameParameters.ballRadius < -GameParameters.tableWidth / 2)
            {
                float alpha = 1 - (-GameParameters.tableWidth / 2 - ball.position.X + GameParameters.ballRadius + move.X) / move.X;
                ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, (float)Math.PI - ball.directionAngle);
            }

            if (ball.position.Y + GameParameters.ballRadius > GameParameters.tableDepth / 2)
            {
                var alpha = 1 - (GameParameters.tableDepth / 2 - ball.position.Y - GameParameters.ballRadius + move.Y) / move.Y;
                ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, -ball.directionAngle);
            }
            else if (ball.position.Y - GameParameters.ballRadius < -GameParameters.tableDepth / 2)
            {
                var alpha = 1 - (-GameParameters.tableDepth / 2 - ball.position.Y + GameParameters.ballRadius + move.Y) / move.Y;
                ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, -ball.directionAngle);
            }
            #endregion

            foreach (var b in AllOtherBalls(ball))
                if ((b.position - ball.position).Length() < GameParameters.ballRadius * 2)
                {
                    var distanceBetweenBalls = b.position - ball.position;
                    var speedratio = distanceBetweenBalls.Y / distanceBetweenBalls.X;
                    var bAngle = (float)Math.Atan(distanceBetweenBalls.Y / distanceBetweenBalls.X);
                    var newAngle = -((float)Math.PI - 2 * bAngle + ball.directionAngle);
                    var speedRatio = 0.75f;// 1 - (float)Math.Abs(Math.Cos(bAngle));
                    var velocity = Math.Abs(ball.velocity + b.velocity);
                    ball.MoveInDirection(ball.directionAngle, (-distanceBetweenBalls).Length() - GameParameters.ballRadius * 2);
                    b.ApplyVelocity(velocity* speedRatio, bAngle);
                    ball.ApplyVelocity(Math.Max(velocity * (1 - speedRatio), 0), newAngle);
                }
        }
        Vector3 UpdateCameraDirection()
        {
            return new Vector3(-(float)Math.Cos(stick.directionAngle), -(float)Math.Sin(stick.directionAngle), 1) * GameParameters.cameraDistance;
        }
        void UpdateLights()
        {
            if(trackingLightOn)
            {
                engine.RemoveAllLights();
                if (staticLightOn) engine.AddLight(staticLight);
                engine.AddLight(new PointLight(new Vector3(whiteBall.position.X, whiteBall.position.Y, GameParameters.cameraDistance)));
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
        public void StaticCamera()
        {
            staticCamera = true;
            engine.ChangeCameraPosition(new Vector3(0.5f, 0, 1.5f) * GameParameters.cameraDistance, new Vector3(0, 0, 0));
        }
        public void ActiveCamera()
        {
            staticCamera = false;
            cameraDirection = UpdateCameraDirection();
            engine.ChangeCameraPosition(stick.position + cameraDirection, stick.position);
        }
        public void SwitchPointLight()
        {
            staticLightOn = !staticLightOn;
            engine.AddLight(staticLight);
        }
        public void SwitchTrackingLight()
        {
            trackingLightOn = !trackingLightOn;
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
                            stick.MoveInDirection(stick.directionAngle, -GameParameters.powerStep);
                    }
                }
                else
                {
                    if (rotateLeft)
                    {
                        stick.Rotate(-GameParameters.angleStep);
                        if (!staticCamera)
                        {
                            cameraDirection = UpdateCameraDirection();
                            engine.ChangeCameraPosition(stick.position + cameraDirection, stick.position);
                        }
                        rotateLeft = false;
                    }
                    if (rotateRigth)
                    {
                        stick.Rotate(GameParameters.angleStep);
                        if (!staticCamera)
                        {
                            cameraDirection = UpdateCameraDirection();
                            engine.ChangeCameraPosition(stick.position + cameraDirection, stick.position);
                        }
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
                    stick.MoveInDirection(stick.directionAngle, release);
                    stick.ApplyVelocity(stick.velocity - release, stick.directionAngle);
                    if (stick.velocity <= 0)
                    {
                        whiteBall.ApplyVelocity(GameParameters.ballVelocity * power, stick.directionAngle);
                    }
                }
                else
                {
                    if (models.Contains(stick.model)) models.Remove(stick.model);
                    if (balls.Sum(b => b.velocity) + whiteBall.velocity > 0)
                    {
                        if (whiteBall.velocity > 0 && !staticCamera)
                            engine.ChangeCameraPosition( whiteBall.position + cameraDirection, whiteBall.position);
                        UpdateBallPosition(whiteBall);
                        foreach (var b in balls)
                            UpdateBallPosition(b);
                        UpdateLights();
                    }
                    else
                    {
                        duringMove = false;
                        stick.Move(whiteBall.position.X - stick.position.X, whiteBall.position.Y - stick.position.Y);
                        models.Add(stick.model);
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
