using System;
using System.Drawing.Drawing2D;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LudumDare49
{
    public sealed class Game : IDisposable
    {
        private class Weight
        {
            public float Distance { get; set; }
            public float Force { get; set; }
            public float Torque => Distance * Force;

            public Weight(float distance, float force)
            {
                Distance = distance;
                Force = force;
            }
        }
        
        private Weight[] _weights = new []{
            new Weight(-0.55f, 1f),
            new Weight(0.45f,  1f),
            new Weight(0f, 5f),
            new Weight(-0.5f, 2f),
            new Weight(0.5f, 2f),
        };
        
        private readonly Window _window;
        private readonly Entity3D _balance;
        private readonly Entity3D _pivot;
        private readonly Entity3D _leftBall;
        private readonly Entity3D _rightBall;
        private readonly Scene _scene;
        private float _angularVelocity;
        private float _angle;

        private Weight PlankLeft => _weights[0];
        private Weight PlankRight => _weights[1];
        private Weight Mouse => _weights[2];
        private Weight LeftBall => _weights[3];
        private Weight RightBall => _weights[4];

        public Game(Window window)
        {
            _window = window;
            
            _scene = new Scene(new Camera(_window),
                new DirectionalLight(new Vector3(0.1f, -0.5f, -0.4f),
                    Vector3.One * 0.2f,
                    Vector3.One * 0.6f,
                    Vector3.One * 0.8f));

            _rightBall = new Entity3D("models.obj", "pumpkinLarge", "white.png");
            _leftBall = new Entity3D("models.obj", "pumpkinLarge", "white.png");
            
            _balance = new Entity3D("models.obj", "Plank", "WoodenPlank.png");
            _balance.Transform.Rotation *= Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(90), 0);
            _balance.Transform.Position += Vector3.UnitY * 1.8f;
            
            _pivot = new Entity3D("models.obj", "gravestone", "white.png");
            _pivot.Transform.Rotation *= Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(90), 0);
        }

        public void Update(float deltaT)
        {
            UpdateMouse();

            UpdatePlank(deltaT);
        }

        private void UpdateMouse()
        {
            if (_window.MouseState.IsButtonDown(MouseButton.Left))
            {
                float mouse = _window.MousePosition.X / _window.ClientSize.X * 2 - 1;
                Mouse.Distance = mouse;
            }
        }

        private void UpdatePlank(float deltaT)
        {
            float angularAcceleration = _weights.Sum(w => w.Torque);
            _angularVelocity -= angularAcceleration * deltaT;

            _angle += _angularVelocity * deltaT;
            if (_angle is < 0.45f and > -0.45f)
            {
                _balance.Transform.Rotation = Quaternion.FromEulerAngles(Vector3.UnitY * MathHelper.DegreesToRadians(90)) *
                                              Quaternion.FromEulerAngles(Vector3.UnitX * _angle);
            }
            else
            {
                _angularVelocity = 0;
            }
        }

        public void Render()
        {
            _balance.Render(_scene);
            _pivot.Render(_scene);
            _leftBall.Render(_scene);
            _rightBall.Render(_scene);
        }

        public void Dispose()
        {
            _balance.Dispose();
            _pivot.Dispose();
        }
    }
}