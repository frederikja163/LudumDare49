using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudumDare49.OpenGL
{
    public sealed class VertexArray : IDisposable
    {
        private readonly VertexArrayHandle _handle;

        public VertexArray()
        {
            _handle = GL.GenVertexArray();
        }

        public void SetIndexBuffer(Buffer<uint> buffer)
        {
            Bind();
            buffer.Bind();
            Unbind();
            buffer.Unbind();
        }

        public void AddVertexAttribute<T>(Buffer<T> buffer, int location, int size, VertexAttribPointerType type, int stride, nint offset)
            where T : unmanaged
        {
            Bind();
            buffer.Bind();
            
            GL.EnableVertexAttribArray((uint)location);
            GL.VertexAttribPointer((uint)location, size, type, false, stride, offset);
            
            Unbind();
            buffer.Unbind();
        }

        public void Bind()
        {
            GL.BindVertexArray(_handle);
        }

        public void Unbind()
        {
            GL.BindVertexArray(VertexArrayHandle.Zero);
        }
        
        public void Dispose()
        {
            GL.DeleteVertexArray(_handle);
        }
    }
}