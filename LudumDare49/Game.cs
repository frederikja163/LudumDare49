using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LudumDare49
{
    public sealed class Game : IDisposable
    {
        public const float DevScore = 193;
        
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
        };
        
        private readonly Window _window;
        private readonly Entity3D _balance;
        private readonly Entity3D _pivot;
        private readonly Entity3D _ball;
        private readonly Entity3D _background;
        private readonly Scene _scene;
        private float _angularVelocity;
        private float _angle;
        private float _timeForPumpkin = 10;
        private Random _random = new Random();

        private bool _hasStarted = false;
        private float _highScore;
        private float _score = 0;
        private float _gameOverTime = 0;
        public bool IsGameOver { get; private set; } = false;

        private Weight Mouse => _weights[0];

        public Game(Window window)
        {
            _window = window;

            if (!File.Exists("highScore"))
            {
                _highScore = 0;
            }
            else
            {
                _highScore = float.Parse(File.ReadAllText("highscore"));
            }
            AddWeight();
            _scene = new Scene(new Camera(_window),
                new DirectionalLight(new Vector3(0.2f, -1.0f, 0.3f),
                    Vector3.One * 0.2f,
                    Vector3.One * 0.5f,
                    Vector3.One * 0.6f));

            _background = new Entity3D("scene", ".*", "Dirt2.png", "Dirt3.png");

            _ball = new Entity3D("models", "pumpkinLarge", "Dirt2.png", "Dirt3.png");
            
            _balance = new Entity3D("models", "Plank", "plankdiff.jpg", "plankspec.png");
            _balance.Transform.Rotation *= Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(90), 0);
            _balance.Transform.Position += Vector3.UnitY * 2.8f;

            _pivot = new Entity3D("models", "gravestone", "Dirt2.png", "Dirt3.png");
            _pivot.Transform.Rotation *= Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(180), 0);
        }

        public void AddWeight()
        {
            _timeForPumpkin = 10;
            float weight = (float)_random.NextDouble() + 0.5f;
            _weights.Add(new Weight(0, weight));
        }

        public void Update(float deltaT)
        {
            
            if (!IsGameOver)
            {
                UpdateMouse();
                if (!_hasStarted)
                {
                    return;
                }
                _score += deltaT;
                
                _timeForPumpkin -= deltaT;
                if (_timeForPumpkin <= 0)
                {
                    AddWeight();
                }
            }
            else
            {
                _gameOverTime += deltaT;
            }
            
            UpdateWeights(deltaT);
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
                    IsGameOver = true;
                    if (_score > _highScore)
                    {
                        _highScore = _score;
                        File.WriteAllText("highscore", _highScore.ToString());
                    }
                }
            }
        }

        private void UpdateMouse()
        {
            if (_window.MouseState.IsButtonDown(MouseButton.Left))
            {
                float mouse = _window.MousePosition.X / _window.ClientSize.X * 2 - 1;
                Mouse.Distance = mouse;
                _hasStarted = true;
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

            Text.Render(_timeForPumpkin.ToString(" time 00"), Sprite.WindowSize / 2 - new Vector2(250, 70), 0.6f);
            Text.Render(_score.ToString("score 000"), Sprite.WindowSize / 2 - new Vector2(250, 110), 0.6f);
            Text.Render(_highScore.ToString("highscore 000"), Sprite.WindowSize / 2 - new Vector2(275, 140), 0.4f);
            Text.Render(DevScore.ToString("devscore 000"), Sprite.WindowSize / 2 - new Vector2(260, 170), 0.4f);

            if (!_hasStarted)
            {
                WriteCenteredText("press the plank to start", -Sprite.WindowSize.Y / 3, 1.2f);
                WriteCenteredText("keep pumpkins on the plank to score points", -Sprite.WindowSize.Y / 3 + 200, 0.8f);
                WriteCenteredText("push on the edges for a bigger effect", -Sprite.WindowSize.Y / 3 + 150, 0.8f);
                WriteCenteredText("can you beat my score", -Sprite.WindowSize.Y / 3 + 100, 0.8f);
            }
            
            if (IsGameOver)
            {
                Text.Render("game over",  new Vector2(-2f * 39 * 4, Sprite.WindowSize.Y / 3), 2f);
                if (_score > DevScore)
                {
                    WriteCenteredText("but did you really lose",  Sprite.WindowSize.Y / 3 - 100, 0.8f);
                    WriteCenteredText("idk",  Sprite.WindowSize.Y / 3 - 150, 0.8f);
                    WriteCenteredText("you definitely beat me",  Sprite.WindowSize.Y / 3 - 200, 0.8f);
                    WriteCenteredText("good job",  Sprite.WindowSize.Y / 3 - 250, 0.8f);
                }

                if ((int)_gameOverTime % 2 == 1)
                {
                    WriteCenteredText("press space to restart", -Sprite.WindowSize.Y / 3, 1.2f);
                }
            }
        }

        private void WriteCenteredText(string text, float y, float size)
        {
            Text.Render(text, new Vector2(-size * 39 * text.Length / 2, y), size);
        }

        public void Dispose()
        {
            _balance.Dispose();
            _pivot.Dispose();
        }
    }
}