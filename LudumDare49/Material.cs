using LudumDare49.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49
{
    public sealed record Material(Vector3 Ambient, Vector3 Diffuse, Vector3 Specular, float Shininess);
}