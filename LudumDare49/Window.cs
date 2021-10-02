using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    public sealed class Window : GameWindow
    {
        private Scene _scene;
        private Entity3D _obj;
        private Entity3D _pivot;
        
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _scene = new Scene(new Camera(this),
                new DirectionalLight(new Vector3(0.1f, -0.5f, -0.4f),
                    Vector3.One * 0.2f,
                    Vector3.One * 0.6f,
                    Vector3.One * 0.8f));
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            _obj = new Entity3D("models.obj", "Plank", "WoodenPlank.png");
            _pivot = new Entity3D("models.obj", "gravestone", "white.png");
            
            GL.ClearColor(Color4.Magenta);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _obj.Render(_scene);
            _pivot.Render(_scene);
            
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