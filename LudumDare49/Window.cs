using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    public sealed class Window : GameWindow
    {
        public static Window Instance { get; private set; }
        
        private Entity3D _obj;
        
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            Instance = this;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            _obj = new Entity3D("models.obj", "coffinB");
            
            GL.ClearColor(Color4.Magenta);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _obj.Render();
            
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            _obj.Dispose();
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