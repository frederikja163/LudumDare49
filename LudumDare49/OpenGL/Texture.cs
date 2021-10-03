using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudumDare49.OpenGL
{
    public sealed class Texture : IDisposable
    {
        private readonly TextureHandle _handle;
        public int Width { get; }
        public int Height { get; }

        public Texture(string path)
        {
            _handle = GL.GenTexture();

            using Stream stream = Assets.LoadAsset(path);
            using Bitmap bitmap = new Bitmap(stream);
            Width = bitmap.Width;
            Height = bitmap.Height;
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            Bind();
            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            Unbind();
        }

        public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2d, _handle);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2d, TextureHandle.Zero);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
        }
    }
}