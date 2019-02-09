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
        enum CameraPosition
        {
            Static,
            Active,
            Tracking
        }
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
        CameraPosition cameraPosition = CameraPosition.Static;
        float power = 0;
        Vector3 cameraDirection = new Vector3(0, -GameParameters.cameraDistance, GameParameters.cameraDistance);
        bool staticLightOn = false;
        bool trackingLightOn = false;
        PointLight staticLight = new PointLight(new Vector3(0, 0, GameParameters.lightHeight));
        Stack<ObjectParameters> stack = new Stack<ObjectParameters>();
        public Game(Resolution res)
        {
            engine = new CPUEngine(res);
            engine.FOV = 60;

            AddTable(Colors.DarkGreen);
            AddStick(Colors.Brown);
            AddWhiteBall();
            AddBallTriangle(0, 5);
            AddBall(Colors.Turquoise, 5, 0);

            engine.SwitchToGouraudShading();
            //ActiveCamera();
            StaticCamera();
            //TrackingCamera();
            SwitchPointLight();
            //SwitchTrackingLight();
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
            float sqrt2 = (float)Math.Sqrt(2);
            const int N = 10;
            const int M = 20;
            var model = new Model() { isConvex = false };
            var n = new Vector4(0, 0, 1, 0);
            const float step = 0.1f;

            #region table base
            for (int i = 0; i < N; i++)
                for(int j = 0; j < M; j++)
                {
                    Vector4 p1, p2, p3, p4;
                    float x1 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)i / N);
                    float x2 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)(i + 1) / N);
                    float y1 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)j / M);
                    float y2 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)(j + 1) / M);

                    p1 = new Vector4(x1, y1, 0, 1);
                    p2 = new Vector4(x2, y1, 0, 1);
                    p3 = new Vector4(x1, y2, 0, 1);
                    p4 = new Vector4(x2, y2, 0, 1);

                    model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                    {
                        (p1, n) , (p2, n), (p3, n)
                    })
                    { color = c });
                    model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                    {
                        (p2, n), (p4, n), (p3, n)
                    })
                    { color = c });
                }
            #endregion

            #region borders on depth
            for (int i = 0; i < N; i++)
            {
                float x1 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)i / N);
                float x2 = -GameParameters.tableWidth / 2 + GameParameters.tableWidth * ((float)(i + 1) / N);

                if (x1 <= -GameParameters.tableWidth / 2 + GameParameters.pocketSize / 2 * sqrt2)
                    if (x2 >= -GameParameters.tableWidth / 2 + GameParameters.pocketSize / 2 * sqrt2)
                        x1 = -GameParameters.tableWidth / 2 + GameParameters.pocketSize / 2 * sqrt2;
                    else continue;
                if (x2 >= GameParameters.tableWidth / 2 - GameParameters.pocketSize / 2 * sqrt2)
                    if (x1 <= GameParameters.tableWidth / 2 - GameParameters.pocketSize / 2 * sqrt2)
                        x2 = GameParameters.tableWidth / 2 - GameParameters.pocketSize / 2 * sqrt2;
                    else continue;

                var n1 = new Vector4(1, 0, 0, 0);

                var p1 = new Vector4(x1, -GameParameters.tableDepth / 2, 0, 1);
                var p2 = new Vector4(x2, -GameParameters.tableDepth / 2, 0, 1);
                var p3 = new Vector4(x1, -GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
                var p4 = new Vector4(x2, -GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n1), (p2, n1), (p3, n1)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n1), (p2, n1), (p3, n1)
                })
                { color = c });

                p1 = p3; p1.Y -= GameParameters.borderThickness;
                p2 = p4; p2.Y -= GameParameters.borderThickness;
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });


                n1 *= -1;
                p1 = new Vector4(x1, GameParameters.tableDepth / 2, 0, 1);
                p2 = new Vector4(x2, GameParameters.tableDepth / 2, 0, 1);
                p3 = new Vector4(x1, GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
                p4 = new Vector4(x2, GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n1), (p2, n1), (p3, n1)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n1), (p2, n1), (p3, n1)
                })
                { color = c });


                p1 = p3; p1.Y += GameParameters.borderThickness;
                p2 = p4; p2.Y += GameParameters.borderThickness;
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });
            }
            #endregion

            #region borders on width
            for (int j = 0; j < M; j++)
            {
                var n1 = new Vector4(0, 1, 0, 0);
                float y1 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)j / M);
                float y2 = -GameParameters.tableDepth / 2 + GameParameters.tableDepth * ((float)(j + 1) / M);

                if (y1 <= -GameParameters.tableDepth / 2 + GameParameters.pocketSize / 2 * sqrt2)
                    if (y2 >= -GameParameters.tableDepth / 2 + GameParameters.pocketSize / 2 * sqrt2)
                        y1 = -GameParameters.tableDepth / 2 + GameParameters.pocketSize / 2 * sqrt2;
                    else continue;
                if (y2 >= GameParameters.tableDepth / 2 - GameParameters.pocketSize / 2 * sqrt2)
                    if (y1 <= GameParameters.tableDepth / 2 - GameParameters.pocketSize / 2 * sqrt2)
                        y2 = GameParameters.tableDepth / 2 - GameParameters.pocketSize / 2 * sqrt2;
                    else continue;
                if (y1 < -GameParameters.pocketSize / 2 && y2 > -GameParameters.pocketSize / 2) y2 = -GameParameters.pocketSize / 2;
                if (y1 < GameParameters.pocketSize / 2 && y2 > GameParameters.pocketSize / 2) y1 = GameParameters.pocketSize / 2;
                if (y1 > -GameParameters.pocketSize / 2 && y2 < GameParameters.pocketSize / 2) continue;

                var p1 = new Vector4(-GameParameters.tableWidth / 2, y1, 0, 1);
                var p2 = new Vector4(-GameParameters.tableWidth / 2, y2, 0, 1);
                var p3 = new Vector4(-GameParameters.tableWidth / 2, y1, GameParameters.tableHeight, 1);
                var p4 = new Vector4(-GameParameters.tableWidth / 2, y2, GameParameters.tableHeight, 1);
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n1), (p2, n1), (p3, n1)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n1), (p2, n1), (p3, n1)
                })
                { color = c });

                p1 = p3; p1.X -= GameParameters.borderThickness;
                p2 = p4; p2.X -= GameParameters.borderThickness;
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                n1 *= -1;
                p1 = new Vector4(GameParameters.tableWidth / 2, y1, 0, 1);
                p2 = new Vector4(GameParameters.tableWidth / 2, y2, 0, 1);
                p3 = new Vector4(GameParameters.tableWidth / 2, y1, GameParameters.tableHeight, 1);
                p4 = new Vector4(GameParameters.tableWidth / 2, y2, GameParameters.tableHeight, 1);
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n1), (p2, n1), (p3, n1)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n1), (p2, n1), (p3, n1)
                })
                { color = c });

                p1 = p3; p1.X += GameParameters.borderThickness;
                p2 = p4; p2.X += GameParameters.borderThickness;
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });
                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });
            }
            #endregion

            #region pockets
            for(float t = 0; t < 1; t += step)
            {
                float ang = (float)Math.PI / 2;
                var p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                var p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                var p3 = new Vector4(-GameParameters.tableWidth / 2 - GameParameters.borderThickness, p1.Y, GameParameters.tableHeight, 1);
                var p4 = new Vector4(p3.X, p2.Y, p3.Z, 1);

                p1.X -= (GameParameters.tableWidth + GameParameters.pocketSize) / 2;
                p2.X -= (GameParameters.tableWidth + GameParameters.pocketSize) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                ang *= -1;
                p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                p3 = new Vector4(GameParameters.tableWidth / 2 + GameParameters.borderThickness, p1.Y, GameParameters.tableHeight, 1);
                p4 = new Vector4(p3.X, p2.Y, p3.Z, 1);

                p1.X += (GameParameters.tableWidth + GameParameters.pocketSize) / 2;
                p2.X += (GameParameters.tableWidth + GameParameters.pocketSize) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                // 4th quarter
                ang = (float)Math.PI * 0.75f;
                p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                p3 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p4 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);


                p3.X -= (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.X -= (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p3.Y -= (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.Y -= (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);

                p1.X -= (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p2.X -= (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p1.Y -= (GameParameters.tableDepth + GameParameters.pocketSize  / 2) / 2;
                p2.Y -= (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                // 1st quarter
                ang = -(float)Math.PI * 0.25f;
                p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                p3 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p4 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);


                p3.X += (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.X += (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p3.Y += (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.Y += (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);

                p1.X += (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p2.X += (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p1.Y += (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;
                p2.Y += (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                // 2nd quarter
                ang = (float)Math.PI * 0.25f;
                p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                p3 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p4 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);


                p3.X -= (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.X -= (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p3.Y += (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.Y += (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);

                p1.X -= (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p2.X -= (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p1.Y += (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;
                p2.Y += (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });

                // 3rd quarter
                ang = -(float)Math.PI * 0.75f;
                p1 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p2 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);

                p3 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, t * (float)Math.PI + ang, GameParameters.tableHeight);
                p4 = PointExtensions.fromPolarCoordinates(GameParameters.pocketSize / 2, (t + step) * (float)Math.PI + ang, GameParameters.tableHeight);


                p3.X += (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.X += (GameParameters.tableWidth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p3.Y -= (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);
                p4.Y -= (GameParameters.tableDepth / 2 + GameParameters.borderThickness - GameParameters.pocketSize / 2);

                p1.X += (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p2.X += (GameParameters.tableWidth + GameParameters.pocketSize / 2) / 2;
                p1.Y -= (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;
                p2.Y -= (GameParameters.tableDepth + GameParameters.pocketSize / 2) / 2;

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p1, n), (p2, n), (p3, n)
                })
                { color = c });

                model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
                {
                    (p4, n), (p2, n), (p3, n)
                })
                { color = c });
            }

            // 4th quarter
            var pp1 = new Vector4(-GameParameters.tableWidth / 2 + GameParameters.pocketSize / 2 * sqrt2, -GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
            var pp2 = new Vector4(-GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 + GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableHeight, 1);
            var pp3 = new Vector4(pp1.X, pp1.Y - GameParameters.borderThickness, pp1.Z, 1);
            var pp4 = new Vector4(pp2.X - GameParameters.borderThickness, pp2.Y, pp2.Z, 1);
            var pp5 = new Vector4(pp3.X - GameParameters.borderThickness, pp3.Y, pp3.Z, 1);
            var pp6 = new Vector4(pp4.X, pp4.Y - GameParameters.borderThickness, pp4.Z, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, n), (pp3, n), (pp5, n)
            })
            { color = c });
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp2, n), (pp4, n), (pp6, n)
            })
            { color = c });

            // 1st quarter
            pp1 = new Vector4(GameParameters.tableWidth / 2 - GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
            pp2 = new Vector4(GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 - GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableHeight, 1);
            pp3 = new Vector4(pp1.X, pp1.Y + GameParameters.borderThickness, pp1.Z, 1);
            pp4 = new Vector4(pp2.X + GameParameters.borderThickness, pp2.Y, pp2.Z, 1);
            pp5 = new Vector4(pp3.X + GameParameters.borderThickness, pp3.Y, pp3.Z, 1);
            pp6 = new Vector4(pp4.X, pp4.Y + GameParameters.borderThickness, pp4.Z, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, n), (pp3, n), (pp5, n)
            })
            { color = c });
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp2, n), (pp4, n), (pp6, n)
            })
            { color = c });

            // 2nd quarter
            pp1 = new Vector4(-GameParameters.tableWidth / 2 + GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
            pp2 = new Vector4(-GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 - GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableHeight, 1);
            pp3 = new Vector4(pp1.X, pp1.Y + GameParameters.borderThickness, pp1.Z, 1);
            pp4 = new Vector4(pp2.X - GameParameters.borderThickness, pp2.Y, pp2.Z, 1);
            pp5 = new Vector4(pp3.X - GameParameters.borderThickness, pp3.Y, pp3.Z, 1);
            pp6 = new Vector4(pp4.X, pp4.Y + GameParameters.borderThickness, pp4.Z, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, n), (pp3, n), (pp5, n)
            })
            { color = c });
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp2, n), (pp4, n), (pp6, n)
            })
            { color = c });

            // 3rd quarter
            pp1 = new Vector4(GameParameters.tableWidth / 2 - GameParameters.pocketSize / 2 * sqrt2, -GameParameters.tableDepth / 2, GameParameters.tableHeight, 1);
            pp2 = new Vector4(GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 + GameParameters.pocketSize / 2 * sqrt2, GameParameters.tableHeight, 1);
            pp3 = new Vector4(pp1.X, pp1.Y - GameParameters.borderThickness, pp1.Z, 1);
            pp4 = new Vector4(pp2.X + GameParameters.borderThickness, pp2.Y, pp2.Z, 1);
            pp5 = new Vector4(pp3.X + GameParameters.borderThickness, pp3.Y, pp3.Z, 1);
            pp6 = new Vector4(pp4.X, pp4.Y - GameParameters.borderThickness, pp4.Z, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, n), (pp3, n), (pp5, n)
            })
            { color = c });
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp2, n), (pp4, n), (pp6, n)
            })
            { color = c });
            #endregion

            #region black wholes
            // 4th quarter
            pp1 = new Vector4(-GameParameters.tableWidth / 2 + GameParameters.pocketSize * sqrt2 / 2, -GameParameters.tableDepth / 2, 0.05f, 1);
            pp2 = new Vector4(-GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 + GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp3 = new Vector4(-GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 - GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp4 = new Vector4(-GameParameters.tableWidth / 2 - GameParameters.pocketSize * sqrt2 / 2, -GameParameters.tableDepth / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));

            // 1st quarter
            pp1 = new Vector4(GameParameters.tableWidth / 2 - GameParameters.pocketSize * sqrt2 / 2, GameParameters.tableDepth / 2, 0.05f, 1);
            pp2 = new Vector4(GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 - GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp3 = new Vector4(GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 + GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp4 = new Vector4(GameParameters.tableWidth / 2 + GameParameters.pocketSize * sqrt2 / 2, GameParameters.tableDepth / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));

            // 2nd quarter
            pp1 = new Vector4(-GameParameters.tableWidth / 2 + GameParameters.pocketSize * sqrt2 / 2, GameParameters.tableDepth / 2, 0.05f, 1);
            pp2 = new Vector4(-GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 - GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp3 = new Vector4(-GameParameters.tableWidth / 2, GameParameters.tableDepth / 2 + GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp4 = new Vector4(-GameParameters.tableWidth / 2 - GameParameters.pocketSize * sqrt2 / 2, GameParameters.tableDepth / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));

            // 3rd quarter
            pp1 = new Vector4(GameParameters.tableWidth / 2 - GameParameters.pocketSize * sqrt2 / 2, -GameParameters.tableDepth / 2, 0.05f, 1);
            pp2 = new Vector4(GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 + GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp3 = new Vector4(GameParameters.tableWidth / 2, -GameParameters.tableDepth / 2 - GameParameters.pocketSize * sqrt2 / 2, 0.05f, 1);
            pp4 = new Vector4(GameParameters.tableWidth / 2 + GameParameters.pocketSize * sqrt2 / 2, -GameParameters.tableDepth / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));

            // others
            pp1 = new Vector4(GameParameters.tableWidth / 2, -GameParameters.pocketSize / 2, 0.05f, 1);
            pp2 = new Vector4(GameParameters.tableWidth / 2, GameParameters.pocketSize / 2, 0.05f, 1);
            pp3 = new Vector4(GameParameters.tableWidth / 2 + GameParameters.pocketSize, -GameParameters.pocketSize / 2, 0.05f, 1);
            pp4 = new Vector4(GameParameters.tableWidth / 2 + GameParameters.pocketSize, GameParameters.pocketSize / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));

            pp1 = new Vector4(-GameParameters.tableWidth / 2, -GameParameters.pocketSize / 2, 0.05f, 1);
            pp2 = new Vector4(-GameParameters.tableWidth / 2, GameParameters.pocketSize / 2, 0.05f, 1);
            pp3 = new Vector4(-GameParameters.tableWidth / 2 - GameParameters.pocketSize, -GameParameters.pocketSize / 2, 0.05f, 1);
            pp4 = new Vector4(-GameParameters.tableWidth / 2 - GameParameters.pocketSize, GameParameters.pocketSize / 2, 0.05f, 1);

            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp1, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            model.Add(new Triangle(new List<(Vector4 point, Vector4 NormalVector)>()
            {
                (pp4, Vector4.Zero), (pp2, Vector4.Zero), (pp3, Vector4.Zero)
            }));
            #endregion

            models.Add(model);
            table = new ObjectParameters(model, Vector3.Zero);
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
            List<ObjectParameters> list = new List<ObjectParameters>();
            foreach (var b in balls)
                if (b != ball) list.Add(b);
            if (ball != whiteBall) list.Add(whiteBall);
            return list;
        }
        bool BallFalling(ObjectParameters ball)
        {
            ball.Fall(ball.velocity / 2);
            if (ball.position.X > GameParameters.tableWidth / 2 - GameParameters.ballRadius * (-1)) ball.Move(GameParameters.tableWidth / 2 - GameParameters.ballRadius * (-1) - ball.position.X, 0);
            if (ball.position.X < -GameParameters.tableWidth / 2 + GameParameters.ballRadius * (-1)) ball.Move(-GameParameters.tableWidth / 2 + GameParameters.ballRadius * (-1) - ball.position.X, 0);
            if (ball.position.Y > GameParameters.tableDepth / 2 - GameParameters.ballRadius * (-1)) ball.Move(0, GameParameters.tableDepth / 2 - GameParameters.ballRadius * (-1) - ball.position.Y);
            if (ball.position.Y < -GameParameters.tableDepth / 2 + GameParameters.ballRadius * (-1)) ball.Move(0, -GameParameters.tableDepth / 2 + GameParameters.ballRadius * (-1) - ball.position.Y);
            if (ball.position.Z < -GameParameters.ballRadius)
            {
                ball.ApplyVelocity(0, 0);
                if (ball == whiteBall)
                {
                    ball.Fall(ball.position.Z - GameParameters.ballRadius);
                    ball.Move(-ball.position.X, -ball.position.Y);
                }
                else
                {
                    models.Remove(ball.model);
                    //balls.Remove(ball);
                    stack.Push(ball);
                }
                return true;
            }
            return false;
        }
        void UpdateBallPosition(ObjectParameters ball)
        {
            if (ball.velocity == 0) return;
            var move = ball.UpdatePosition(GameParameters.ballAcceleration);
            float distance = move.Length();

            #region borders and pockets detection
            if ((ball.position.X <= -GameParameters.tableWidth / 2 + GameParameters.ballRadius + GameParameters.eps && ball.position.Y <= -GameParameters.tableDepth / 2 + GameParameters.ballRadius + GameParameters.eps)
                || (ball.position.X >= GameParameters.tableWidth / 2 - GameParameters.ballRadius - GameParameters.eps && ball.position.Y <= -GameParameters.tableDepth / 2 + GameParameters.ballRadius + GameParameters.eps)
                || (ball.position.X <= -GameParameters.tableWidth / 2 + GameParameters.ballRadius + GameParameters.eps && ball.position.Y >= GameParameters.tableDepth / 2 - GameParameters.ballRadius - GameParameters.eps)
                || (ball.position.X >= GameParameters.tableWidth / 2 - GameParameters.ballRadius - GameParameters.eps && ball.position.Y >= GameParameters.tableDepth / 2 - GameParameters.ballRadius - GameParameters.eps))
            {
                var dx = (float)Math.Sign(ball.position.X);
                var dy = (float)Math.Sign(ball.position.Y);
                float directon = 0;
                if (dx > 0 && dy > 0) directon = (float)Math.PI / 4;
                if (dx > 0 && dy < 0) directon = -(float)Math.PI / 4;
                if (dx < 0 && dy > 0) directon = (float)Math.PI * 0.75f;
                if (dx < 0 && dy < 0) directon = (float)Math.PI * 1.25f;
                ball.ApplyVelocity(ball.velocity, directon);
                if (BallFalling(ball)) return;
            }
            else
            {
                if (ball.position.X + GameParameters.ballRadius > GameParameters.tableWidth / 2)
                {
                    if (ball.position.Y >= -GameParameters.pocketSize / 2 + GameParameters.ballRadius && ball.position.Y <= GameParameters.pocketSize / 2 - GameParameters.ballRadius)
                    {
                        if (BallFalling(ball)) return;
                    }
                    else
                    {
                        float alpha = 1 - (GameParameters.tableWidth / 2 - ball.position.X - GameParameters.ballRadius + move.X) / move.X;
                        ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                        ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, (float)Math.PI - ball.directionAngle);
                    }
                }
                else if (ball.position.X - GameParameters.ballRadius < -GameParameters.tableWidth / 2)
                {

                    if (ball.position.Y >= -GameParameters.pocketSize / 2 + GameParameters.ballRadius && ball.position.Y <= GameParameters.pocketSize / 2 - GameParameters.ballRadius)
                    {
                        if (BallFalling(ball)) return;
                    }
                    else
                    {
                        float alpha = 1 - (-GameParameters.tableWidth / 2 - ball.position.X + GameParameters.ballRadius + move.X) / move.X;
                        ball.MoveInDirection(ball.directionAngle, -distance * alpha);
                        ball.ApplyVelocity(ball.velocity - GameParameters.ballAcceleration, (float)Math.PI - ball.directionAngle);
                    }
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
        Vector3 UpdateActiveCameraDirection()
        {
            return new Vector3(-(float)Math.Cos(stick.directionAngle), -(float)Math.Sin(stick.directionAngle), 1) * GameParameters.cameraDistance;
        }
        void UpdateLights()
        {
            engine.RemoveAllLights();
            if (staticLightOn) engine.AddLight(staticLight);
            if (trackingLightOn)
            {
                engine.AddLight(new ReflectorLight(new Vector3(whiteBall.position.X, whiteBall.position.Y, GameParameters.lightHeight), whiteBall.position));
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
            cameraPosition = CameraPosition.Static;
            engine.ChangeCameraPosition(new Vector3(0.5f, 0, 1.5f) * GameParameters.cameraDistance, new Vector3(0, 0, 0));
        }
        public void ActiveCamera()
        {
            cameraPosition = CameraPosition.Active;
            cameraDirection = UpdateActiveCameraDirection();
            engine.ChangeCameraPosition(stick.position + cameraDirection, stick.position);
        }
        public void TrackingCamera()
        {
            cameraPosition = CameraPosition.Tracking;
            engine.ChangeCameraPosition(new Vector3(1, 0, 1) * GameParameters.cameraDistance, whiteBall.position);
        }
        public void SwitchPointLight()
        {
            staticLightOn = !staticLightOn;
            UpdateLights();
        }
        public void SwitchTrackingLight()
        {
            trackingLightOn = !trackingLightOn;
            UpdateLights();
        }
        public void ConstantShading() => engine.SwitchToConstantShading();
        public void GouraudShading() => engine.SwitchToGouraudShading();
        public void PhongShading() => engine.SwitchToPhongShading();
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
                        if (cameraPosition == CameraPosition.Active)
                        {
                            cameraDirection = UpdateActiveCameraDirection();
                            engine.ChangeCameraPosition(stick.position + cameraDirection, stick.position);
                        }
                        rotateLeft = false;
                    }
                    if (rotateRigth)
                    {
                        stick.Rotate(GameParameters.angleStep);
                        if (cameraPosition == CameraPosition.Active)
                        {
                            cameraDirection = UpdateActiveCameraDirection();
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
                        if (whiteBall.velocity > 0)
                            switch(cameraPosition)
                            {
                                case CameraPosition.Active:
                                    engine.ChangeCameraPosition(whiteBall.position + cameraDirection, whiteBall.position);
                                    break;
                                case CameraPosition.Tracking:
                                    engine.ChangeCameraPosition(new Vector3(1, 0, 1) * GameParameters.cameraDistance, whiteBall.position);
                                    break;
                            }
                        UpdateBallPosition(whiteBall);
                        foreach (var b in balls)
                            UpdateBallPosition(b);
                        while(stack.Count > 0)
                        {
                            var ball = stack.Pop();
                            balls.Remove(ball);
                        }
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
