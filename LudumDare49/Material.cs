using LudumDare49.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49
{
    public sealed record Material(Texture Diffuse, Texture Specular, float Shininess);
}