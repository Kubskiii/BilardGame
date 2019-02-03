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
        List<ObjectParameters> balls = new List<ObjectParameters>();
        ObjectParameters table;
        ObjectParameters stick;
        ObjectParameters whiteball;
        float tableWidth = 40, tableDepth = 80;
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 90;
            engine.ChangeCameraPosition(new Vector3(-50, 0, 50), new Vector3(0, 0, 0));
            engine.SwitchToGouraudShading();
            engine.AddLight(new PointLight(new Vector3(0, 0, 20)));
            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            BallTriangle();
        }
        void AddBall(Color color)
        {
            var ball = ModelBuilder.CreateSphere(1, color);
            var position = new Vector3(0, 0, 1);
            ball.Translate(position.X, position.Y, position.Z);
            models.Add(ball);
            balls.Add(new ObjectParameters(models.Count - 1, position));
        }
        void BallTriangle()
        {
            float xbeg = 0, ybeg = 10;
            float ystep = (float)Math.Sqrt(3);
            Color[] colors = new Color[6]
            {
                Colors.Red, Colors.Yellow, Colors.Blue, Colors.Magenta, Colors.Orange, Colors.DarkGray
            };
            AddBall(colors[0]);
            MoveBall(balls[balls.Count - 1], xbeg, ybeg);

            ybeg += ystep;
            AddBall(colors[1]);
            MoveBall(balls[balls.Count - 1], xbeg - 1, ybeg);
            AddBall(colors[2]);
            MoveBall(balls[balls.Count - 1], xbeg + 1, ybeg);

            ybeg += ystep;
            AddBall(colors[3]);
            MoveBall(balls[balls.Count - 1], xbeg, ybeg);
            AddBall(colors[4]);
            MoveBall(balls[balls.Count - 1], xbeg - 2, ybeg);
            AddBall(colors[5]);
            MoveBall(balls[balls.Count - 1], xbeg + 2, ybeg);
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
                    float x1 = -tableWidth / 2 + tableWidth * ((float)i / N);
                    float x2 = -tableWidth / 2 + tableWidth * ((float)(i + 1) / N);
                    float y1 = -tableDepth / 2 + tableDepth * ((float)j / M);
                    float y2 = -tableDepth / 2 + tableDepth * ((float)(j + 1) / M);
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
            table = new ObjectParameters(models.Count - 1, new Vector3(0, 0, 0));
        }
        void AddStick(Color c)
        {
            var model = ModelBuilder.CreateTube(0.5f, 50, c);
            model.RotateX((float)Math.PI / 2);
            model.Translate(0, 1, 5);
            models.Add(model);
            stick = new ObjectParameters(models.Count - 1, new Vector3(0, 0, 1));
        }
        void AddWhiteBall()
        {
            var ball = ModelBuilder.CreateSphere(1, Colors.White);
            ball.Translate(0, 0, 1);
            models.Add(ball);
            whiteball = new ObjectParameters(models.Count - 1, new Vector3(0, 0, 1));
        }

        void MoveBall(ObjectParameters ball, float x, float y)
        {
            ball.position.X += x;
            ball.position.Y += y;
            models[ball.id].Translate(x, y, 0);
        }
        public uint[,] Display()
        {
            return engine.Render(models);
        }

    }
}
