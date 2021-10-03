using LudumDare49.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49
{
    public sealed class Sprite
    {
        private static Vector2 _windowSize;
        public static Vector2 WindowSize
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                _orthographicMatrix = Matrix4.CreateOrthographic(value.X, value.Y, 0.01f, 100f);
                Shader.SetUniform("uView", ViewMatrix);
                Shader.SetUniform("uPerspective", _orthographicMatrix);
            }
        }
        private static Matrix4 _orthographicMatrix;
        private static readonly Matrix4 ViewMatrix = Matrix4.LookAt(-Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
        private static readonly Shader Shader = new Shader("sprite");
        private static readonly Buffer<float> Vbo = new Buffer<float>(BufferTargetARB.ArrayBuffer,
            -1, 1,
            -1, 0,
            0, 0,
            0, 1
        );
        private static readonly Buffer<uint> Ebo = new Buffer<uint>(BufferTargetARB.ElementArrayBuffer,
            0, 1, 2,
            0, 2, 3);
        private static readonly VertexArray Vao = new VertexArray();

        static Sprite()
        {
            Vao.SetIndexBuffer(Ebo);
            Vao.AddVertexAttribute(Vbo, 0, 2, VertexAttribPointerType.Float, false, 2, 0);
        }
        
        private readonly Texture _texture;

        public Sprite(Texture texture)
        {
            _texture = texture;
        }

        public void Render(Vector4 texCoord, Vector2 size, Vector2 position)
        {
            Shader.SetUniform("uTexCoord", new Vector4(texCoord.X / _texture.Width, texCoord.Y / _texture.Height, 
                texCoord.Z / _texture.Width, texCoord.W / _texture.Height));
            Shader.SetUniform("uSize", size);
            Shader.SetUniform("uPosition", position * new Vector2(-1, 1));
            
            Shader.SetUniform("uTexture", 0);
            
            _texture.Bind(TextureUnit.Texture0);
            Vao.Bind();
            Shader.Bind();
            
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            
            Vao.Unbind();
            Shader.Unbind();
        }
    }
}