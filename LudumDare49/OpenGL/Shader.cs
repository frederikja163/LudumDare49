using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudumDare49.OpenGL
{
    public sealed class Shader : IDisposable
    {
        private readonly ProgramHandle _handle;
        
        public Shader(string path)
        {
            using StreamReader vertStream = new StreamReader(Assets.LoadAsset(path + ".vert"));
            using StreamReader fragStream = new StreamReader(Assets.LoadAsset(path + ".frag"));

            ShaderHandle vertHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertHandle, vertStream.ReadToEnd());
            GL.CompileShader(vertHandle);
#if DEBUG
            GL.GetShaderInfoLog(vertHandle, out string vertInfo);
            if (!string.IsNullOrWhiteSpace(vertInfo))
            {
                Console.Error.Write($"Vertex shader compilation failed {path}: " + vertInfo);
            }
#endif
            
            ShaderHandle fragHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragHandle, fragStream.ReadToEnd());
            GL.CompileShader(fragHandle);
#if DEBUG
            GL.GetShaderInfoLog(fragHandle, out string fragInfo);
            if (!string.IsNullOrWhiteSpace(fragInfo))
            {
                Console.Error.Write($"Fragment shader compilation failed {path}: " + vertInfo);
            }
#endif

            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vertHandle);
            GL.AttachShader(_handle, fragHandle);
            
            GL.LinkProgram(_handle);
#if DEBUG
            GL.GetProgramInfoLog(_handle, out string programInfo);
            if (!string.IsNullOrWhiteSpace(programInfo))
            {
                Console.Error.Write($"Program link failed {path}: " + vertInfo);
            }
#endif
            
            GL.DetachShader(_handle, vertHandle);
            GL.DeleteShader(vertHandle);
            GL.DetachShader(_handle, fragHandle);
            GL.DeleteShader(fragHandle);
        }

        public int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(_handle, name);
        }

        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_handle, name);
        }

        public void SetUniform(int location, int value)
        {
            Bind();
            GL.Uniform1i(location, value);
            Unbind();
        }

        public void Bind()
        {
            GL.UseProgram(_handle);
        }
        
        public void Unbind()
        {
            GL.UseProgram(ProgramHandle.Zero);
        }
        
        public void Dispose()
        {
            GL.DeleteProgram(_handle);
        }
    }
}