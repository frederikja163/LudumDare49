using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    public sealed class Window : GameWindow
    {
        private Game _game;
        
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            _game = new Game(this);
            
            GL.ClearColor(Color4.Magenta);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            _game.Update((float)args.Time * 0.5f);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _game.Render();
            
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _game.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Render();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0, 0, e.Width, e.Height);
            
            Render();
        }
    }
}