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
        List<Model> models = new List<Model>();
        CPUEngine engine;
        GameParameters gp = new GameParameters();
        bool rotateRigth = false;
        bool rotateLeft = false;
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 90;
            engine.ChangeCameraPosition(new Vector3(-20, 0, 80), new Vector3(0, 0, 0));
            engine.SwitchToGouraudShading();
            engine.AddLight(new PointLight(new Vector3(0, 0, 20)));


            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            BallTriangle(0, 5);
        }
        void AddBall(Color color)
        {
            var ball = ModelBuilder.CreateSphere(gp.ballR, color);
            var position = new Vector3(0, 0, gp.ballR);
            ball.Translate(position.X, position.Y, position.Z);
            models.Add(ball);
            gp.balls.Add(new ObjectParameters(models.Count - 1, position));
        }
        void BallTriangle(float x, float y)
        {
            float yStep = (float)Math.Sqrt(3) * gp.ballR;
            AddBall(gp.ballColors[0]);
            MoveBall(gp.balls[gp.balls.Count - 1], x, y);

            y += yStep;
            AddBall(gp.ballColors[1]);
            MoveBall(gp.balls[gp.balls.Count - 1], x - gp.ballR, y);
            AddBall(gp.ballColors[2]);
            MoveBall(gp.balls[gp.balls.Count - 1], x + gp.ballR, y);

            y += yStep;
            AddBall(gp.ballColors[3]);
            MoveBall(gp.balls[gp.balls.Count - 1], x, y);
            AddBall(gp.ballColors[4]);
            MoveBall(gp.balls[gp.balls.Count - 1], x - 2 * gp.ballR, y);
            AddBall(gp.ballColors[5]);
            MoveBall(gp.balls[gp.balls.Count - 1], x + 2 * gp.ballR, y);
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
                    float x1 = -gp.tableWidth / 2 + gp.tableWidth * ((float)i / N);
                    float x2 = -gp.tableWidth / 2 + gp.tableWidth * ((float)(i + 1) / N);
                    float y1 = -gp.tableDepth / 2 + gp.tableDepth * ((float)j / M);
                    float y2 = -gp.tableDepth / 2 + gp.tableDepth * ((float)(j + 1) / M);
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
            gp.table = new ObjectParameters(models.Count - 1, new Vector3(0, 0, 0));
        }
        void AddStick(Color c)
        {
            var model = ModelBuilder.CreateTube(gp.stickR, gp.stickL, c);
            model.RotateX((float)Math.PI / 2);
            model.Translate(0, -3 * gp.ballR, gp.ballR);
            models.Add(model);
            gp.stick = new ObjectParameters(models.Count - 1, new Vector3(0, 0, gp.ballR));
        }
        void AddWhiteBall()
        {
            var ball = ModelBuilder.CreateSphere(gp.ballR, Colors.White);
            ball.Translate(0, 0, gp.ballR);
            models.Add(ball);
            gp.whiteBall = new ObjectParameters(models.Count - 1, new Vector3(0, 0, gp.ballR));
        }
        void MoveBall(ObjectParameters ball, float x, float y)
        {
            ball.position.X += x;
            ball.position.Y += y;
            models[ball.id].Translate(x, y, 0);
        }
        void RotateStick(float angle)
        {
            models[gp.stick.id].RotateZ(angle);
        }
        public void RotateStickLeft() => rotateLeft = true;
        public void RotateStickRigth() => rotateRigth = true;
        public void Update()
        {
            if(rotateLeft)
            {
                RotateStick(-gp.angle);
                rotateLeft = false;
            }

            if(rotateRigth)
            {
                RotateStick(gp.angle);
                rotateRigth = false;
            }
        }
        public uint[,] Display()
        {
            return engine.Render(models);
        }

    }
}
