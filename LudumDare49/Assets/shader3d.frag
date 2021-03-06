#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};
uniform Material uMaterial[106];
uniform sampler2D uDiffuse;
uniform sampler2D uSpecular;

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform DirLight uDirLight;

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
//#define NR_POINT_LIGHTS 0
//uniform PointLight pointLights[NR_POINT_LIGHTS];

struct SpotLight{
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};
//uniform SpotLight spotLight;

uniform vec3 uViewPos;

in vec3 fPos;
in vec3 fNorm;
in vec2 fTexCoord;
flat in int fMaterialIndex;

out vec4 Color;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 norm = normalize(fNorm);
    float distance = length(uViewPos - fPos);
    vec3 viewDir = normalize(uViewPos - fPos);

    vec3 result = CalcDirLight(uDirLight, norm, viewDir);
//    for(int i = 0; i < NR_POINT_LIGHTS; i++)
//        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
//    result += CalcSpotLight(spotLight, norm, FragPos, viewDir);
    
    result = mix(result, vec3(1, 1, 1), clamp(distance / 8 - 1, 0, 2));
    
    Color = vec4(result, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    
    // Diffuse shading.
    float diff = max(dot(normal, lightDir), 0.0);
    
    // Specular shading.
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial[fMaterialIndex].shininess);
    
    // Combine results.
    vec3 ambient  = light.ambient * uMaterial[fMaterialIndex].ambient * texture(uDiffuse, fTexCoord).rgb;
    vec3 diffuse  = light.diffuse * diff * uMaterial[fMaterialIndex].diffuse * texture(uDiffuse, fTexCoord).rgb;
    vec3 specular = light.specular * spec * uMaterial[fMaterialIndex].specular * texture(uSpecular, fTexCoord).rgb;
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // Diffuse shading.
    float diff = max(dot(normal, lightDir), 0.0);
    
    // Specular shading.
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial[fMaterialIndex].shininess);
    
    // Attenuation.
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));
    
    // Combine results.
    vec3 ambient  = light.ambient * uMaterial[fMaterialIndex].ambient * texture(uDiffuse, fTexCoord).rgb;
    vec3 diffuse  = light.diffuse * diff * uMaterial[fMaterialIndex].diffuse * texture(uDiffuse, fTexCoord).rgb;
    vec3 specular = light.specular * spec * uMaterial[fMaterialIndex].specular * texture(uSpecular, fTexCoord).rgb;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    // Diffuse shading.
    vec3 lightDir = normalize(light.position - fPos);
    float diff = max(dot(normal, lightDir), 0.0);

    // Specular shading.
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial[fMaterialIndex].shininess);

    // Attenuation.
    float distance    = length(light.position - fPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    // Spotlight intensity.
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    // Combine results.
    vec3 ambient  = light.ambient * uMaterial[fMaterialIndex].ambient * texture(uDiffuse, fTexCoord).rgb;
    vec3 diffuse  = light.diffuse * diff * uMaterial[fMaterialIndex].diffuse * texture(uDiffuse, fTexCoord).rgb;
    vec3 specular = light.specular * spec * uMaterial[fMaterialIndex].specular * texture(uSpecular, fTexCoord).rgb;
    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}