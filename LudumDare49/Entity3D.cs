using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        private readonly Texture _texture;
        private readonly Material[] _materials;
        public Transform Transform { get; }
        
        public Entity3D(string path, string meshName, string texturePath)
        {
            LoadMaterials(path, out Dictionary<string, Material> allMaterials);
            LoadModel(path, meshName, allMaterials,
                out List<uint> indices, out List<float> vertices, out List<Material> usedMaterials);
            _materials = usedMaterials.ToArray();

#if DEBUG
            if (vertices.Count <= 0)
            {
                Console.Error.WriteLine($"Model does not exist! {meshName}");
            }
#endif

            _vbo = new Buffer<float>(BufferTargetARB.ArrayBuffer, vertices.ToArray());
            _ebo = new Buffer<uint>(BufferTargetARB.ElementArrayBuffer, indices.ToArray());
            _vao = new VertexArray();
            _vao.SetIndexBuffer(_ebo);
            _vao.AddVertexAttribute(_vbo, 0, 3, VertexAttribPointerType.Float, false, 9, 0);
            _vao.AddVertexAttribute(_vbo, 1, 3, VertexAttribPointerType.Float, false, 9, 3 * sizeof(float));
            _vao.AddVertexAttribute(_vbo, 2, 2, VertexAttribPointerType.Float, false, 9, (3 + 3) * sizeof(float));
            _vao.AddVertexAttribute(_vbo, 3, 1, VertexAttribPointerType.Float, false, 9, (3 + 3 + 2) * sizeof(float));
            _indexCount = indices.Count;
            _texture = new Texture(texturePath);
            Transform = new Transform();
        }

        private static void LoadMaterials(string path, out Dictionary<string, Material> materials)
        {
            using Stream file = Assets.LoadAsset(path + ".mtl");
            StreamReader sr = new StreamReader(file);

            materials = new();
            string currentMaterial = "";
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(' ');
                if (values[0] == "Ka")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    materials[currentMaterial] = materials[currentMaterial] with { Ambient = new Vector3(x, y, z) };
                }
                else if (values[0] == "Kd")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    materials[currentMaterial] = materials[currentMaterial] with { Diffuse = new Vector3(x, y, z) };
                }
                else if (values[0] == "Ks")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    materials[currentMaterial] = materials[currentMaterial] with { Specular = new Vector3(x, y, z) };
                }
                else if (values[0] == "newmtl")
                {
                    currentMaterial = values[1];
                    materials.Add(currentMaterial, new Material(Vector3.One, Vector3.One, Vector3.One, 8));
                }
            }
        }

        private static void LoadModel(string path, string meshName, Dictionary<string, Material> materials,
            out List<uint> indices, out List<float> vertices, out List<Material> usedMaterials)
        {
            using Stream file = Assets.LoadAsset(path + ".obj");
            StreamReader sr = new StreamReader(file);

            Regex regex = new Regex(meshName);
            
            indices = new();
            vertices = new();
            List<Vector3> positions = new();
            List<Vector3> normals = new();
            List<Vector2> textureCoordinates = new();
            Dictionary<string, uint> vertexIndices = new();
            string o = "";
            int currentMaterial = 0;
            usedMaterials = new();
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
                else if (values[0] == "usemtl" && regex.IsMatch(o))
                {
                    currentMaterial = usedMaterials.Count;
                    usedMaterials.Add(materials[values[1]]);
                }
                else if (values[0] == "f" && regex.IsMatch(o))
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
                            vertices.Add(currentMaterial);
                            index = (uint)vertices.Count / (3 + 3 + 2 + 1) - 1;
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
        }

        public void Render(Scene scene)
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                Shader.SetUniform($"uMaterial[{i}].ambient", _materials[i].Ambient);
                Shader.SetUniform($"uMaterial[{i}].diffuse", _materials[i].Diffuse);
                Shader.SetUniform($"uMaterial[{i}].specular", _materials[i].Specular);
                Shader.SetUniform($"uMaterial[{i}].shininess", _materials[i].Shininess);
            }
            
            Shader.SetUniform("uTexture", 0);
            _texture.Bind(TextureUnit.Texture0);
            
            Shader.SetScene(scene);
            Shader.SetUniform("uModel", Transform.Matrix);
            
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