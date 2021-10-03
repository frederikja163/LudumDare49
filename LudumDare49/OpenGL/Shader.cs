using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49.OpenGL
{
    public sealed class Shader : IDisposable
    {
        private readonly ProgramHandle _handle;
        private Dictionary<string, int> _uniformLocations = new ();

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

        private int GetUniformLocation(string name)
        {
            if (!_uniformLocations.TryGetValue(name, out int location))
            {
                location = GL.GetUniformLocation(_handle, name);
                _uniformLocations.Add(name, location);
            }
            return location;
        }

        public void SetUniform(string name, int value)
        {
            Bind();
            GL.Uniform1i(GetUniformLocation(name), value);
            Unbind();
        }

        public void SetUniform(string name, float value)
        {
            Bind();
            GL.Uniform1f(GetUniformLocation(name), value);
            Unbind();
        }

        public void SetUniform(string name, Vector2 value)
        {
            Bind();
            GL.Uniform2f(GetUniformLocation(name), value);
            Unbind();
        }
        
        public void SetUniform(string name, Vector3 value)
        {
            Bind();
            GL.Uniform3f(GetUniformLocation(name), value);
            Unbind();
        }
        
        public void SetUniform(string name, Vector4 value)
        {
            Bind();
            GL.Uniform4f(GetUniformLocation(name), value);
            Unbind();
        }

        public void SetUniform(string name, Matrix4 value)
        {
            Bind();
            GL.UniformMatrix4f(GetUniformLocation(name), false, value.Row0.X);
            Unbind();
        }

        public void SetScene(Scene scene)
        {
            Bind();
            GL.UniformMatrix4f(GetUniformLocation("uProjection"), false, scene.Camera.Projection.Row0.X);
            GL.UniformMatrix4f(GetUniformLocation("uView"), false, scene.Camera.View.Row0.X);
            GL.Uniform3f(GetUniformLocation("uViewPos"), scene.Camera.ViewPos);
            
            GL.Uniform3f(GetUniformLocation("uDirLight.direction"), scene.DirectionalLight.Direction);
            GL.Uniform3f(GetUniformLocation("uDirLight.ambient"), scene.DirectionalLight.Ambient);
            GL.Uniform3f(GetUniformLocation("uDirLight.diffuse"), scene.DirectionalLight.Diffuse);
            GL.Uniform3f(GetUniformLocation("uDirLight.specular"), scene.DirectionalLight.Specular);
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