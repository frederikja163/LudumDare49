using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LudumDare49.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49
{
    public sealed class Entity3D : IDisposable
    {
        private static readonly Shader Shader = new Shader("shader3d");
        private readonly Buffer<float> _vbo;
        private readonly Buffer<uint> _ebo;
        private readonly VertexArray _vao;
        private readonly int _indexCount = 0;

        public Entity3D(string path, string meshName)
        {
            using Stream file = Assets.LoadAsset(path);
            StreamReader sr = new StreamReader(file);

            while (!sr.ReadLine().StartsWith("o ") && !sr.EndOfStream) ;
            if (sr.EndOfStream)
            {
                return;
            }

            
            List<uint> indices = new ();
            List<float> vertices = new ();
            List<Vector3> positions = new ();
            List<Vector3> normals = new ();
            List<Vector2> textureCoordinates = new ();
            Dictionary<string, uint> vertexIndices = new ();
            string o = "";
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(' ');
                if (values[0] == "v")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    positions.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vn")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    normals.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vt")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    textureCoordinates.Add(new Vector2(x, y));
                }
                else if (values[0] == "f" && o == meshName)
                {
                    for (int i = 1; i < values.Length; i++)
                    {
                        if (!vertexIndices.TryGetValue(values[i], out uint index))
                        {
                            string[] vertValue = values[i].Split('/');
                            int positionIndex = int.Parse(vertValue[0]) - 1;
                            Vector3 position = positions[positionIndex];
                            int textureCoordinateIndex = int.Parse(vertValue[1]) - 1;
                            Vector2 textureCoordinate = textureCoordinates[textureCoordinateIndex];
                            int normalIndex = int.Parse(vertValue[2]) - 1;
                            Vector3 normal = normals[normalIndex];

                            vertices.Add(position.X);
                            vertices.Add(position.Y);
                            vertices.Add(position.Z);
                            vertices.Add(normal.X);
                            vertices.Add(normal.Y);
                            vertices.Add(normal.Z);
                            vertices.Add(textureCoordinate.X);
                            vertices.Add(textureCoordinate.Y);
                            index = (uint) vertices.Count / (3 + 3 + 2) - 1;
                            vertexIndices.Add(values[i], index);
                        }

                        indices.Add(index);
                    }
                }
                else if (values[0] == "o")
                {
                    o = values[1];
                }
            }

            _vbo = new Buffer<float>(BufferTargetARB.ArrayBuffer, vertices.ToArray());
            _ebo = new Buffer<uint>(BufferTargetARB.ElementArrayBuffer, indices.ToArray());
            _vao = new VertexArray();
            _vao.SetIndexBuffer(_ebo);
            _vao.AddVertexAttribute(_vbo, 0, 3, VertexAttribPointerType.Float, 8, 0);
            _vao.AddVertexAttribute(_vbo, 1, 3, VertexAttribPointerType.Float, 8, 3 * sizeof(float));
            _vao.AddVertexAttribute(_vbo, 2, 2, VertexAttribPointerType.Float, 8, 3 * 3 * sizeof(float));
            _indexCount = indices.Count;
        }

        public void Render()
        {
            Shader.SetUniform("uProjection", Camera.Projection);
            Shader.SetUniform("uView", Camera.View);
            
            Shader.Bind();
            _vao.Bind();
            
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            
            _vao.Unbind();
            Shader.Unbind();
        }

        public void Dispose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
        }
    }
}