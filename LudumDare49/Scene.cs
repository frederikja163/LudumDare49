using OpenTK.Mathematics;

namespace LudumDare49
{
    public record Scene(Camera Camera, DirectionalLight DirectionalLight);

    public record DirectionalLight(
        Vector3 Direction,
        Vector3 Ambient,
        Vector3 Diffuse,
        Vector3 Specular);

    public record PointLight(
        Vector3 Position,
        
        float Constant,
        float Linear,
        float Quadratic,
        
        Vector3 Ambient,
        Vector3 Diffuse,
        Vector3 Specular);

    public record SpotLight(
        Vector3 Position,
        Vector3 Direction,
        
        float CutOff,
        float OutCutOff,
        
        float Constant,
        float Linear,
        float Quadratic,
        
        Vector3 Ambient,
        Vector3 Diffuse,
        Vector3 Specular);
}