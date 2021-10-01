using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudumDare49.OpenGL
{
    public sealed class Buffer<T> : IDisposable
        where T : unmanaged
    {
        private readonly BufferHandle _handle;
        private readonly BufferTargetARB _target;

        public Buffer(BufferTargetARB target, nint size)
        {
            _target = target;
            _handle = GL.GenBuffer();
            Bind();
            GL.BufferData(target, size, (new T[size])[0], BufferUsageARB.StaticDraw);
            Unbind();
        }

        public Buffer(BufferTargetARB target, params T[] data)
        {
            _target = target;
            _handle = GL.GenBuffer();
            Bind();
            GL.BufferData(target, data, BufferUsageARB.StaticDraw);
            Unbind();
        }
        
        public void Bind()
        {
            GL.BindBuffer(_target, _handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(_target, BufferHandle.Zero);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_handle);
        }
    }
}