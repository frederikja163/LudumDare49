using System;
using System.Collections.Generic;
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
        
        private List<Weight> _weights = new (){
            new Weight(0f, 3.2f),
            new Weight(-0.1f, 1.6f),
        };
        
        private readonly Window _window;
        private readonly Entity3D _balance;
        private readonly Entity3D _pivot;
        private readonly Entity3D _ball;
        private readonly Scene _scene;
        private float _angularVelocity;
        private float _angle;
        
        private Weight Mouse => _weights[0];

        public Game(Window window)
        {
            _window = window;
            
            _scene = new Scene(new Camera(_window),
                new DirectionalLight(new Vector3(0.2f, -1.0f, 0.3f),
                    Vector3.One * 0.2f,
                    Vector3.One * 0.5f,
                    Vector3.One * 1f));

            _ball = new Entity3D("models", "pumpkinLarge", "white.png");
            
            _balance = new Entity3D("models", "Plank", "Textura_tabla_1.jpg");
            _balance.Transform.Rotation *= Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(90), 0);
            _balance.Transform.Position += Vector3.UnitY * 1.8f;

            _pivot = new Entity3D("models", "gravestone", "white.png");
            _pivot.Transform.Rotation *= Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(180), 0);
        }

        public void AddWeight(float distance, float weight)
        {
            _weights.Add(new Weight(distance, weight));
        }

        public void Update(float deltaT)
        {
            Transform transform = _balance.Transform;
            for (int i = 1; i < _weights.Count; i++)
            {
                Weight weight = _weights[i];
                weight.Distance = MathHelper.Clamp(weight.Distance - _angle * deltaT * weight.Force, -0.95f, 0.95f);
            }
            
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
            if (angle < -0.45f)
            {
                _angle = -0.45f;
                _angularVelocity = 0;
            }
            else if (angle > 0.45f)
            {
                _angle = 0.45f;
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
            _balance.Render(_scene);
            _pivot.Render(_scene);

            Transform transform = _balance.Transform;
            for (int i = 1; i < _weights.Count; i++)
            {
                Weight weight = _weights[i];
                _ball.Transform.Scale = Vector3.One * weight.Force;
                float distance = 4.4f * weight.Distance;
                float radius = 0.6f * weight.Force;
                _ball.Transform.Rotation = Quaternion.FromEulerAngles(Vector3.UnitZ * distance / radius);
                _ball.Transform.Position = -transform.Forward * distance + transform.Position + transform.Up * radius;
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