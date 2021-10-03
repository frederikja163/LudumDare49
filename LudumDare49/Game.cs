using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
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
        
        private List<Weight> _weights = new (){
            new Weight(0f, 5f),
            new Weight(0f, 1f),
        };
        
        private readonly Window _window;
        private readonly Entity3D _balance;
        private readonly Entity3D _pivot;
        private readonly Entity3D _ball;
        private readonly Entity3D _background;
        private readonly Scene _scene;
        private float _angularVelocity;
        private float _angle;
        private Random _random = new Random();
        private readonly Timer _timer;

        private bool _isGameOver = false;
        
        private Weight Mouse => _weights[0];

        public Game(Window window)
        {
            _window = window;

            _timer = new Timer(TimerElapsed, null, 10000, 10000);
            
            _scene = new Scene(new Camera(_window),
                new DirectionalLight(new Vector3(0.2f, -1.0f, 0.3f),
                    Vector3.One * 0.2f,
                    Vector3.One * 0.5f,
                    Vector3.One * 0.6f));

            _background = new Entity3D("scene", ".*", "white.png");

            _ball = new Entity3D("models", "pumpkinLarge", "white.png");
            
            _balance = new Entity3D("models", "Plank", "Textura_tabla_1.jpg");
            _balance.Transform.Rotation *= Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(90), 0);
            _balance.Transform.Position += Vector3.UnitY * 2.8f;

            _pivot = new Entity3D("models", "gravestone", "white.png");
            _pivot.Transform.Rotation *= Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(180), 0);
        }

        private void TimerElapsed(object? state)
        {
            AddWeight();
        }

        public void AddWeight()
        {
            float weight = (float)_random.NextDouble() + 0.5f;
            _weights.Add(new Weight(0, weight));
        }

        public void Update(float deltaT)
        {
            UpdateWeights(deltaT);
            UpdateMouse();
            UpdatePlank(deltaT);
        }

        private void UpdateWeights(float deltaT)
        {
            Transform transform = _balance.Transform;
            for (int i = 1; i < _weights.Count; i++)
            {
                Weight weight = _weights[i];
                weight.Distance -= _angle * deltaT * 1/weight.Force;
                if (weight.Distance is > 0.95f or < -0.95f)
                {
                    _isGameOver = true;
                }
            }
        }

        private void UpdateMouse()
        {
            if (_isGameOver)
            {
                return;
            }
            
            if (_window.MouseState.IsButtonDown(MouseButton.Left))
            {
                float mouse = _window.MousePosition.X / _window.ClientSize.X * 2 - 1;
                Mouse.Distance = mouse;
            }
            else
            {
                Mouse.Distance = 0;
            }
        }

        private void UpdatePlank(float deltaT)
        {
            float angularAcceleration = _weights.Sum(w => w.Torque);
            _angularVelocity -= angularAcceleration * deltaT;

            float angle = _angle + _angularVelocity * deltaT;
            if (angle < -0.35f)
            {
                _angle = -0.35f;
                _angularVelocity = 0;
            }
            else if (angle > 0.35f)
            {
                _angle = 0.35f;
                _angularVelocity = 0;
            }
            else
            {
                _angle = angle;
            }
            _balance.Transform.Rotation = Quaternion.FromEulerAngles(Vector3.UnitY * MathHelper.DegreesToRadians(90)) *
                                          Quaternion.FromEulerAngles(Vector3.UnitX * _angle);
        }

        public void Render()
        {
            _background.Render(_scene);
            _balance.Render(_scene);
            _pivot.Render(_scene);

            Transform transform = _balance.Transform;
            for (int i = 1; i < _weights.Count; i++)
            {
                Weight weight = _weights[i];
                _ball.Transform.Scale = Vector3.One * weight.Force;
                const float plankLength = 6;
                float distance = plankLength * weight.Distance;
                float radius = 0.6f * weight.Force;
                _ball.Transform.Rotation = Quaternion.FromEulerAngles(Vector3.UnitZ * distance / radius);
                if (weight.Distance is > 1f or < -1f)
                {
                    Vector3 pos = -transform.Forward * plankLength * MathF.Sign(weight.Distance) + transform.Position + transform.Up * radius;
                    _ball.Transform.Position = -Vector3.UnitX * (weight.Distance - MathF.Sign(weight.Distance)) * plankLength + pos;
                }
                else
                {
                    _ball.Transform.Position = -transform.Forward * distance + transform.Position + transform.Up * radius;
                }
                _ball.Render(_scene);
            }
        }

        public void Dispose()
        {
            _balance.Dispose();
            _pivot.Dispose();
        }
    }
}