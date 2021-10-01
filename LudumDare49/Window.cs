using LudumDare49.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    public sealed class Window : GameWindow
    {
        private Buffer<float> _vbo;
        private Buffer<uint> _ebo;
        private VertexArray _vao;
        private Shader _shader;
        private BufferHandle _buffer;
        
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shader = new Shader("basic");
            
            _vbo = new Buffer<float>(BufferTargetARB.ArrayBuffer,
                new float[]
                {
                    1f, 1f,
                    1f, -1f,
                    -1f, -1f,
                    -1f, 1f
                });
            _ebo = new Buffer<uint>(BufferTargetARB.ElementArrayBuffer, 0, 1, 2, 0, 2, 3);
            
            _vao = new VertexArray();
            _vao.SetIndexBuffer(_ebo);
            _vao.AddVertexAttribute(_vbo, _shader.GetAttributeLocation("vPos"), 2, VertexAttribPointerType.Float, 2 * sizeof(float), 0);
            
            GL.ClearColor(Color4.Magenta);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            _shader.Bind();
            _vao.Bind();
            
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            
            _vao.Unbind();
            _shader.Unbind();
            
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
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